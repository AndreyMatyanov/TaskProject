using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto) => Ok(await _authService.RegisterAsync(dto));

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto) => Ok(await _authService.LoginAsync(dto));
    }
}
