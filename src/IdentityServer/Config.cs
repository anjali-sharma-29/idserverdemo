﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {

        public static IEnumerable<ApiScope> ApiScopes =>
                new List<ApiScope>
                {
                        new ApiScope("api1", "My API"),                        
                };
        public static IEnumerable<IdentityResource> IdentityResources =>
                new List<IdentityResource>
                {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                 };


        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                   new Client
                   {
                           // Console client
                            ClientId = "client",
                            // no interactive user, use the clientid/secret for authentication
                            AllowedGrantTypes = GrantTypes.ClientCredentials,
                            // secret for authentication
                           ClientSecrets =
                            {
                                new Secret("secret".Sha256())
                            },
                            // scopes that client has access to
                            AllowedScopes = { "api1" }
                    },
                   new Client
                    {
                            ClientId = "mvc",
                            ClientSecrets = { new Secret("secret".Sha256()) },
                            AllowedGrantTypes = GrantTypes.Code,
                            // where to redirect to after login
                            RedirectUris = { "https://localhost:44316/signin-oidc" },
                            // where to redirect to after logout
                            PostLogoutRedirectUris = { "https://localhost:44316/signout-callback-oidc"},
                            AllowedScopes = new List<string>
                            {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "api1"
                            }
                    },
                   // JavaScript Client
                   new Client
                   {
                        ClientId = "js",
                        ClientName = "JavaScript Client",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RedirectUris = { "https://localhost:44339/callback.html" },
                        PostLogoutRedirectUris = { "https://localhost:44339/index.html" },
                        AllowedCorsOrigins = { "https://localhost:44339" },
                        AllowedScopes =
                        {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                        }
                   }
            };
    }
}