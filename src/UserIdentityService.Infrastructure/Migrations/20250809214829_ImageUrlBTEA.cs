using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlBTEA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profile_image_url",
                table: "users");

            migrationBuilder.AddColumn<byte[]>(
                name: "profile_image",
                table: "users",
                type: "bytea",
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profile_image",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "profile_image_url",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
