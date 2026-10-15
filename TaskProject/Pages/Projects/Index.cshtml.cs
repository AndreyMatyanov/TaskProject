using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Pages.Projects
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly IProjectService _projectService;
        public IndexModel(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public List<ProjectResponseDto> Projects { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            SuccessMessage = TempData["SuccessMessage"] as string;
            ErrorMessage = TempData["ErrorMessage"] as string;
            var result = await _projectService.GetAllProjectsAsync();
            Projects = result.ToList();
        }
    }
}
