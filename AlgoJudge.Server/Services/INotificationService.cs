using AlgoJudge.Server.Database.Models;

namespace AlgoJudge.Server.Services
{
    public interface INotificationService
    {
        void NotifyNewActivity(Activity activity);
    }
}
