using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Location404.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateIndexInRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_refresh_tokens_token",
                table: "refresh_tokens",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_refresh_tokens_token",
                table: "refresh_tokens");
        }
    }
}
