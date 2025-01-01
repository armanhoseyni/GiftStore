using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class addtelegramstarrs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "factors");

            migrationBuilder.DropColumn(
                name: "UserPhone",
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

            migrationBuilder.CreateIndex(
                name: "IX_factors_UserId",
                table: "factors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_factors_giftCards_GiftCardId",
                table: "factors",
                column: "GiftCardId",
                principalTable: "giftCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_factors_users_UserId",
                table: "factors",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_factors_giftCards_GiftCardId",
                table: "factors");

            migrationBuilder.DropForeignKey(
                name: "FK_factors_users_UserId",
                table: "factors");

            migrationBuilder.DropIndex(
                name: "IX_factors_GiftCardId",
                table: "factors");

            migrationBuilder.DropIndex(
                name: "IX_factors_UserId",
                table: "factors");

            migrationBuilder.DropColumn(
                name: "GiftCardId",
                table: "factors");

            migrationBuilder.AddColumn<double>(
                name: "PaymentAmount",
                table: "factors",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "UserPhone",
                table: "factors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
