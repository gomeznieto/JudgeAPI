using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JudgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserFixColumnUniversity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Universidad",
                table: "AspNetUsers",
                newName: "University");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "University",
                table: "AspNetUsers",
                newName: "Universidad");
        }
    }
}
