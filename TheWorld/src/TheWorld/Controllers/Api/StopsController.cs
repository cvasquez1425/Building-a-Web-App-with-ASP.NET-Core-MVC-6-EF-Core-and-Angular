#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWorld.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using TheWorld.ViewModels;
using TheWorld.Services;
using Microsoft.AspNetCore.Authorization;
#endregion

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TheWorld.Controllers.Api
{
    /// <summary>
    /// this Authorize attribute says that every call to this Controller we're expecting someone to be logged in or to have authentication information.
    /// </summary>
    //[Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private GeoCoordsService _coordsService;
        private ILogger<StopsController> _logger;
        private IWorldRepository _repository;

        public StopsController(IWorldRepository repository, 
            ILogger<StopsController> logger,
            GeoCoordsService coordsService)             // added as a new Injectable parm into our Controller.
        {
            _repository    = repository;
            _logger        = logger;
            _coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                //var trip = _repository.GetTripByName(tripName);
                var trip = _repository.GetUserTripByName(tripName, User.Identity.Name);
                // use AutoMapper to map IEnumerable of StopViewModel. Essentially, we wante to use the StopViewModel instead of the Stops Entity.                
                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));   // we will return StopViewModel. To From, we pass the trip.Stops.
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to get stops: {0}", ex);
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                // if the VM is valid
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    //Lookup the GeoCodes. GeoLocation Services
                    var result = await _coordsService.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude    = result.Latitude;
                        newStop.Longitude   = result.Longitude;

                        //Save to the database
                        _repository.AddStop(tripName, newStop, User.Identity.Name);

                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"/api/trips/{tripName}/stops/{newStop.Name}", //Created is the result of a Post when successfully save a new object. 201 Status code.
                                Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }                
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to save new Stop: {0}", ex);
            }

            return BadRequest("Failed to save new stop");
        }
    }
}
