using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdFromUserDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key (with correct name)
            migrationBuilder.Sql(@"
                IF OBJECT_ID('FK_UserDevices_Users_UserId', 'F') IS NOT NULL
                    ALTER TABLE [UserDevices] DROP CONSTRAINT [FK_UserDevices_Users_UserId];
                ELSE IF OBJECT_ID('FK_UserDevices_AspNetUsers_UserId', 'F') IS NOT NULL
                    ALTER TABLE [UserDevices] DROP CONSTRAINT [FK_UserDevices_AspNetUsers_UserId];
            ");

            // Drop the index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserDevices_UserId')
                    DROP INDEX [IX_UserDevices_UserId] ON [UserDevices];
            ");

            // Drop the column
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserDevices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserDevices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevices_AspNetUsers_UserId",
                table: "UserDevices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

