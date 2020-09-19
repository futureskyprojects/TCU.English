using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class AddSpeakingEmbed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WritingPartTwos_users_CreatorId",
                table: "WritingPartTwos");

            migrationBuilder.DropForeignKey(
                name: "FK_WritingPartTwos_test_categories_TestCategoryId",
                table: "WritingPartTwos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WritingPartTwos",
                table: "WritingPartTwos");

            migrationBuilder.RenameTable(
                name: "WritingPartTwos",
                newName: "writing_part_2");

            migrationBuilder.RenameIndex(
                name: "IX_WritingPartTwos_TestCategoryId",
                table: "writing_part_2",
                newName: "IX_writing_part_2_TestCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_WritingPartTwos_CreatorId",
                table: "writing_part_2",
                newName: "IX_writing_part_2_CreatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_writing_part_2",
                table: "writing_part_2",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "speaking_embed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    YoutubeVideo = table.Column<string>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    TestCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_speaking_embed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_speaking_embed_users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_speaking_embed_test_categories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "test_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_speaking_embed_CreatorId",
                table: "speaking_embed",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_speaking_embed_TestCategoryId",
                table: "speaking_embed",
                column: "TestCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_writing_part_2_users_CreatorId",
                table: "writing_part_2",
                column: "CreatorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_writing_part_2_test_categories_TestCategoryId",
                table: "writing_part_2",
                column: "TestCategoryId",
                principalTable: "test_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_writing_part_2_users_CreatorId",
                table: "writing_part_2");

            migrationBuilder.DropForeignKey(
                name: "FK_writing_part_2_test_categories_TestCategoryId",
                table: "writing_part_2");

            migrationBuilder.DropTable(
                name: "speaking_embed");

            migrationBuilder.DropPrimaryKey(
                name: "PK_writing_part_2",
                table: "writing_part_2");

            migrationBuilder.RenameTable(
                name: "writing_part_2",
                newName: "WritingPartTwos");

            migrationBuilder.RenameIndex(
                name: "IX_writing_part_2_TestCategoryId",
                table: "WritingPartTwos",
                newName: "IX_WritingPartTwos_TestCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_writing_part_2_CreatorId",
                table: "WritingPartTwos",
                newName: "IX_WritingPartTwos_CreatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WritingPartTwos",
                table: "WritingPartTwos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WritingPartTwos_users_CreatorId",
                table: "WritingPartTwos",
                column: "CreatorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WritingPartTwos_test_categories_TestCategoryId",
                table: "WritingPartTwos",
                column: "TestCategoryId",
                principalTable: "test_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
