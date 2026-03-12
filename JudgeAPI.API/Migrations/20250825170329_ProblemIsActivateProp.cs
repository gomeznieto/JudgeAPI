using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JudgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class ProblemIsActivateProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActivate",
                table: "Problems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActivate",
                table: "Problems");
        }
    }
}
