using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class AddHintForWritingPart2SpeakingEmbed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hint",
                table: "writing_part_2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hint",
                table: "speaking_embed",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hint",
                table: "writing_part_2");

            migrationBuilder.DropColumn(
                name: "Hint",
                table: "speaking_embed");
        }
    }
}
