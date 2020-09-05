using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class _200902 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Audio",
                table: "listening_media",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.CreateTable(
                name: "writing_part_1",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    DefaultSentence = table.Column<string>(nullable: false),
                    SecondSentence = table.Column<string>(nullable: false),
                    Hint = table.Column<string>(nullable: true),
                    Answers = table.Column<string>(nullable: false),
                    ExplainLink = table.Column<string>(nullable: true),
                    CreatorId = table.Column<int>(nullable: false),
                    TestCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writing_part_1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_writing_part_1_users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_writing_part_1_test_categories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "test_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_writing_part_1_CreatorId",
                table: "writing_part_1",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_writing_part_1_TestCategoryId",
                table: "writing_part_1",
                column: "TestCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "writing_part_1");

            migrationBuilder.AlterColumn<string>(
                name: "Audio",
                table: "listening_media",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
