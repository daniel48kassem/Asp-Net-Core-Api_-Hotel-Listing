using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration:IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
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
    }
}