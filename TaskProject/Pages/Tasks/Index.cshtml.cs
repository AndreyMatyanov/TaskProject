using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskProject.DTOs;
using TaskProject.Helpers;
using TaskProject.Services;

namespace TaskProject.Pages.Tasks
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly ITaskService _taskService;
        public IndexModel(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public List<TaskResponseDto> Tasks { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            SuccessMessage = TempData["SuccessMessage"] as string;
            ErrorMessage = TempData["ErrorMessage"] as string;

            var userId = CurrentUserHelper.GetUserId(User);
            var result = await _taskService.GetAllTasksAsync(userId, new TaskQueryParams
            {
                Page = 1,
                PageSize = 100
            });
            Tasks = result.Items.ToList();
        }
    }
}
