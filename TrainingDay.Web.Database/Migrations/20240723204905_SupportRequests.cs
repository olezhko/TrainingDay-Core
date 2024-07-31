using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDay.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class SupportRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseImageUrl",
                table: "UserLastTrainingExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseImageUrl",
                table: "UserExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseImageUrl",
                table: "Exercises");

            migrationBuilder.AddColumn<int>(
                name: "CodeNum",
                table: "UserLastTrainingExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupportRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportRequests");

            migrationBuilder.DropColumn(
                name: "CodeNum",
                table: "UserLastTrainingExercises");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseImageUrl",
                table: "UserLastTrainingExercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseImageUrl",
                table: "UserExercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseImageUrl",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
