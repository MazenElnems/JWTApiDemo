using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTAuthApp.Migrations
{
    /// <inheritdoc />
    public partial class seedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "User", "USER", Guid.NewGuid().ToString() },
                    { Guid.NewGuid(), "Admin", "ADMIN", Guid.NewGuid().ToString() }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles");
        }
    }
}
