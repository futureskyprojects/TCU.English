using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class AddWritingPart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WritingPartTwos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    Questions = table.Column<string>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    TestCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingPartTwos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WritingPartTwos_users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingPartTwos_test_categories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "test_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WritingPartTwos_CreatorId",
                table: "WritingPartTwos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingPartTwos_TestCategoryId",
                table: "WritingPartTwos",
                column: "TestCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WritingPartTwos");
        }
    }
}
