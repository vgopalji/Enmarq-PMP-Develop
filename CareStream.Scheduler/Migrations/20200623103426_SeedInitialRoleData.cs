using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CareStream.Scheduler.Migrations
{
    public partial class SeedInitialRoleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate", "RoleSection", "RoleType" },
                values: new object[,]
                {
                    { 1L, "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 64, DateTimeKind.Local).AddTicks(857), "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 64, DateTimeKind.Local).AddTicks(1650), "Users", null },
                    { 2L, "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 65, DateTimeKind.Local).AddTicks(9846), "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 65, DateTimeKind.Local).AddTicks(9869), "Groups", null },
                    { 3L, "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 66, DateTimeKind.Local).AddTicks(31), "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 66, DateTimeKind.Local).AddTicks(35), "UserAttributes", null },
                    { 4L, "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 66, DateTimeKind.Local).AddTicks(73), "Admin", new DateTime(2020, 6, 23, 16, 4, 26, 66, DateTimeKind.Local).AddTicks(76), "BulkOperations", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 4L);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Permission");
        }
    }
}
