using AlgoJudge.Server.Database.Models;

namespace AlgoJudge.Server.Services
{
    public interface IPermissionService
    {
        bool CanListActivityInfo();
        bool CanCreateActivity();
    }
}
