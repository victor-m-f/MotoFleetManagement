using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mfm.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class Rental : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "DateOfBirth",
            table: "DeliveryPerson",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.CreateTable(
            name: "Rental",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                MotorcycleId = table.Column<string>(type: "text", nullable: false),
                DeliveryPersonId = table.Column<string>(type: "text", nullable: false),
                PlanType = table.Column<int>(type: "integer", nullable: false),
                StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ExpectedEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                TotalCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rental", x => x.Id);
                table.ForeignKey(
                    name: "FK_Rental_DeliveryPerson_DeliveryPersonId",
                    column: x => x.DeliveryPersonId,
                    principalTable: "DeliveryPerson",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Rental_Motorcycle_MotorcycleId",
                    column: x => x.MotorcycleId,
                    principalTable: "Motorcycle",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Rental_DeliveryPersonId",
            table: "Rental",
            column: "DeliveryPersonId");

        migrationBuilder.CreateIndex(
            name: "IX_Rental_MotorcycleId",
            table: "Rental",
            column: "MotorcycleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Rental");

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateOfBirth",
            table: "DeliveryPerson",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");
    }
}
