using TaskProject.DTOs;

namespace TaskProject.Services
{
    public interface ITaskService
    {
        Task<PagedResponseDto<TaskResponseDto>> GetAllTasksAsync(int userId, TaskQueryParams queryParams);
        Task<TaskResponseDto?> GetTaskByIdAsync(int id, int userId);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId);
        Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
    }
}
