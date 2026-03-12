using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JudgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionResultExtraData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Output",
                table: "SubmissionResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ErrorOutput",
                table: "SubmissionResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExitCode",
                table: "SubmissionResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExecuted",
                table: "SubmissionResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTle",
                table: "SubmissionResults",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorOutput",
                table: "SubmissionResults");

            migrationBuilder.DropColumn(
                name: "ExitCode",
                table: "SubmissionResults");

            migrationBuilder.DropColumn(
                name: "IsExecuted",
                table: "SubmissionResults");

            migrationBuilder.DropColumn(
                name: "IsTle",
                table: "SubmissionResults");

            migrationBuilder.AlterColumn<string>(
                name: "Output",
                table: "SubmissionResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
