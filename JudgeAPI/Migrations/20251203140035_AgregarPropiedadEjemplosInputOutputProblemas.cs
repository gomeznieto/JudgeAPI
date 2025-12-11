using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JudgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPropiedadEjemplosInputOutputProblemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExampleInput",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExampleOutput",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleInput",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "ExampleOutput",
                table: "Problems");
        }
    }
}
