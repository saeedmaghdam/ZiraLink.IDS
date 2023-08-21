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

    public static IEnumerable<Client> GetClients(IConfiguration configuration)
    {
        var apiBaseUri = new Uri(configuration["ZIRALINK_API_URL"]);
        var apiSignInUri = new Uri(apiBaseUri, "signin-oidc");
        var apiSignOutUri = new Uri(apiBaseUri, "signout-callback-oidc");

        var clientBaseUri = new Uri(configuration["ZIRALINK_CLIENT_URL"]);
        var clientSignInUri = new Uri(clientBaseUri, "signin-oidc");
        var clientSignOutUri = new Uri(clientBaseUri, "signout-callback-oidc");

        return new List<Client>
        {
            new Client
            {
                ClientId = "bff",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { apiSignInUri.ToString() },

                // where to redirect to after logout
                PostLogoutRedirectUris = { apiSignOutUri.ToString() },

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
            },
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { clientSignInUri.ToString() },

                // where to redirect to after logout
                PostLogoutRedirectUris = { clientSignOutUri.ToString() },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Address
                }
            },
        };
    }
}
