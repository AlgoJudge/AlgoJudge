namespace AlgoJudge.Server.Services.Models
{
    public class ActivityCreateModel
    {
        public required string ShortName { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
