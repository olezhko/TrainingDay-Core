using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDay.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSocialWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SocialWorkouts_ServerId",
                table: "SocialWorkouts");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "SocialWorkouts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServerId",
                table: "SocialWorkouts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkouts_ServerId",
                table: "SocialWorkouts",
                column: "ServerId",
                unique: true);
        }
    }
}
