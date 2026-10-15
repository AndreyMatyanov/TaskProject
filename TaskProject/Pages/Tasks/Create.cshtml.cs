using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskProject.DTOs;
using TaskProject.Helpers;
using TaskProject.Services;

namespace TaskProject.Pages.Tasks
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CreateModel : PageModel
    {
        private ITaskService _taskService;
        private IProjectService _projectService;

        public CreateModel(ITaskService taskService, IProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        [BindProperty]
        public CreateTaskDto Task { get; set; } = new();

        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> Priority { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadSelectListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSelectListsAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var userId = CurrentUserHelper.GetUserId(User);
                Task.UserId = userId;

                await _taskService.CreateTaskAsync(Task, userId);
                TempData["SuccessMessage"] = "Задача создана успешно!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        private async Task LoadSelectListsAsync()
        {
            var userId = CurrentUserHelper.GetUserId(User);

            var projects = await _projectService.GetAllProjectsAsync();
            Projects = projects.Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList();

            Statuses = new List<SelectListItem>
            {
                new("К выполнению", "Todo"),
                new("В работе", "InProgress"),
                new("Выполнено", "Done")
            };

            Priority = new List<SelectListItem>
            {
                new("Низкий", "Low"),
                new("Средний", "Medium"),
                new("Высокий", "High")
            };
        }
    }
}
