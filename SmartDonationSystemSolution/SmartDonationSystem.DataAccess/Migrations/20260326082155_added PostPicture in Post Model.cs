using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDonationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedPostPictureinPostModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostPicture",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostPicture",
                table: "Posts");
        }
    }
}
