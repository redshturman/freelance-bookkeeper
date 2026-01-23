using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceBookkeeper.Migrations
{
    /// <inheritdoc />
    public partial class AddedTaxAndBasePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "Expenses",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "CustomerTransactions",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "CustomerTransactions");
        }
    }
}
