using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAll();
                var results = _mapper.Map<List<CountryDTO>>(countries);
                return Ok(results);
            }

            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(GetCountries)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                //we are getting the country ,by giving the expression ,
                //and we get the hotels in this country by providing include list as second parameter
                var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> {"Hotels"});

                //here we are mapping to single countryDTO
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }

            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(GetCountry)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountrylDTO countrylDto)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"invalid Update attempt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(q => q.Id == id);
                if (country == null)
                {
                    _logger.LogError($"invalid Update attempt in {nameof(UpdateCountry)}");
                    return BadRequest("Submitted Data s not valid");
                }

                //(source,out object)
                _mapper.Map(countrylDto, country);
                _unitOfWork.Countries.Update(country);
                await _unitOfWork.save();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(UpdateCountry)}");
                return StatusCode(500, "internal server error");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"invalid Update attempt in {nameof(DeleteCountry)}");
                return BadRequest("invalid Data Provided");
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(q => q.Id == id);
                if (country == null)
                {
                    _logger.LogError($"invalid Delete attempt in {nameof(DeleteCountry)}");
                    return BadRequest("Submitted Data is not valid");
                }

                await _unitOfWork.Countries.Delete(id);
                await _unitOfWork.save();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something Went Wrong in {nameof(DeleteCountry)}");
                return StatusCode(500, "internal server error");
            }
        }
    }
}