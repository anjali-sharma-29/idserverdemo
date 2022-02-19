// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
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
            const string connectionString = @"Data Source=.\sqlexpress;initial catalog=IdentityServer4;integrated security=True;MultipleActiveResultSets=True;";
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
            InitializeDatabase(app);
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

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService < IServiceScopeFactory >().CreateScope())
{
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService< ConfigurationDbContext > ();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
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
            }
        }


    }
}
