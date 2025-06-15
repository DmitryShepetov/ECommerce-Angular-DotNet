using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyShop.Migrations
{
    /// <inheritdoc />
    public partial class versionFour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "OrderItem");

        }
    }
}
