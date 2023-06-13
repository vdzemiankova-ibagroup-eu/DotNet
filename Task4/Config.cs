using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace Task4
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
    new ApiScope[]
    {
        new ApiScope("myapi.read"),
        new ApiScope("myapi.write"),
    };

        public static IEnumerable<IdentityResource> IdentityResources =>
    new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };
        public static List<TestUser> TestUsers =>
    new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1144",
            Username = "mukesh",
            Password = "mukesh",
            Claims =
            {
                new Claim(JwtClaimTypes.Name, "Mukesh Murugan"),
                new Claim(JwtClaimTypes.GivenName, "Mukesh"),
                new Claim(JwtClaimTypes.FamilyName, "Murugan"),
                new Claim(JwtClaimTypes.WebSite, "http://codewithmukesh.com"),
            }
        }
    };
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
        {
            new ApiResource("myapi", "My API")
        };
        }

        public static IEnumerable<ApiResource> ApiResources =>
    new ApiResource[]
    {
        new ApiResource("myApi")
        {
            Scopes = new List<string>{ "myapi.read","myapi.write" },
            ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
        }
    };

        public static IEnumerable<Client> Clients =>
    new Client[]
    {
        new Client
        {
            ClientId = "cwm.client",
            ClientName = "Client Credentials Client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "myapi.read" }
        },
    };

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
        {
            new Client
            {
                ClientId = "myclient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("mysecret".Sha256())
                },
                AllowedScopes = { "myapi" }
            }
        };
        }
    }
}
