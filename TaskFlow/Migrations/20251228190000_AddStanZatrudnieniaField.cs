using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddStanZatrudnieniaField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the new StanZatrudnienia column with default value 'Zatrudniony'
            migrationBuilder.AddColumn<string>(
                name: "StanZatrudnienia",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Zatrudniony");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StanZatrudnienia",
                table: "Pracownicy");
        }
    }
}
