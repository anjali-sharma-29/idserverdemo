// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityServer4.Quickstart.EntityFramework-4.0.0;trusted_connection=yes;";
            services.AddIdentityServer()
                                  .AddDeveloperSigningCredential() //This is for dev only scenarios when→you don’t have a certificate to use.                                 
                                                                   //.AddInMemoryIdentityResources(Config.IdentityResources)
                                                                   //.AddInMemoryApiScopes(Config.ApiScopes)
                                                                   //.AddInMemoryClients(Config.Clients)

                                  .AddTestUsers(TestUsers.Users)
                                  .AddConfigurationStore(options =>
                                  {
                                      options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                                      sql => sql.MigrationsAssembly(migrationsAssembly));
                                  })
                                  .AddOperationalStore(options =>
                                  {
                                       options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                                       sql => sql.MigrationsAssembly(migrationsAssembly));
                                  });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
