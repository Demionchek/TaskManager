using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "Users",
                newName: "RefreshTokenExpiry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiry",
                table: "Users",
                newName: "ExpiresAt");
        }
    }
}
