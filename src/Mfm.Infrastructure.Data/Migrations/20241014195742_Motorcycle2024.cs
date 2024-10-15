using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mfm.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class Motorcycle2024 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Motorcycle2024",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Year = table.Column<int>(type: "integer", nullable: false),
                Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                LicensePlate = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Motorcycle2024", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Motorcycle2024_LicensePlate",
            table: "Motorcycle2024",
            column: "LicensePlate",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Motorcycle2024");
    }
}
