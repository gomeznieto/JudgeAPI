using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JudgeAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActivate",
                table: "Units",
                newName: "IsActivate");

            migrationBuilder.RenameColumn(
                name: "isMandatory",
                table: "Problems",
                newName: "IsMandatory");

            migrationBuilder.RenameColumn(
                name: "isActivate",
                table: "Problems",
                newName: "IsActivate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActivate",
                table: "Units",
                newName: "isActivate");

            migrationBuilder.RenameColumn(
                name: "IsMandatory",
                table: "Problems",
                newName: "isMandatory");

            migrationBuilder.RenameColumn(
                name: "IsActivate",
                table: "Problems",
                newName: "isActivate");
        }
    }
}
