using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDay.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "MobileTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "MobileTokens",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
