using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class addTicketChats2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentPath",
                table: "ticketChats",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_UserId",
                table: "tickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ticketChats_TicketId",
                table: "ticketChats",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_ticketChats_tickets_TicketId",
                table: "ticketChats",
                column: "TicketId",
                principalTable: "tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_users_UserId",
                table: "tickets",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ticketChats_tickets_TicketId",
                table: "ticketChats");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_users_UserId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_tickets_UserId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_ticketChats_TicketId",
                table: "ticketChats");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPath",
                table: "ticketChats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
