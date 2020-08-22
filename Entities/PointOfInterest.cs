using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        
        public int Id { get; set; }

        // Add validation
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // We want to create a property and foreign key to refer back to the City table 
        public City City { get; set; }
        // The following foreign key with the primary key of the principal table (City) .. This is not necessary to define, but rec'd
        public int CityId { get; set; }
    }
}
