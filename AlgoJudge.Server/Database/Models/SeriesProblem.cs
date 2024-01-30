using System.ComponentModel.DataAnnotations.Schema;

namespace AlgoJudge.Server.Database.Models
{
    public class SeriesProblem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string ShortName { get; set; }
        public int ProblemId { get; set; }
        public required Problem Problem { get; set; }
        public ICollection<Series> Series { get; set; } = new List<Series>();
    }
}
