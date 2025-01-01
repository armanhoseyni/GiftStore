using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftStore.Migrations
{
    /// <inheritdoc />
    public partial class addTicketChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "response",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "responseDate",
                table: "tickets");

            migrationBuilder.CreateTable(
                name: "ticketChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sender = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticketChats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticketChats");

            migrationBuilder.AddColumn<string>(
                name: "response",
                table: "tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "responseDate",
                table: "tickets",
                type: "datetime2",
                nullable: true);
        }
    }
}
