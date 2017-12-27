using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace TheWorld
{
    public class Program
    {
        /// <summary>
        ///  this boiler plate code you won't need to change very often
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) //this where our app is starting up. this is the starting point of all your code.
        {
            var host = new WebHostBuilder()     // listen to our app.
                .UseKestrel()                   // name of the web server under ASP.NET Core
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()            // support for IIS like headers and windows authentication
                .UseStartup<Startup>()          // very important, use this class Startup to set up my web server, and instantiate it when you start the Web Hosl.
                .Build();

            host.Run();
        }
    }
}
