using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ZiraLink.IDS;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(), // For ui login
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> { new ApiScope("api1") };

    public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            ClientId = "bff",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
    
            // where to redirect to after login
            RedirectUris = { "https://ot.api.kub.aghdam.nl/signin-oidc", "https://localhost:5212/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://ot.api.kub.aghdam.nl/signout-callback-oidc", "https://localhost:5212/signout-callback-oidc" },

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "api1"
            }
        }
    };
}
