using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDonationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Addedconversationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageAt",
                table: "Conversations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User1UnreadCount",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "User2UnreadCount",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessageAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "User1UnreadCount",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "User2UnreadCount",
                table: "Conversations");
        }
    }
}
