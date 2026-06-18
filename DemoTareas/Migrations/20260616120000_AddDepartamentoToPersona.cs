using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoTareas.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartamentoToPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Departamento",
                table: "Personas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departamento",
                table: "Personas");
        }
    }
}
