using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDonationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedlastmessageIsreadinconversationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "lastMessageIsRead",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastMessageIsRead",
                table: "Conversations");
        }
    }
}
