using System.Security.Claims;

namespace TaskProject.Helpers
{
    public class CurrentUserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out int userId))
                throw new UnauthorizedAccessException("Не удалось определить пользователя");
            return userId;
        }
    }
}
