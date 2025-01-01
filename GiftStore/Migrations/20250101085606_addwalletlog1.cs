using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class addwalletlog1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletLog_users_UserId",
                table: "WalletLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletLog",
                table: "WalletLog");

            migrationBuilder.RenameTable(
                name: "WalletLog",
                newName: "walletLogs");

            migrationBuilder.RenameIndex(
                name: "IX_WalletLog_UserId",
                table: "walletLogs",
                newName: "IX_walletLogs_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_walletLogs",
                table: "walletLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_walletLogs_users_UserId",
                table: "walletLogs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_walletLogs_users_UserId",
                table: "walletLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_walletLogs",
                table: "walletLogs");

            migrationBuilder.RenameTable(
                name: "walletLogs",
                newName: "WalletLog");

            migrationBuilder.RenameIndex(
                name: "IX_walletLogs_UserId",
                table: "WalletLog",
                newName: "IX_WalletLog_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletLog",
                table: "WalletLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletLog_users_UserId",
                table: "WalletLog",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
