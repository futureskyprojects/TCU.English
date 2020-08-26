using Microsoft.EntityFrameworkCore.Migrations;

namespace TCU.English.Migrations
{
    public partial class InitialCreateUpdateReadingPartOneDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerA",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerB",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerC",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerD",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerE",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerF",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerG",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerH",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerI",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerK",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerL",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerM",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerN",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "AnswerO",
                table: "reading_part_1");

            migrationBuilder.DropColumn(
                name: "CorrectAnswerIndex",
                table: "reading_part_1");

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "test_categories",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "test_categories",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answers",
                table: "reading_part_1",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answers",
                table: "reading_part_1");

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "test_categories",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "test_categories",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "AnswerA",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerB",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerC",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerD",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerE",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerF",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerG",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerH",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerI",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerK",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerL",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerM",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerN",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerO",
                table: "reading_part_1",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswerIndex",
                table: "reading_part_1",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
