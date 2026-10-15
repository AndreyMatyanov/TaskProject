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
    public class DeleteModel : PageModel
    {
        private readonly ITaskService _taskService;

        public DeleteModel(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public TaskResponseDto Task { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = CurrentUserHelper.GetUserId(User);
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            if (task == null)
            {
                return NotFound();
            }
            Task = task;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = CurrentUserHelper.GetUserId(User);

            try
            {
                    await _taskService.DeleteTaskAsync(id, userId);
                    TempData["SuccessMessage"] = "Задача удалена успешно!";
                    return RedirectToPage("Index");
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении задачи: {ex.Message}";
                return RedirectToPage("Index");
            }

        }
    }
}
