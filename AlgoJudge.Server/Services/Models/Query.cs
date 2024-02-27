namespace AlgoJudge.Server.Services.Models
{
    public class Query
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string? OrderBy { get; set; }
    }
}
