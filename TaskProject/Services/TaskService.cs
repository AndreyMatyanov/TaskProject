using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskProject.Data;
using TaskProject.Data.Models;
using TaskProject.DTOs;

namespace TaskProject.Services
{
    public class TaskService: ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResponseDto<TaskResponseDto>> GetAllTasksAsync(int userId, TaskQueryParams queryParams)
        {
            IQueryable<TaskItem> query = _context.Tasks
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Project)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                query = query.Where(t => t.Status == queryParams.Status);
            }
            if (!string.IsNullOrEmpty(queryParams.Priority))
            {
                query = query.Where(t => t.Priority == queryParams.Priority);
            }
            query = queryParams.SortBy?.ToLower() switch
            {
                "title" => queryParams.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),

                "priority" => queryParams.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority),

                _ => queryParams.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
            };

            var totalCount = await query.CountAsync();

            var tasks = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PagedResponseDto<TaskResponseDto>
            {
                Items = _mapper.Map<List<TaskResponseDto>>(tasks),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            return task == null ? null : _mapper.Map<TaskResponseDto?>(task);
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId)
        {
            if (dto.UserId != userId)
                throw new InvalidOperationException("Нельзя создать задачу от имени другого пользователя");
            
            if (!await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId))
                throw new InvalidOperationException($"Проект с Id {dto.ProjectId} не существует");

            var task = _mapper.Map<TaskItem>(dto);
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return (await GetTaskByIdAsync(task.Id, userId))!;
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null)
                return null;
            if (dto.UserId != userId)
                throw new InvalidOperationException("Нельзя изменить задачу от имени другого пользователя");
            if (!await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId))
                throw new InvalidOperationException($"Проект с Id {dto.ProjectId} не существует");
            _mapper.Map(dto, task);
            await _context.SaveChangesAsync();
            return await GetTaskByIdAsync(id, userId);
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
