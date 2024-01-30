using System.ComponentModel.DataAnnotations.Schema;

namespace AlgoJudge.Server.Database.Models
{
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string MimeType { get; set; }
        public required byte[] Content { get; set; }
    }
}
