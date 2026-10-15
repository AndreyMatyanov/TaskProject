using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Pages.Projects
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DeleteModel : PageModel
    {
        private readonly IProjectService _projectService;

        public DeleteModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public ProjectResponseDto Project { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            Project = project;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                TempData["SuccessMessage"] = "Проект удален успешно!";
                return RedirectToPage("Index");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении проекта: {ex.Message}";
                return RedirectToPage("Index");
            }
        }
    }
}
