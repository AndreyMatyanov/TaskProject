using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Pages.Projects
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CreateModel : PageModel
    {
        private readonly IProjectService _projectService;
        public CreateModel (IProjectService projectService)
        {
            _projectService = projectService;
        }

        [BindProperty]
        public CreateProjectDto Project { get; set; } = new();

        public string? ErrorMessage { get; set; }
        
        public void OnGet()
        {
            // Any initialization logic can go here
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                await _projectService.CreateProjectAsync(Project);
                TempData["SuccessMessage"] = "Проект создан успешно!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
