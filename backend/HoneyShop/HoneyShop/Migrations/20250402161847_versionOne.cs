using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyShop.Migrations
{
    /// <inheritdoc />
    public partial class versionOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "Honey",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    shortDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    longDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    shelfLife = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    bju = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isFavorite = table.Column<bool>(type: "bit", nullable: false),
                    newHoney = table.Column<bool>(type: "bit", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    avaliable = table.Column<bool>(type: "bit", nullable: false),
                    Categoryid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Honey", x => x.id);
                    table.ForeignKey(
                        name: "FK_Honey_Category_Categoryid",
                        column: x => x.Categoryid,
                        principalTable: "Category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });




            migrationBuilder.CreateIndex(
                name: "IX_Honey_Categoryid",
                table: "Honey",
                column: "Categoryid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Honey");

        }
    }
}
