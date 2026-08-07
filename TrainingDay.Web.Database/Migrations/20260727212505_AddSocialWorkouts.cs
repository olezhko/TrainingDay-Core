using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TrainingDay.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialWorkouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShareCompletedWorkouts",
                table: "MobileUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "SocialWorkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    OwnerNickname = table.Column<string>(type: "longtext", nullable: false),
                    WorkoutName = table.Column<string>(type: "longtext", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialWorkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialWorkouts_MobileUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "MobileUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SocialWorkoutExercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SocialWorkoutId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    ExerciseName = table.Column<string>(type: "longtext", nullable: false),
                    MusclesString = table.Column<string>(type: "longtext", nullable: false),
                    WeightAndRepsString = table.Column<string>(type: "longtext", nullable: false),
                    TagsValue = table.Column<int>(type: "int", nullable: false),
                    CodeNum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialWorkoutExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialWorkoutExercises_SocialWorkouts_SocialWorkoutId",
                        column: x => x.SocialWorkoutId,
                        principalTable: "SocialWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SocialWorkoutLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SocialWorkoutId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialWorkoutLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialWorkoutLikes_MobileUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "MobileUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialWorkoutLikes_SocialWorkouts_SocialWorkoutId",
                        column: x => x.SocialWorkoutId,
                        principalTable: "SocialWorkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkoutExercises_SocialWorkoutId",
                table: "SocialWorkoutExercises",
                column: "SocialWorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkoutLikes_SocialWorkoutId_UserId",
                table: "SocialWorkoutLikes",
                columns: new[] { "SocialWorkoutId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkoutLikes_UserId",
                table: "SocialWorkoutLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkouts_OwnerUserId",
                table: "SocialWorkouts",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkouts_ServerId",
                table: "SocialWorkouts",
                column: "ServerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialWorkoutExercises");

            migrationBuilder.DropTable(
                name: "SocialWorkoutLikes");

            migrationBuilder.DropTable(
                name: "SocialWorkouts");

            migrationBuilder.DropColumn(
                name: "ShareCompletedWorkouts",
                table: "MobileUsers");
        }
    }
}
