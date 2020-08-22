using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class City
    {
        // Explicitly using 'Key' and 'DatabaseGenerated' attributes to specify Id field as primary key (it would anyway b/c it's named Id)
        // And attribute DBGenerated creates a new value for Id purposes whenever a row is added

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Add Validation
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
                = new List<PointOfInterest>();
    }
}
