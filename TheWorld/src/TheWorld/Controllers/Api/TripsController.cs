#region Includes
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;
#endregion

namespace TheWorld.Controllers.Api
{
    [Route("api/trips")]                // centralize the api/trips
    //[Authorize]
    public class TripsController : Controller
    {
        private ILogger<TripsController> _logger;
        private IWorldRepository _repository;

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]      // making a request to the server for an URI that matches that, this method will be called. Using PostMan tool.
        public IActionResult Get()
        {
            try
            {
                //var results = _repository.GetAllTrips();
                var results = _repository.GetTripsByUsername(this.User.Identity.Name);   // this.User is the object that represents the claims for the user, but also includes an identity object.

                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));  // Map TripViewModel our results. We're only seeing what is in the TripViewModel.
            }
            catch (Exception ex)
            {
                // TODO Logging
                _logger.LogError($"Failed to get All Trips: {ex}");

                return BadRequest("Error occurred");
            }
            
            //if (true) return BadRequest("Bad things happened");
            //return Ok(_repository.GetAllTrips());
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]TripViewModel theTrip)       // FromBody says model bind the data that is coming in with the POST to this object
        {
            if (ModelState.IsValid)
            {
                // Save to the Database
                //var newTrip = new Trip()      this is not a good approach, it is hard to copy this over and over, also handle field names are different here.
                //{                             that is why we use the AutoMapper open source project. Automapper is an NuGet Package.
                //    Name = theTrip.Name,
                //    DateCreated = theTrip.Created
                //};

                // better approach with AutoMapper
                var newTrip = Mapper.Map<Trip>(theTrip);  // we want a Trip object, and what we pass is the theTrip. 
                                                          //  Maps needs to be created between these two types(i.e., Trip & TripViewModel) from Trip to TripViewModel essentially.
                newTrip.UserName = User.Identity.Name;

                _repository.AddTrip(newTrip);

                if (await _repository.SaveChangesAsync())
                {
                    //return Ok(true);  201 Created Status code
                    return Created($"api/trips/{theTrip.Name}", Mapper.Map<TripViewModel>(newTrip));  // the opposite of what we did above.
                }
            }

            //return BadRequest("Bad Data");
            //return BadRequest(ModelState);  // if you are the only one consuming this API.
            return BadRequest("Failed to save the trip");
        }
    } // end of program.
}
