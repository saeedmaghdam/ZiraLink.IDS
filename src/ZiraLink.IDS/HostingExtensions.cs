using ZiraLink.IDS.Data;
using ZiraLink.IDS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using ZiraLink.IDS.Framework;
using System.Net;
using System.Net.Security;

namespace ZiraLink.IDS;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        var pathToExe = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
        var environment = configuration["ASPNETCORE_ENVIRONMENT"];
        var connectionString = configuration["ASPNETCORE_ENVIRONMENT"] == "Development" ? $"Data Source={Path.Combine(pathToExe, "database.db")}" : configuration["ZIRALINK_CONNECTIONSTRINGS_DB"];

        if (configuration["ASPNETCORE_ENVIRONMENT"] == "Test")
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                string expectedThumbprint = "10CE57B0083EBF09ED8E53CF6AC33D49B3A76414";
                if (certificate!.GetCertHashString() == expectedThumbprint)
                    return true;

                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;

                return false;
            };
        }

        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(options =>
        {
            // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
            options.EmitStaticAudienceClaim = true;

            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Test")
                options.IssuerUri = new Uri(configuration["ZIRALINK_ISSUER_URL"]!).ToString();
        })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddAspNetIdentity<ApplicationUser>();


        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit 
            // configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddLocalApiAuthentication();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app, IConfiguration configuration)
    {
        app.UseForwardedHeaders();

        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
        //}
        app.UseErrorHandler();

        InitializeDatabase(app, configuration);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapControllers()
            .RequireAuthorization();
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }

    private static void InitializeDatabase(IApplicationBuilder app, IConfiguration configuration)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients(configuration))
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();
            if (!applicationDbContext.Users.Any())
            {
                SeedData.EnsureSeedData(app as WebApplication);
            }
        }
    }
}
