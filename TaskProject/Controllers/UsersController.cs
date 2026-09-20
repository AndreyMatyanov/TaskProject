using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) => _userService = userService;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll() => Ok(await _userService.GetAllUsersAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound(new { message = "Пользователь не найден" }) : Ok(user);
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllForAdmin()
            => Ok(await _userService.GetAllUsersForAdminAsync());

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
            => CreatedAtAction(nameof(GetById), new { id = (await _userService.CreateUserAsync(dto)).Id }, await _userService.CreateUserAsync(dto));

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return user == null ? NotFound(new { message = "Пользователь не найден" }) : Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
            => await _userService.DeleteUserAsync(id) ? NoContent() : NotFound(new { message = "Пользователь не найден" });
    }
}
