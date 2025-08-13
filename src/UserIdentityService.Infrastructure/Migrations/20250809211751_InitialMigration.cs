using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <summary>
        /// Builds the initial database schema for user identities.
        /// </summary>
        /// <remarks>
        /// Creates the "users" table with columns for id, email, password, username, profile image URL,
        /// email verification flag, active flag, creation/updated/last-login timestamps, and preferred language.
        /// Adds a unique index on users.email. Creates the "external_logins" table with a composite primary key
        /// (login_provider, provider_key) and a foreign key to users.id (cascade delete), plus an index on user_id.
        /// This method defines the schema changes applied when the migration is run.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    profile_image_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    preferred_language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "external_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_users_external_logins",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_external_logins_user_id",
                table: "external_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "external_logins");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
