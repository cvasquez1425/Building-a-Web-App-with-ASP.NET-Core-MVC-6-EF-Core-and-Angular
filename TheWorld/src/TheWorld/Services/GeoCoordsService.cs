using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheWorld.Services
{
    public class GeoCoordsService
    {
        private IConfigurationRoot _config;
        private ILogger<GeoCoordsService> _logger;

        public GeoCoordsService(ILogger<GeoCoordsService> logger, IConfigurationRoot config)  // adding configuration as a dependency to this class, and store it as class level _config.
        {
            _logger = logger;
            _config = config;
        }

                        //GeoCoordsResult new type of structure.
        public async Task<GeoCoordsResult> GetCoordsAsync(string name)
        {
            var result = new GeoCoordsResult()
            {
                Success = false,
                Message = "Failed to get coordinates"
            };

            //Service Bing Maps to convert a name of a place, or address of a place into set of longitudes and latitudes. www.bingmapsportal.com
            var apiKey = _config["keys:BingKey"];
            var encodedName = WebUtility.UrlEncode(name);
            // construct the actual URL that the Bing Documentation gave it to us. we will take this URL across the web to parse some data to get the longitude and latitude.
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedName}&key={apiKey}";

            // Actual calling to this URL. Get the Resource file from github.com/shawnwildermuth/BuildingWebASPNETCore

            var client = new HttpClient();

            var json = await client.GetStringAsync(url); // get the results of this query for the name.

            // Read out the results
            // Fragile, might need to change if the Bing API changes
            var results = JObject.Parse(json);                          // parse with Linq to Json.
            var resources = results["resourceSets"][0]["resources"];
            if (!resources.HasValues)
            {
                result.Message = $"Could not find '{name}' as a location";
            }
            else
            {
                var confidence = (string)resources[0]["confidence"];   // what confidence the data returned is for the location we specified. 
                if (confidence != "High")                               // we couldn't get a confident match for that name.
                {
                    result.Message = $"Could not find a confident match for '{name}' as a location";
                }
                else
                {
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    result.Latitude = (double)coords[0];
                    result.Longitude = (double)coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
            }

            return result;
        }
    }
}
