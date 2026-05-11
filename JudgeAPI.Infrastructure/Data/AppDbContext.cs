using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            _ = builder.Entity<SubmissionResult>()
                .HasOne(static sr => sr.Submission)
                .WithMany(static s => s.Results)
                .HasForeignKey(static sr => sr.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.Entity<SubmissionResult>()
                .HasOne(static sr => sr.TestCase)
                .WithMany()
                .HasForeignKey(static sr => sr.TestCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public required DbSet<Unit> Units { get; set; }
        public required DbSet<Problem> Problems { get; set; }
        public required DbSet<TestCase> TestCases { get; set; }
        public required DbSet<Submission> Submissions { get; set; }
        public required DbSet<SubmissionResult> SubmissionResults { get; set; }
        public required DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    }

}
