﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointOfInterestForCreationDto
    {
        // Required Attribute sets up validation for the model. Error message can be added as shown
        [Required(ErrorMessage = "Name value must be provided")]

        // There are many dozens of validation attributes like MaxLength 
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
