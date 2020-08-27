using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class _2008272 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reading_part_2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    QuestionText = table.Column<string>(nullable: false),
                    QuestionImage = table.Column<string>(nullable: true),
                    Hint = table.Column<string>(nullable: true),
                    Answers = table.Column<string>(nullable: false),
                    ExplainLink = table.Column<string>(nullable: true),
                    CreatorId = table.Column<int>(nullable: false),
                    TestCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reading_part_2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reading_part_2_users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reading_part_2_test_categories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "test_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reading_part_2_CreatorId",
                table: "reading_part_2",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_reading_part_2_TestCategoryId",
                table: "reading_part_2",
                column: "TestCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reading_part_2");
        }
    }
}
