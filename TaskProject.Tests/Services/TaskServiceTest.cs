using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskProject.Data;
using TaskProject.Data.Models;
using TaskProject.DTOs;
using TaskProject.Mapping;
using TaskProject.Services;
using Xunit;

namespace TaskProject.Tests.Services
{
    public class TaskServiceTest
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var oprions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(oprions);
        }

        private IMapper CreateMapper()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IMapper>();
        }

        private User CreateTestUser(int id, string name, string email, string role = "User")
        {
            return new User
            {
                Id = id,
                Name = name,
                Email = email,
                PasswordHash = "hash",
                Role = role
            };
        }

        [Fact]
        public async Task GetTasksAsync_ShoulReturnOnlyUserTask()
        {
            var context = CreateInMemoryDbContext();
            var mapper = CreateMapper();
            var taskService = new TaskService(context, mapper);

            var user1 = CreateTestUser(1, "User1", "u1@test.com");
            var user2 = CreateTestUser(2, "User2", "u2@test.com");
            context.Users.AddRange(user1, user2);

            var project = new Project { Id = 1, Name = "Test Project" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            context.Tasks.AddRange(
               new TaskItem { Id = 1, Title = "Task 1", UserId = 1, ProjectId = 1, Status = "Todo", Priority = "High" },
               new TaskItem { Id = 2, Title = "Task 2", UserId = 1, ProjectId = 1, Status = "Done", Priority = "InProgress" },
               new TaskItem { Id = 3, Title = "Task 3", UserId = 2, ProjectId = 1, Status = "InProgress", Priority = "Low" }
            );

            await context.SaveChangesAsync();

            var queryParams = new TaskQueryParams { Page = 1, PageSize = 10 };

            var result = await taskService.GetAllTasksAsync(userId: 1, queryParams);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.Items.All(t => t.User.Id == 1).Should().BeTrue();
        }

        [Fact]
        public async Task GetTaskAsync_WithStatusFilter_ShouldReturnFilteredTasks()
        {
            var context = CreateInMemoryDbContext();
            var mapper = CreateMapper();
            var taskService = new TaskService(context, mapper);

            var user = CreateTestUser(1, "User", "u@test.com");
            context.Users.Add(user);
            var project = new Project { Id = 1, Name = "Project" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            context.Tasks.AddRange(
                new TaskItem { Id = 1, Title = "Todo Task", UserId = 1, ProjectId = 1, Status = "Todo", Priority = "High"},
                new TaskItem { Id = 2, Title = "Done Task", UserId = 1, ProjectId = 1, Status = "Done", Priority = "High"}
            );
            await context.SaveChangesAsync();

            var queryParams = new TaskQueryParams
            {
                Page = 1,
                PageSize = 10,
                Status = "Todo"
            };

            var result = await taskService.GetAllTasksAsync(userId: 1, queryParams: queryParams);

            result.Items.Should().HaveCount(1);
            result.Items.First().Status.Should().Be("Todo");
        }

        [Fact]
        public async Task CreateTaskAsync_WithDifferentUserId_ShouldThrowException()
        {
            var context = CreateInMemoryDbContext();
            var mapper = CreateMapper();
            var taskService = new TaskService(context, mapper);

            var user = CreateTestUser(1, "User", "u@text.com");
            context.Users.Add(user);
            var project = new Project { Id = 1, Name = "Project" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var dto = new CreateTaskDto
            {
                Title = "Task",
                UserId = 999,
                ProjectId = 1,
                Status = "Todo",
                Priority = "High"
            };

            var act = () => taskService.CreateTaskAsync(dto, userId: 1);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Нельзя создать задачу от имени другого пользователя");
        }
    }
}
