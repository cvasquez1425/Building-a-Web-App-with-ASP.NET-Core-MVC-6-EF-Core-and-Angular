#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
#endregion

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
            // Read the config file here
            var builder = new ConfigurationBuilder()
              .SetBasePath(_env.ContentRootPath)  
              .AddJsonFile("config.json")       // config.json is a comfortable way for development for us to add these default settings. Use Env Variables that overrides those would be very helpful.
              .AddEnvironmentVariables();       // Adding other sources such as Env Variables by commingle different configuration sources. Configuration will be overriden in different formats.

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // in ASP.NET Core we are required to use dependency injection. MVC requires a number of services, a number of interfaces.
            // here register MVC services.
            services.AddMvc(config =>
            {
                if (_env.IsProduction())
                {
                    config.Filters.Add(new RequireHttpsAttribute());        // if attempt to go to an http, it is going to try to redirect you to HTTPS
                }
            })                   // AddJsonOptions special method to configure what happens for our Json results.
            .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });   // New Serializer for Json .net Core

            // just like we registered the IMailService, we need to register ConfigurationRootService.
            services.AddSingleton(_config);     // that _config is the ConfigurationRoot. We added as a Singleton.

            if (_env.IsEnvironment("Development") || _env.IsEnvironment("Testing"))
            {
                //register our own service. it means we are going to supply an Interface (i.e., IMailService) that it will fullfill by that class DebugMailService
                services.AddScoped<IMailService, DebugMailService>();
                
                // with AddSingleton, we are going to create one instance for the first time we need it, and then pass that instance in over and over and over again.
                //services.AddSingleton<IMailService, DebugMailService>();
            }
            else
            {
                // implement a real Mail Service.
            }

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath  = "/Auth/Login";  // we redirect people, send them here to get authenticated
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()          // Another piece of configuration. creae Events Property, a set of callbacks while the authentication is happening.
                {
                    OnRedirectToLogin = async ctx =>        // change the way the redirect on login works so that when we have an API it return a status code instead of redirection.
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && 
                           ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        await Task.Yield();
                    }
                };
            })
            .AddEntityFrameworkStores<WorldContext>(); // this is where the identities entities will be stored.            

            // Register by adding to the Configure Services the WorldContext class. Extension Method to wire up the Entity Framework, and out Context.
            services.AddDbContext<WorldContext>();

            // it is expensive to actually create the context class, I want to create it once per request cycle, and AddScoped is going to allow me to do that.
            services.AddScoped<IWorldRepository, WorldRepository>();

            // Bing Maps. We register as a type, so we should be able to add it as a new INJECTABLE PARAMETER into our StopController
            services.AddTransient<GeoCoordsService>();

            services.AddTransient<WorldContextSeedData>();  // here only supply the class name, Transient is going to create this every time we need it. we are going to inject it directly in the Configure method.

            //Add all the interfaces and services required for Logging.
            services.AddLogging();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. Configure the middlewares, and the order of
        // middlewares are very important because as the request comes in, it is going to hand it to each middleware in this order.

        //Summary
        //Remember these three major points about Middleware in ASP.NET Core 1.0:

        //Middleware components allow you complete control over the HTTP pipeline in your app.
        //Among other things, Middleware components can handle authorization, redirects, headers, and even cancel execution of the request.
        //The order in which Middleware components are registered in the Startup file is very important.

        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            WorldContextSeedData seeder, 
            ILoggerFactory factory,
            WorldContext context)
        {
            factory.AddDebug(LogLevel.Warning);

            // AutoMapper Initializer
            Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>().ReverseMap(); // To, From. ReverseMap create a map for both directions.
                config.CreateMap<StopViewModel, Stop> ().ReverseMap();
            });           

            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);             // Debug as a logger type, is actually in another package. Debug Window.
            }
            else
            {
                factory.AddDebug(LogLevel.Error);                   // write it out to the Debug Window.
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("<html><body><h3>Hello World!</h3></body></html>");
            //});

            // another piece of middleware to add some standard default files. this piece of middleware is called app.UseDefailtfiles, when looking into
            // wwwroot directory it is going to automatically look for an index.html, index.htm etc

            // Now we are going to be using the MVC Index.cshtml as the default page NOT the index.html; that is why we commented out app.UseDefaultFiles();
            //app.UseDefaultFiles(); // need to be first than UseStaticFiles

            // to serve static file index.html. Every feature of the web server is optional.
            // To serve static file you need the app class with staticfile method. This is adding the middleware to server files to the web browser.
            app.UseStaticFiles(); // Middleware. the yellow bulb is called Quick Launch.

            app.UseIdentity();      // turn on ASP.NET Identity


            //app.UseDeveloperExceptionPage(); // display those errors. another piece of midlleware.

            // Enable to listne MVC operations.
            // take an URL and map it to a route, here we define the route to the controller.
            app.UseMvc(config =>
            {
            config.MapRoute(
                name: "Default",
                template: "{controller=App}/{action=Index}/{id?}"); //,  // PATTERN, the third part is an optional ID with the question mark
                    //defaults: new { controller = "App", action = "Index" } // this will allow the root path mapped directly to that index method on the app controller.
                    //);
            });     // Middleware

            context.Database.Migrate();
            //let's call it with Wait so it becomes a synchronous operation. I can't make configure an Asynch call, has to be synchronous, that is why the little trick of using Wait instead.
            seeder.EnsureDeedData().Wait();           
        }

    }   // end of program.
}
