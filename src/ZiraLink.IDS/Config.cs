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
        new List<ApiScope> { new ApiScope("ziralink"), new ApiScope(IdentityServerConstants.LocalApi.ScopeName) };

    public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            ClientId = "bff",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            // where to redirect to after login
            RedirectUris = { "https://api.ziralink.com:6001/signin-oidc", "https://localhost:5212/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://api.ziralink.com:6001/signout-callback-oidc", "https://localhost:5212/signout-callback-oidc" },

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Phone,
                IdentityServerConstants.StandardScopes.Address,
                "ziralink"
            }
        },
        new Client
        {
            ClientId = "back",

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { "ziralink", IdentityServerConstants.LocalApi.ScopeName, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Email }
        }
    };
}
