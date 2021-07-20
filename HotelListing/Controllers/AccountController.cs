using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;
        // private readonly SignInManager<ApiUser> _signInManager;

        public AccountController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper,UserManager<ApiUser> userManager,IAuthManager authManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _authManager = authManager;
            // _signInManager = signInManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration Attempt For user{userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);
                user.UserName = userDTO.Email;
                
                //this will create the user ,hash the password,store the user in database
                var result = await _userManager.CreateAsync(user,userDTO.Password);
                
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code,error.Description);
                        
                    }
                    return BadRequest(ModelState);
                }
                
                await _userManager.AddToRolesAsync(user,userDTO.Roles);
                return Accepted();
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"something wrong in the {nameof(Register)}");
                return Problem($"something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            _logger.LogInformation($"Registration Attempt For user{userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                if (!await _authManager.ValidateUser(userDTO)) 
                {
                    return Unauthorized();
                }
                var key = Environment.GetEnvironmentVariable("KEY");
                
                //return a new object,contains the token
                return Accepted(new {Token =await _authManager.CreateToken()});
            }
            
            catch (Exception e)
            {
                _logger.LogError(e,$"something wrong in the {nameof(Login)}");
                return Problem($"something Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
            
        }
    }
}