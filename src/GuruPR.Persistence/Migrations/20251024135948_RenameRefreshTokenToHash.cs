using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuruPR.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameRefreshTokenToHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "AspNetUsers",
                newName: "RefreshTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "AspNetUsers",
                newName: "RefreshToken");
        }
    }
}
