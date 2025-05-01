using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_application",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    username = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    email_address = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    profile_pictureUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    preferred_language = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    google_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    password_salt = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_application", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_application");
        }
    }
}
