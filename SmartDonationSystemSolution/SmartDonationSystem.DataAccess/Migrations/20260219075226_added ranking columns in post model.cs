using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDonationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedrankingcolumnsinpostmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "AiSummary",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ImpactScore",
                table: "Posts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastScoredAt",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityLevel",
                table: "Posts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiSummary",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImpactScore",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LastScoredAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PriorityLevel",
                table: "Posts");

            migrationBuilder.AddColumn<float>(
                name: "Rate",
                table: "Posts",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
