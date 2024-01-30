using System.ComponentModel.DataAnnotations.Schema;

namespace AlgoJudge.Server.Database.Models
{
    public class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string ShortName { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<Series> Series { get; set; } = new List<Series>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    }
}
