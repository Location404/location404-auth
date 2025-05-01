using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CreateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_user_application_email_address",
                table: "user_application",
                column: "email_address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_application_google_id",
                table: "user_application",
                column: "google_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_application_username",
                table: "user_application",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_user_application_email_address",
                table: "user_application");

            migrationBuilder.DropIndex(
                name: "idx_user_application_google_id",
                table: "user_application");

            migrationBuilder.DropIndex(
                name: "idx_user_application_username",
                table: "user_application");
        }
    }
}
