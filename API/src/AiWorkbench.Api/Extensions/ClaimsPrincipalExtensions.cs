using System.Security.Claims;

namespace AiWorkbench.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? user.FindFirstValue("sub");
            return id is null ? throw new NullReferenceException() : Guid.Parse(id);
        }
    }
}
