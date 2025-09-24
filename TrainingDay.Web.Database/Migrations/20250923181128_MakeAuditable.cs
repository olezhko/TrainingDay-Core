using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDay.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpanded",
                table: "UserTrainingGroups");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "UserTokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "UserTokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "MobileTokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "MobileTokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "MobileTokens");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "MobileTokens");

            migrationBuilder.AddColumn<bool>(
                name: "IsExpanded",
                table: "UserTrainingGroups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
