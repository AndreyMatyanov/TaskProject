using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskProject.Data;
using TaskProject.Data.Models;
using TaskProject.DTOs;

namespace TaskProject.Services
{
    public class ProjectService: IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetAllProjectsAsync()
        {
            var projects = await _context.Projects.ToListAsync();
            return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetProjectsByUserIdAsync()
        {
            var projects = await _context.Projects.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
        }

        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return project == null ? null: _mapper.Map<ProjectResponseDto>(project);
        }

        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto dto)
        {
            if (await _context.Projects.AnyAsync(p => p.Name == dto.Name))
                throw new InvalidOperationException("Проект с таким именем уже существует");

            var project = _mapper.Map<Project>(dto);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProjectResponseDto>(project);
        }

        public async Task<ProjectResponseDto?> UpdateProjectAsync(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return null;

            if (project.Name != dto.Name && await _context.Projects.AnyAsync(p => p.Name == dto.Name))
                throw new InvalidOperationException("Проект с таким именем уже существует");

            _mapper.Map(dto, project);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProjectResponseDto>(project);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
