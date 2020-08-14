using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        
        // Reformated using IActionResult instead of JsonResult to be able to provide detailed status code in
        // response to client (Ok/NotFound)
        public IActionResult GetCity(int id)
        {
            // find the city from our datastore class
            var cityReturn = CitiesDataStore.Current.Cities
                .FirstOrDefault(q => q.Id == id);
            if (cityReturn == null)
            {
                return NotFound();
            }
            return Ok(cityReturn);
        }

    }
}
