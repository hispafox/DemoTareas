using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoTareas.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonaToTarea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonaId",
                table: "Tareas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_PersonaId",
                table: "Tareas",
                column: "PersonaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_Personas_PersonaId",
                table: "Tareas",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_Personas_PersonaId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_PersonaId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "PersonaId",
                table: "Tareas");
        }
    }
}
