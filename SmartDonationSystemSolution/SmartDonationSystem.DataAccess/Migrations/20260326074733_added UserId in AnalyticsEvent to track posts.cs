using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDonationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedUserIdinAnalyticsEventtotrackposts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AnalyticsEvents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_ApplicationUserId",
                table: "AnalyticsEvents",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_PostId_ApplicationUserId",
                table: "AnalyticsEvents",
                columns: new[] { "PostId", "ApplicationUserId" },
                unique: true,
                filter: "[PostId] IS NOT NULL AND [ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalyticsEvents_AspNetUsers_ApplicationUserId",
                table: "AnalyticsEvents",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalyticsEvents_AspNetUsers_ApplicationUserId",
                table: "AnalyticsEvents");

            migrationBuilder.DropIndex(
                name: "IX_AnalyticsEvents_ApplicationUserId",
                table: "AnalyticsEvents");

            migrationBuilder.DropIndex(
                name: "IX_AnalyticsEvents_PostId_ApplicationUserId",
                table: "AnalyticsEvents");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AnalyticsEvents");
        }
    }
}
