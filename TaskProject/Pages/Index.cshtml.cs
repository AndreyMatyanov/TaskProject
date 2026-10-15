using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskProject.DTOs;
using TaskProject.Helpers;
using TaskProject.Services;

namespace TaskProject.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public IndexModel(ITaskService taskService, IUserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        public string UserName { get; set; } = "";
        public int TotalTasks { get; set; }
        public int TodoTask { get; set; }
        public int InProgressTasks { get; set; }
        public int DoneTasks { get; set; }
        public List<TaskResponseDto> Tasks { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = CurrentUserHelper.GetUserId(User);
            
            var user = await _userService.GetUserByIdAsync(userId);
            UserName = user?.Name ?? "Пользователь";

            var result = await _taskService.GetAllTasksAsync(userId, new TaskQueryParams
            {
                Page = 1,
                PageSize = 100
            });

            Tasks = result.Items.ToList();
            TotalTasks = Tasks.Count;
            TodoTask = Tasks.Count(t => t.Status == "ToDo");
            InProgressTasks = Tasks.Count(t => t.Status == "InProgress");
            DoneTasks = Tasks.Count(t => t.Status == "Done");
            InProgressTasks = Tasks.Count(t => t.Status == "InProgress");
            DoneTasks = Tasks.Count(t => t.Status == "Done");
        }
    }
}
