using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskProject.DTOs;
using TaskProject.Helpers;
using TaskProject.Services;

namespace TaskProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService) => _taskService = taskService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll([FromQuery] TaskQueryParams queryParams)
        {
            var userId = CurrentUserHelper.GetUserId(User);
            var result = await _taskService.GetAllTasksAsync(userId, queryParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, CurrentUserHelper.GetUserId(User));
            return task == null ? NotFound(new { message = "Задача не найдена" }) : Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> Create([FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateTaskAsync(dto, CurrentUserHelper.GetUserId(User));
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDto>> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _taskService.UpdateTaskAsync(id, dto, CurrentUserHelper.GetUserId(User));
            return task == null ? NotFound(new { message = "Задача не найдена" }) : Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _taskService.DeleteTaskAsync(id, CurrentUserHelper.GetUserId(User))
                ? NoContent()
                : NotFound(new { message = "Задача не найдена" });
    }
}
