using System.Security.Claims;
using IdentityModel;
using ZiraLink.IDS.Data;
using ZiraLink.IDS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ZiraLink.IDS;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logon = userMgr.FindByNameAsync("logon").Result;
            if (logon == null)
            {
                logon = new ApplicationUser
                {
                    UserName = "logon",
                    Email = "smasafat@gmail.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(logon, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(logon, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Saeed Aghdam"),
                            new Claim(JwtClaimTypes.GivenName, "Saeed"),
                            new Claim(JwtClaimTypes.FamilyName, "Aghdam"),
                            new Claim(JwtClaimTypes.Role, "admin"),
                        }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
            {
                var test = userMgr.FindByNameAsync("test").Result;
                if (test == null)
                {
                    var userId = "c2bacf97-47ab-452d-ba04-937a001f72ac";

                    test = new ApplicationUser
                    {
                        Id = userId,
                        UserName = "test",
                        Email = "ziralink@aghdam.nl",
                        EmailConfirmed = true,
                    };
                    var result = userMgr.CreateAsync(test, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(test, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Test User"),
                            new Claim(JwtClaimTypes.GivenName, "Test"),
                            new Claim(JwtClaimTypes.FamilyName, "User"),
                            new Claim(JwtClaimTypes.Role, "admin")
                        }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
            }
        }
    }
}
