using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <summary>
    /// Fix StationCode unique index to allow reusing codes from deleted stations
    /// OLD: UNIQUE INDEX on StationCode (enforced on all records including deleted)
    /// NEW: FILTERED UNIQUE INDEX on StationCode WHERE IsDeleted = 0
    /// 
    /// Problem: Cannot create new station with code ST000022 if deleted station exists with that code
    /// Solution: Only enforce uniqueness on non-deleted stations
    /// </summary>
    public partial class FixStationCodeUniqueIndexForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old unique index (applies to all records)
            migrationBuilder.DropIndex(
                name: "IX_Stations_StationCode",
                table: "Stations");

            // Create filtered unique index - only enforce uniqueness for non-deleted records
            // This allows reusing StationCode from deleted stations
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_Stations_StationCode 
                ON Stations(StationCode) 
                WHERE IsDeleted = 0
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the filtered index
            migrationBuilder.DropIndex(
                name: "IX_Stations_StationCode",
                table: "Stations");

            // Recreate the old unique index (without filter)
            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationCode",
                table: "Stations",
                column: "StationCode",
                unique: true);
        }
    }
}
