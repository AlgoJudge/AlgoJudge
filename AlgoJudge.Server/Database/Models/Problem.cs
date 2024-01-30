using System.ComponentModel.DataAnnotations.Schema;

namespace AlgoJudge.Server.Database.Models
{
    public class Problem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string ShortName { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public ICollection<SeriesProblem> SeriesProblem { get; set; } = new List<SeriesProblem>();
    }
}
