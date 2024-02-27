using AlgoJudge.Server.Services;
using AlgoJudge.Server.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgoJudge.Server.Controllers
{
    [ApiController]
    [Route("activity")]
    [Authorize]
    public class ActivityController(IActivityService activityService) : ControllerBase
    {
        [HttpGet("list")]
        public ICollection<ActivityInfo> List([FromQuery] Query query)
        {
            return activityService.GetActivityInfoList(query);
        }

        [HttpPost("create")]
        public async Task<ActivityInfo> Create(ActivityCreateModel model)
        {
            return await activityService.CreateActivity(model);
        }
    }
}
