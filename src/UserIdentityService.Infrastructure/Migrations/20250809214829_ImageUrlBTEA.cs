using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlBTEA : Migration
    {
        /// <summary>
        /// Applies the migration that replaces the users.profile_image_url string column with a binary profile_image column.
        /// </summary>
        /// <remarks>
        /// Drops the "profile_image_url" column from the "users" table and adds a nullable byte[] column named "profile_image"
        /// with database type "bytea" and max length 512.
        /// </remarks>
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
