using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class adduserstarslog1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStarsLog_users_UserId",
                table: "UserStarsLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStarsLog",
                table: "UserStarsLog");

            migrationBuilder.RenameTable(
                name: "UserStarsLog",
                newName: "userStarsLogs");

            migrationBuilder.RenameIndex(
                name: "IX_UserStarsLog_UserId",
                table: "userStarsLogs",
                newName: "IX_userStarsLogs_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userStarsLogs",
                table: "userStarsLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userStarsLogs_users_UserId",
                table: "userStarsLogs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userStarsLogs_users_UserId",
                table: "userStarsLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userStarsLogs",
                table: "userStarsLogs");

            migrationBuilder.RenameTable(
                name: "userStarsLogs",
                newName: "UserStarsLog");

            migrationBuilder.RenameIndex(
                name: "IX_userStarsLogs_UserId",
                table: "UserStarsLog",
                newName: "IX_UserStarsLog_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStarsLog",
                table: "UserStarsLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStarsLog_users_UserId",
                table: "UserStarsLog",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
