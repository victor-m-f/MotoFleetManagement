using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mfm.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class DeliveryPerson : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DeliveryPerson",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CnhNumber = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                CnhType = table.Column<string>(type: "text", nullable: false),
                CnhImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeliveryPerson", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DeliveryPerson_CnhNumber",
            table: "DeliveryPerson",
            column: "CnhNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_DeliveryPerson_Cnpj",
            table: "DeliveryPerson",
            column: "Cnpj",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeliveryPerson");
    }
}
