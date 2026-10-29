using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.APIs.Errors;
using Store.APIs.Extensions;
using Store.Core.Dtos.Auth;
using Store.Core.Entities.Identity;
using Store.Core.Services.Contract;
using Store.Service.Services.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.APIs.Controllers
{
    public class AccountsController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(
            IUserService userService,
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IMapper mapper
            )
        {
            _userService = userService;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userService.LoginAsync(loginDto);
            if (user is null)
            {
                return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized));
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = await _userService.RegisterAsync(registerDto);
            if (user is null)
            {
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));
            }
            return Ok(user);
        }

        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }


        [HttpGet("Address")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUserAddress()
        {
            var user = await _userManager.FindByEmailWithAddressAsync(User);

            if (user is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(_mapper.Map<AddressDto>(user.Address));
        }



        [HttpPut("Address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var user = await _userManager.FindByEmailWithAddressAsync(User);

            if (user is null)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));


            if (user.Address is null)
                user.Address = _mapper.Map<Address>(addressDto);
            else
                _mapper.Map(addressDto, user.Address);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(_mapper.Map<AddressDto>(user.Address));
        }

    }
}
