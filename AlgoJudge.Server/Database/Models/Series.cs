using System.ComponentModel.DataAnnotations.Schema;

namespace AlgoJudge.Server.Database.Models
{
    public class Series
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string ShortName { get; set; }
        public required string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<SeriesProblem> SeriesProblems { get; set; } = new List<SeriesProblem>();
    }
}
