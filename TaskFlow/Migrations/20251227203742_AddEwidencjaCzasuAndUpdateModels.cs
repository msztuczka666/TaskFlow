using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddEwidencjaCzasuAndUpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nazwa",
                table: "Zlecenia");

            migrationBuilder.RenameColumn(
                name: "FirmaIdentyfikatorowa",
                table: "Pracownicy",
                newName: "FirmaId");

            migrationBuilder.RenameColumn(
                name: "FirmaGlowna",
                table: "Pracownicy",
                newName: "Firma");

            migrationBuilder.AlterColumn<string>(
                name: "Opis",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Informacje",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NrZlecenia",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Informacje",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LiczbaDniNaZeszycie",
                table: "Pracownicy",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LiczbaDniWolnych",
                table: "Pracownicy",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LiczbaDniWykorzystanych",
                table: "Pracownicy",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MPK",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NrPrzepustki",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SEPNapiecie",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SEPNr",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stanowisko",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypPracownika",
                table: "Pracownicy",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LiczbaGodzin",
                table: "Nieobecnosci",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EwidencjaCzasu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PracownikId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZlecenieId = table.Column<int>(type: "INTEGER", nullable: false),
                    GodzinaOd = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    GodzinaDo = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    OpisPrac = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LiczbaGodzin = table.Column<decimal>(type: "TEXT", nullable: false),
                    Nadgodziny = table.Column<decimal>(type: "TEXT", nullable: false),
                    DataUtworzenia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataModyfikacji = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UtworzonyPrzez = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ZmodyfikowanyPrzez = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EwidencjaCzasu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EwidencjaCzasu_Pracownicy_PracownikId",
                        column: x => x.PracownikId,
                        principalTable: "Pracownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EwidencjaCzasu_Zlecenia_ZlecenieId",
                        column: x => x.ZlecenieId,
                        principalTable: "Zlecenia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EwidencjaCzasu_Data_PracownikId",
                table: "EwidencjaCzasu",
                columns: new[] { "Data", "PracownikId" });

            migrationBuilder.CreateIndex(
                name: "IX_EwidencjaCzasu_PracownikId",
                table: "EwidencjaCzasu",
                column: "PracownikId");

            migrationBuilder.CreateIndex(
                name: "IX_EwidencjaCzasu_ZlecenieId",
                table: "EwidencjaCzasu",
                column: "ZlecenieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EwidencjaCzasu");

            migrationBuilder.DropColumn(
                name: "Informacje",
                table: "Zlecenia");

            migrationBuilder.DropColumn(
                name: "NrZlecenia",
                table: "Zlecenia");

            migrationBuilder.DropColumn(
                name: "Informacje",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "LiczbaDniNaZeszycie",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "LiczbaDniWolnych",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "LiczbaDniWykorzystanych",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "MPK",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "NrPrzepustki",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "SEPNapiecie",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "SEPNr",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "Stanowisko",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "TypPracownika",
                table: "Pracownicy");

            migrationBuilder.DropColumn(
                name: "LiczbaGodzin",
                table: "Nieobecnosci");

            migrationBuilder.RenameColumn(
                name: "FirmaId",
                table: "Pracownicy",
                newName: "FirmaIdentyfikatorowa");

            migrationBuilder.RenameColumn(
                name: "Firma",
                table: "Pracownicy",
                newName: "FirmaGlowna");

            migrationBuilder.AlterColumn<string>(
                name: "Opis",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "Nazwa",
                table: "Zlecenia",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
