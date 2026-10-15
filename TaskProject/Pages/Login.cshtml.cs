using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TaskProject.DTOs;
using TaskProject.Services;

namespace TaskProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        public string? ErrorMessage { get; set; }
        public string? InfoMessage { get; set; }

        public void OnGet(string? info = null)
        {
            InfoMessage = info;
        }

        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDto
                {
                    Email = email,
                    Password = password
                };

                var result = await _authService.LoginAsync(loginDto);

                var claims = new List<Claim>
                { new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                  new(ClaimTypes.Email, result.Email),
                  new("JwtToken", result.Token)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(1)
                    });

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
