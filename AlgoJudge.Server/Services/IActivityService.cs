using AlgoJudge.Server.Database.Models;
using AlgoJudge.Server.Services.Models;

namespace AlgoJudge.Server.Services
{
    public interface IActivityService
    {
        ICollection<ActivityInfo> GetActivityInfoList(Query query);
        Task<ActivityInfo> CreateActivity(ActivityCreateModel model);
    }
}
