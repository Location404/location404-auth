using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalLoginUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_user_application_google_id",
                table: "user_application");

            migrationBuilder.DropColumn(
                name: "google_id",
                table: "user_application");

            migrationBuilder.AddColumn<bool>(
                name: "external_login",
                table: "user_application",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "external_login_provider",
                table: "user_application",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "external_login_provider_id",
                table: "user_application",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_login",
                table: "user_application");

            migrationBuilder.DropColumn(
                name: "external_login_provider",
                table: "user_application");

            migrationBuilder.DropColumn(
                name: "external_login_provider_id",
                table: "user_application");

            migrationBuilder.AddColumn<string>(
                name: "google_id",
                table: "user_application",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_application_google_id",
                table: "user_application",
                column: "google_id",
                unique: true);
        }
    }
}
