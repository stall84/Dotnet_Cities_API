using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            // ModelState is a dictionary containing validation checks for your imposed validation from the models (PoIForCreationDto in this case)
            // When request comes in ModelState runs checks based on your rules set up and if one or more fails, ModelState isValid
            // property will be false

            // Following modelstate if statement is NOT NEEDED when using the API Controller Attribute which we are above.
            // Only including it for demo purposes
           
            
            // Custom validation checks can be added if they aren't available in the many attributes. 
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                // Use AddModelError method to ModelState library to add custom validation check/response
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name"
                    );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        [HttpPut("{id}")]

        public IActionResult UpdatePointOfInterest(int cityId, int id, 
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            // More validation of input
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                // Use AddModelError method to ModelState library to add custom validation check/response
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name"
                    );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Logic to find city of POI 
            var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            // Search for the matching point of interest coming in from the PUT request
            var pointOfInterestFromStore = city.PointsOfInterest
                    .FirstOrDefault(p => p.Id == id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            // Since this is a PUT request the standard says all resources should be updated, in our case both name and 
            // description

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();     // Return a 204 No Content response to indicate success updating
        }

        // Partially Update a resource via PATCH - JSON-Patch standard. In this case you'll see the input parameter is 
        // JsonPatchDocument & not one of our DTO models
        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterestFromStore = city.PointsOfInterest
                    .FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            // Since the patch is going to work on POIForUpdateDto (we specified above) you'll need to transform the POI from the
            // DataStore to POIForUpdateDto format (which transfers all the existing data to new variable we can then apply patch to)
            var pointOfInterestToPatch =
                    new PointOfInterestForUpdateDto()
                    {
                        Name = pointOfInterestFromStore.Name,
                        Description = pointOfInterestFromStore.Description
                    };
            // PATCH newly created POI instance along with ModelState to check our model for validation requirements
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            // If validation fails return 400 level response to client
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // ModelState is being checked in this case on the inputted model. Which in this case is not one of our DTO models
            // It's a JsonPatchDocument .. which means it could pass simple modelstate validation but in fact still be malformed
            // So we need to explicity check the DTO model after applying the patch document
            // First adding our custom rule we've made for name != description
            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name");
            }
            // Trigger validation on POIToPatch. If this resolves to false, modelstate will be invalid
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }
            // Remaining code works exactly the same at PUT update
            // Here you assign the new request body values to the POI object residing in DataStore memory in this demo case
            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            // Return succesful 204 response code
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities
                    .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest
                     .FirstOrDefault(d => d.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            // Very simple DELETE statement. Remove the route-specified POI from the DataStore
            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            return NoContent();
        }

    }
}
