using AlgoJudge.Server.Database.Models;
using AlgoJudge.Server.Services.Models;
using AutoMapper;

namespace AlgoJudge.Server.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Activity, ActivityInfo>();
            CreateMap<ActivityCreateModel, Activity>();
        }
    }
}
