using AlgoJudge.Server.Database;
using AlgoJudge.Server.Database.Models;
using System.Security.Claims;

namespace AlgoJudge.Server.Services
{
    public class CurrentUserService(
        ApplicationDbContext context,
        IHttpContextAccessor contextAccessor
        ) : ICurrentUserService
    {
        private User? user;
        private bool isInitialized = false;

        public User? Get()
        {
            if (!isInitialized)
            {
                try
                {
                    string id = contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    user = context.Users.First(x => x.Id == id);
                } catch { }
                isInitialized = true;
            }
            return user;
        }
    }
}
