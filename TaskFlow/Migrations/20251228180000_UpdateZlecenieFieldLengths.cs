using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateZlecenieFieldLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update NULL values before changing column constraints
            migrationBuilder.Sql("UPDATE Zlecenia SET NrZlecenia = 'Brak' WHERE NrZlecenia IS NULL");
            migrationBuilder.Sql("UPDATE Zlecenia SET Status = 'Aktywne' WHERE Status IS NULL");
            
            migrationBuilder.AlterColumn<string>(
                name: "NrZlecenia",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Opis",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 110,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Aktywne",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Informacje",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 110,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NrZlecenia",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Opis",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 110);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Informacje",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 110,
                oldNullable: true);
        }
    }
}
