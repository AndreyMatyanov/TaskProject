using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaskProject.Data;
using TaskProject.DTOs;
using TaskProject.Services;
using Xunit;

namespace TaskProject.Tests.Services
{
    public class AuthServiceTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private IConfiguration CreateTestConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string?>
        {
            {"JwtSettings:SecretKey", "VerySuperSecretKeyThatIsAtLeast32CharactersLong123!@#"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpiresInMinutes", "60"}
        };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private ILogger<AuthService> CreateLogger()
        {
            return new Mock<ILogger<AuthService>>().Object;
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange (Подготовка)
            var context = CreateInMemoryDbContext();
            var config = CreateTestConfiguration();
            var logger = CreateLogger();
            var authService = new AuthService(context, config, logger);

            var dto = new RegisterDto
            {
                Name = "Тестовый пользователь",
                Email = "test@example.com",
                Password = "Password123",
                Role = "User"
            };

            var result = await authService.RegisterAsync(dto);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNull();
            result.Email.Should().Be("test@example.com");
            result.UserId.Should().BeGreaterThan(0);

            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            userInDb.Should().NotBeNull();
            userInDb!.Name.Should().Be("Тестовый пользователь");
            userInDb.PasswordHash.Should().NotBe("Password123");

        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldThrowException()
        {
            var context = CreateInMemoryDbContext();
            var config = CreateTestConfiguration();
            var logger = CreateLogger();
            var authService = new AuthService(context, config, logger);

            var dto = new RegisterDto
            {
                Name = "Пользователь 1",
                Email = "existing@example.com",
                Password = "Password123"
            };

            await authService.RegisterAsync(dto);

            var duplicateDto = new RegisterDto
            {
                Name = "Пользователь 2",
                Email = "existing@example.com",
                Password = "Password123"
            };

            var act = () => authService.RegisterAsync(duplicateDto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Пользователь с таким email уже существует");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            var context = CreateInMemoryDbContext();
            var config = CreateTestConfiguration();
            var logger = CreateLogger();
            var authService = new AuthService(context, config, logger);

            await authService.RegisterAsync(new RegisterDto
            {
                Name = "Тест",
                Email = "login@example.com",
                Password = "Password123"
            });

            var loginDto = new LoginDto
            {
                Email = "login@example.com",
                Password = "Password123"
            };

            var result = await authService.LoginAsync(loginDto);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("login@example.com");
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrowException()
        {
            var context = CreateInMemoryDbContext();
            var config = CreateTestConfiguration();
            var logger = CreateLogger();
            var authService = new AuthService(context, config, logger);

            await authService.RegisterAsync(new RegisterDto
            {
                Name = "Тест",
                Email = "wrong@example.com",
                Password = "Password123"
            });

            var loginDto = new LoginDto
            {
                Email = "wrong@example.com",
                Password = "WrongPassword"
            };

            var act = () => authService.LoginAsync(loginDto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Неверный логин и пароль");
        }
    }
}
