using JudgeAPI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubmissionResult>()
                .HasOne(sr => sr.Submission)
                .WithMany(s => s.Results)
                .HasForeignKey(sr => sr.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubmissionResult>()
                .HasOne(sr => sr.TestCase)
                .WithMany()
                .HasForeignKey(sr => sr.TestCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Unit> Units{ get; set; }
        public DbSet<Problem> Problems{ get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<SubmissionResult> SubmissionResults { get; set; }
    }
}
