using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingDay.Web.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedAnonymousUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Raw SQL instead of InsertData: MySql.EntityFrameworkCore appends "SELECT ROW_COUNT();" to
            // InsertData/DeleteData, which fails with InvalidCastException (DBNull -> Int64) when migrating.
            migrationBuilder.Sql(
                """
                INSERT INTO `MobileUsers` (`Id`, `AccessFailedCount`, `ConcurrencyStamp`, `Email`, `EmailConfirmed`, `LockoutEnabled`, `LockoutEnd`, `NormalizedEmail`, `NormalizedUserName`, `PasswordHash`, `PhoneNumber`, `PhoneNumberConfirmed`, `SecurityStamp`, `ShareCompletedWorkouts`, `TwoFactorEnabled`, `UserName`)
                VALUES ('00000000-0000-0000-0000-000000000001', 0, '0B1C2D3E-4F50-4617-8293-A4B5C6D7E8F9', NULL, FALSE, TRUE, NULL, NULL, 'ANONYMOUS', NULL, NULL, FALSE, '7F3B2C9E-5A41-4D8E-9C6B-1E2F3A4B5C6D', TRUE, FALSE, 'Anonymous');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM `MobileUsers` WHERE `Id` = '00000000-0000-0000-0000-000000000001';");
        }
    }
}
