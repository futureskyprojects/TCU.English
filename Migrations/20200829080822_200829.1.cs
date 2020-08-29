using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class _2008291 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "listening_base_question",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    QuestionText = table.Column<string>(nullable: true),
                    Hint = table.Column<string>(nullable: true),
                    Answers = table.Column<string>(nullable: false),
                    ExplainLink = table.Column<string>(nullable: true),
                    CreatorId = table.Column<int>(nullable: false),
                    TestCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listening_base_question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_listening_base_question_users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_listening_base_question_test_categories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "test_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listening_media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    Audio = table.Column<string>(nullable: false),
                    Transcript = table.Column<string>(nullable: true),
                    TestCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listening_media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_listening_media_test_categories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "test_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_listening_base_question_CreatorId",
                table: "listening_base_question",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_listening_base_question_TestCategoryId",
                table: "listening_base_question",
                column: "TestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_listening_media_TestCategoryId",
                table: "listening_media",
                column: "TestCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listening_base_question");

            migrationBuilder.DropTable(
                name: "listening_media");
        }
    }
}
