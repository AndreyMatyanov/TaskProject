using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService) => _projectService = projectService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
            => Ok(await _projectService.GetAllProjectsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            return project == null ? NotFound(new { message = "Проект не найден" }) : Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectResponseDto>> Create([FromBody] CreateProjectDto dto)
        {
            var createdProject = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, createdProject);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            var project = await _projectService.UpdateProjectAsync(id, dto);
            return project == null ? NotFound(new { message = "Проект не найден" }) : Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _projectService.DeleteProjectAsync(id) ? NoContent() : NotFound(new { message = "Проект не найден" });
    }
}
