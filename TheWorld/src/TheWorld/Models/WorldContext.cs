using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TheWorld.Models
{
    public class WorldContext : IdentityDbContext<WorldUser>        // This represents a context to the actual database. The Gateway to interact with the database.
    {                                                               // WorldUser is the generic argument that takes the entity to store user information.
        private IConfigurationRoot _config;

        public WorldContext(IConfigurationRoot config, DbContextOptions options)    // Constructor Injection
            : base(options)     //because the base class for DbContext accepts something called DbContext options
        {
            _config = config; // store at the class level so I can use.
        }
        // it gives us a class that we can use LINQ queries against them. These Queryable Interfaces.
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_config["ConnectionStrings:WorldContextConnection"]);
        }
    }
}
