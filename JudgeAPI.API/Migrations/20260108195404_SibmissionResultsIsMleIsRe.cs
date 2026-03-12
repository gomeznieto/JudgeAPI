using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JudgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class SibmissionResultsIsMleIsRe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMle",
                table: "SubmissionResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRe",
                table: "SubmissionResults",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMle",
                table: "SubmissionResults");

            migrationBuilder.DropColumn(
                name: "IsRe",
                table: "SubmissionResults");
        }
    }
}
