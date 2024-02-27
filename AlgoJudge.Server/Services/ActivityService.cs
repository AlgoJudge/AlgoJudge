using AlgoJudge.Server.Database;
using AlgoJudge.Server.Database.Models;
using AlgoJudge.Server.Services.Models;
using AlgoJudge.Server.Utils;
using AutoMapper;

namespace AlgoJudge.Server.Services
{
    public class ActivityService(
        IPermissionService permissionService,
        IMapper mapper,
        ApplicationDbContext context,
        INotificationService notificationService
        ) : IActivityService
    {
        public async Task<ActivityInfo> CreateActivity(ActivityCreateModel model)
        {
            if (!permissionService.CanCreateActivity())
            {
                throw new AccessDeniedException();
            }
            Activity activity = mapper.Map<Activity>(model);
            context.Activities.Add(activity);
            await context.SaveChangesAsync();
            notificationService.NotifyNewActivity(activity);
            return mapper.Map<ActivityInfo>(activity);
        }

        public ICollection<ActivityInfo> GetActivityInfoList(Query query)
        {
            if (!permissionService.CanListActivityInfo())
            {
                throw new AccessDeniedException();
            }
            var activities = context.Activities.ToList();
            return mapper.Map<ICollection<ActivityInfo>>(activities);
        }
    }
}
