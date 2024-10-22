using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mfm.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RentalReturnDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ReturnDate",
            table: "Rental",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ReturnDate",
            table: "Rental");
    }
}
