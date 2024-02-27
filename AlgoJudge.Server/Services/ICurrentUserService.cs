using AlgoJudge.Server.Database.Models;

namespace AlgoJudge.Server.Services
{
    public interface ICurrentUserService
    {
        User? Get();
    }
}
