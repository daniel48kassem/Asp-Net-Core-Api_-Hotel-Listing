using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        // private readonly SignInManager<ApiUser> _signInManager;

        public AccountController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper,UserManager<ApiUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
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

        // [HttpPost]
        // [Route("login")]
        // public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        // {
        //     _logger.LogInformation($"Registration Attempt For user{userDTO.Email}");
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     
        //     try
        //     {
        //         var result = await _signInManager.PasswordSignInAsync(userDTO.Email,userDTO.Password,false,false);
        //         if (!result.Succeeded)
        //         {
        //             return Unauthorized(userDTO);
        //         }
        //         return Accepted();
        //     }
        //     
        //     catch (Exception e)
        //     {
        //         _logger.LogError($"something wrong in the {nameof(Login)}");
        //         return Problem($"something Went Wrong in the {nameof(Login)}", statusCode: 500);
        //     }
        //     
        // }
    }
}