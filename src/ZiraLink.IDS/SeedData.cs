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
            var alice = userMgr.FindByNameAsync("logon").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "logon",
                    Email = "smasafat@gmail.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Saeed Aghdam"),
                            new Claim(JwtClaimTypes.GivenName, "Saeed"),
                            new Claim(JwtClaimTypes.FamilyName, "Aghdam"),
                            new Claim(JwtClaimTypes.Role, "admin"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                //Log.Debug("logon created");
            }
            else
            {
                //Log.Debug("logon already exists");
            }
        }
    }
}
