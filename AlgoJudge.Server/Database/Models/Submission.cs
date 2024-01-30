using System.ComponentModel.DataAnnotations.Schema;

namespace AlgoJudge.Server.Database.Models
{
    public class Submission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<File> Files { get; set; } = new List<File>();
        public required string UserId { get; set; }
        public required User User { get; set; }
        public int SeriesProblemId { get; set; }
        public required SeriesProblem SeriesProblem { get; set; }
    }
}
