using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(q => q.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(q => q.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest
                    .FirstOrDefault(p => p.Id == id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(pointOfInterest);
        }

        [HttpPost]
        // 2 parameters, city ID and then the request body which will deserialize to a point of interest (using the POIFC model)
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(q => q.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            // For this demo portion we need to add logic to increase the POI Id, first getting the current count of ID's
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                                c => c.PointsOfInterest).Max(d => d.Id);

            // Then adding a new entry incrementing the Id by 1
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            // Push that new entry onto the POI of the city
            city.PointsOfInterest.Add(finalPointOfInterest);

            // We're returning a response that aliases the route URI from above 'GetPointOfInterest', then creating a new
            // anonymous type to respond with the newly created finalPOI Id. Then the newly created PoI
            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId = cityId, id = finalPointOfInterest.Id },
                finalPointOfInterest
                );

        }
    }
}
