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
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAll();
                var results = _mapper.Map<List<HotelDTO>>(hotels);
                return Ok(results);
            }

            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(GetHotels)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id, new List<string> {"Country"});

                var result = _mapper.Map<HotelDTO>(hotel);
                return Ok(result);
            }

            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(GetHotel)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"invalid post attempt{nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDto);
                //we are altering the database so we have to commit changes
                await _unitOfWork.Hotels.Insert(hotel);
                await _unitOfWork.save();

                return CreatedAtRoute("GetHotel", new {id = hotel.Id}, hotel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(CreateHotel)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDto)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"invalid Update attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"invalid Update attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted Data s not valid");
                }

                //(source,out object)
                _mapper.Map(hotelDto,hotel);
                _unitOfWork.Hotels.Update(hotel);
                await _unitOfWork.save();
                
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(UpdateHotel)}");
                return StatusCode(500, "internal server error");
            }
        }
        
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"invalid Delete attempt in {nameof(DeleteHotel)}");
                return BadRequest("invalid Data Provided");
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"invalid Delete attempt in {nameof(DeleteHotel)}");
                    return BadRequest("Submitted Data is not valid");
                }

                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.save();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something Went Wrong in {nameof(DeleteHotel)}");
                return StatusCode(500, "internal server error");
            }
        }
    }
}