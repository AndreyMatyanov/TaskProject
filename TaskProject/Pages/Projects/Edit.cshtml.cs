using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskProject.DTOs;
using TaskProject.Helpers;
using TaskProject.Services;

namespace TaskProject.Pages.Projects
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EditModel : PageModel
    {
        private readonly IProjectService _projectService;

        public EditModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [BindProperty]
        public UpdateProjectDto Project { get; set; } = new();
        public int ProjectId { get; set; }
        public string? ErrorMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    return NotFound();
                }
                Project = new UpdateProjectDto
                {
                    Name = project.Name,
                    Description = project.Description
                };
                ProjectId = id;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error retrieving project: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                ProjectId = id;
                await _projectService.UpdateProjectAsync(id, Project);
                TempData["SuccessMessage"] = "Проект обновлен успешно!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating project: {ex.Message}";
                return Page();
            }
        }
    }
}
