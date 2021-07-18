using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options: options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Syria",
                    ShortName = "SY"
                },
                new Country
                {
                    Id = 2,
                    Name = "England",
                    ShortName = "UK"
                },
                new Country
                {
                    Id = 3,
                    Name = "United States",
                    ShortName = "US"
                }
            );
        
            builder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Four Seasons",
                    CountryId = 2,
                    Address = "aa",
                    Rating = 4.5
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Hayat",
                    CountryId = 1,
                    Address = "aas",
                    Rating = 4.2
                }
            );
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
    }
}