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

    public static IEnumerable<Client> Clients
    {
        get
        {
            var apiBaseUri = new Uri(Environment.GetEnvironmentVariable("ZIRALINK_API_URL"));
            var apiSignInUri = new Uri(apiBaseUri, "signin-oidc");
            var apiSignOutUri = new Uri(apiBaseUri, "signout-callback-oidc");

            var clientBaseUri = new Uri(Environment.GetEnvironmentVariable("ZIRALINK_CLIENT_URL"));
            var clientSignInUri = new Uri(apiBaseUri, "signin-oidc");
            var clientSignOutUri = new Uri(apiBaseUri, "signout-callback-oidc");

            return new List<Client>
            {
                new Client
                {
                    ClientId = "bff",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect to after login
                    RedirectUris = { apiSignInUri.ToString(), "https://localhost:5212/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { apiSignOutUri.ToString(), "https://localhost:5212/signout-callback-oidc" },

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
}
