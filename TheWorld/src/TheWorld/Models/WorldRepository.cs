using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
///  the idea behind the repository is an interface that could implement in test scenarios fairly easily.
///  Becomes a unit of work for queries to the database and really wraps that context class.
///  
/// in order to make this an interface so that we could mock it up later, I am going to use Refactoring by going to the class name, and hitting control + period, to extract an interface.
/// </summary>
namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context,
            ILogger<WorldRepository> logger)            // we decided to add Logging to the repository ILogger<WorldRepository> specialized logger with the class name
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, Stop newStop, string username)
        {
            //var trip = GetTripByName(tripName);
            var trip = GetUserTripByName(tripName, username);

            if (trip != null)   // if the trip exist, add the stops.
            {
                trip.Stops.Add(newStop);        // here the foreign key is being set
                _context.Stops.Add(newStop);    // and here it is actually being added as new object. We need both of these to happen for our Stop in this version of EF Core.
            }
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);     // all is doing here is pushing into the DbContext as a new object, it is not saving to the database yet.
        }

        // a new method to get all the trips. Return a collection of Trips.
        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting All the Trips fro the Database");

            // use the context to generate that query.
            return _context.Trips.ToList();
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Include(t => t.Stops)                      // Eager Load that collection. Allow to add collection of stops to the Trip.
                .Where(t => t.Name == tripName)
                .FirstOrDefault();
        }

        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            return _context
                .Trips
                .Include(t => t.Stops)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public Trip GetUserTripByName(string tripName, string username)
        {
            return _context.Trips
                .Include(t => t.Stops)                      // Eager Load that collection. Allow to add collection of stops to the Trip.
                .Where(t => t.Name == tripName && t.UserName == username)
                .FirstOrDefault();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0; // SaveChanges and SaveChangesAsync both return an integer that represent the number of rows affected.
        }
    }
}
