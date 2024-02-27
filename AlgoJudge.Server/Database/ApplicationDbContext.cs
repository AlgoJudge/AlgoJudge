using AlgoJudge.Server.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = AlgoJudge.Server.Database.Models.File;

namespace AlgoJudge.Server.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<File> Files { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<SeriesProblem> SeriesProblems { get; set; }
        public DbSet<Series> Series {  get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<File>().ToTable("Files");
            builder.Entity<Problem>().ToTable("Problems");
            builder.Entity<SeriesProblem>().ToTable("SeriesProblems");
            builder.Entity<Series>().ToTable("Series");
            builder.Entity<Activity>().ToTable("Activities");
            builder.Entity<Submission>().ToTable("Submissions");
            builder.Entity<Result>().ToTable("Results");
        }
    }
}
