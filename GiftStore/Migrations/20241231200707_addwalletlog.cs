using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class addwalletlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Star",
                table: "WalletLog");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "WalletLog",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WalletLog");

            migrationBuilder.AddColumn<double>(
                name: "Star",
                table: "WalletLog",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
