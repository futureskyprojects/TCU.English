using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class AddInstructorRefForTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "piece_of_test",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_piece_of_test_InstructorId",
                table: "piece_of_test",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_piece_of_test_users_InstructorId",
                table: "piece_of_test",
                column: "InstructorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_piece_of_test_users_InstructorId",
                table: "piece_of_test");

            migrationBuilder.DropIndex(
                name: "IX_piece_of_test_InstructorId",
                table: "piece_of_test");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "piece_of_test");
        }
    }
}
