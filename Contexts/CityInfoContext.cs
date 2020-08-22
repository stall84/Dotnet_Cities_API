using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Contexts
{
    public class CityInfoContext : DbContext
    {
        // DbSet property does the magic of converting requests to SQL database queries, etc
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }
        // Constructor below
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionString")
        //}
    }
}
