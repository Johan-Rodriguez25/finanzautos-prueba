using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;
using UserMicroservice.Modules.Users.Infrastructure.Dtos.request;
using UserMicroservice.Modules.Users.Infrastructure.Dtos.response;
using UserMicroservice.Modules.Users.Infrastructure.Extensions;
using UserMicroservice.Modules.Users.Infrastructure.Services;

namespace UserMicroservice.Modules.Users.Infrastructure
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public UserController(
            ILogger<UserController> logger,
            UserService userService,
            TokenService tokenService)
        {
            _logger = logger;
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequestDto dto)
        {
            _logger.LogInformation("Received registration request for email: {Email}", dto.Email);
            try
            {
                var user = new User(
                    UserId.New(),
                    UserName.FromString(dto.Name),
                    UserEmail.FromString(dto.Email),
                    PasswordHash.FromString(dto.Password),
                    CreatedAt.Now()
                );

                await _userService.Register(user);
                return Created($"/api/User/{user.Id.Value}", null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing registration request for email: {Email}", dto.Email);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            _logger.LogInformation("Received Login request for email: {Email}", dto.Email);
            try
            {
                var user = await _userService.Login(dto.Email, dto.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                var token = _tokenService.GenerateJwtToken(user);
                var responseDto = new LoginResponseDto
                {
                    Token = token
                };

                return Ok(responseDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing Login request for email: {Email}", dto.Email);
                return BadRequest(e.Message);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("Received Logout request");
            try
            {
                var token = HttpContext.GetJwtTokenFromHeader();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("No authorization token provided");
                }

                var success = _tokenService.RevokeToken(token);

                if (!success)
                {
                    return BadRequest("Logout failed. Invalid token.");
                }

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing Logout request");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            _logger.LogInformation("Received GetCurrentUser request");
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token claims");
                    return Unauthorized("Invalid authentication token");
                }

                var user = await _userService.GetOneUserById(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var responseDto = new UserResponseDto
                {
                    Id = user.Id.Value,
                    Name = user.Name.Value,
                    Email = user.Email.Value,
                    CreatedAt = user.CreatedAt.Value
                };

                return Ok(responseDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing GetCurrentUser request");
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] UserRequestDto dto)
        {
            _logger.LogInformation("Received UpdateUser request");
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token claims");
                    return Unauthorized("Invalid authentication token");
                }

                var existingUser = await _userService.GetOneUserById(userId);
                if (existingUser == null)
                {
                    return NotFound("User not found");
                }

                UserName? name = !string.IsNullOrEmpty(dto.Name) ?
                    UserName.FromString(dto.Name) :
                    null;
                UserEmail? email = !string.IsNullOrEmpty(dto.Email) ?
                    UserEmail.FromString(dto.Email) :
                    null;
                PasswordHash? password = !string.IsNullOrEmpty(dto.Password) ?
                    PasswordHash.FromString(dto.Password) :
                    null;

                existingUser.UpdateInfo(
                    name,
                    email,
                    password
                );

                await _userService.EditUser(existingUser);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing UpdateUser request");
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            _logger.LogInformation("Received ValidateToken request");
            try
            {
                var token = HttpContext.GetJwtTokenFromHeader();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("No authorization token provided");
                }

                var isValid = await _tokenService.ValidateToken(token);
                return Ok(new { isValid });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing ValidateToken request");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            _logger.LogInformation("Received RefreshToken request");
            try
            {
                var token = HttpContext.GetJwtTokenFromHeader();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("No authorization token provided");
                }

                var (newToken, success, errorMessage) = _tokenService.RefreshToken(token);

                if (!success)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new TokenResponsetDto { Token = newToken });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing RefreshToken request");
                return BadRequest(e.Message);
            }
        }
    }
}
