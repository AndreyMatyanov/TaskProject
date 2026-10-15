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
    public class EditModel : PageModel
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;

        public EditModel(ITaskService taskService, IProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        [BindProperty]
        public UpdateTaskDto Task { get; set; } = new();
        public int TaskId { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> Priorities { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var userId = CurrentUserHelper.GetUserId(User);
                var task = await _taskService.GetTaskByIdAsync(id, userId);
                if (task == null)
                {
                    return NotFound();
                }
                Task = new UpdateTaskDto
                {
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    ProjectId = task.Project.Id,
                    UserId = task.User.Id,
                };
                TaskId = task.Id;

                await LoadSelectListsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = CurrentUserHelper.GetUserId(User);

            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            try
            {
                TaskId = id;
                Task.UserId = userId;

                await _taskService.UpdateTaskAsync(id, Task, Task.UserId);
                TempData["SuccessMessage"] = "Задача успешно обновлена!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await LoadSelectListsAsync();
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
            new("Завершено", "Done")
        };

            Priorities = new List<SelectListItem>
        {
            new("Низкий", "Low"),
            new("Средний", "Medium"),
            new("Высокий", "High")
        };
        }
    }
}
