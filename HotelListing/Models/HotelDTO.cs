using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models
{
    public class CreateHotelDTO
    {
        [Required] 
        [StringLength(maximumLength:150,ErrorMessage = "Long Name is too long")]
        public string Name { get; set; }
        [Required] 
        [StringLength(maximumLength:200,ErrorMessage = "Short Country Name is too long")]
        public string Address { get; set; }
        
        [Required]
        [Range(1,5)]
        public double Rating { get; set; }
        [Required]
        public int CountryId { get; set; }
    }

    public class HotelDTO:CreateCountryDTO
    {
        public int Id { get; set; }
        public CountryDTO CountryDTO { get; set; }
    }
}