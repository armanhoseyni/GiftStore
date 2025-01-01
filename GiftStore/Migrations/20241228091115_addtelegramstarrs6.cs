using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class addtelegramstarrs6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_factors_giftCards_GiftCardId",
                table: "factors");

            migrationBuilder.DropIndex(
                name: "IX_factors_GiftCardId",
                table: "factors");

            migrationBuilder.DropColumn(
                name: "GiftCardId",
                table: "factors");

            migrationBuilder.CreateIndex(
                name: "IX_factors_GiftId",
                table: "factors",
                column: "GiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_factors_giftCards_GiftId",
                table: "factors",
                column: "GiftId",
                principalTable: "giftCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_factors_giftCards_GiftId",
                table: "factors");

            migrationBuilder.DropIndex(
                name: "IX_factors_GiftId",
                table: "factors");

            migrationBuilder.AddColumn<int>(
                name: "GiftCardId",
                table: "factors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_factors_GiftCardId",
                table: "factors",
                column: "GiftCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_factors_giftCards_GiftCardId",
                table: "factors",
                column: "GiftCardId",
                principalTable: "giftCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
