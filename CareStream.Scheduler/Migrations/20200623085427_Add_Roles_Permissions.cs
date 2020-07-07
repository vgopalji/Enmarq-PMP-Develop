using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CareStream.Scheduler.Migrations
{
    public partial class Add_Roles_Permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
             name: "Role",
             columns: table => new
             {
                 RoleId = table.Column<long>(nullable: false)
                     .Annotation("SqlServer:Identity", "1, 1"),
                 RoleType = table.Column<string>(maxLength: 100, nullable: true),
                 RoleSection = table.Column<string>(maxLength: 100, nullable: true),
                 CreatedDate = table.Column<DateTime>(nullable: false),
                 CreatedBy = table.Column<string>(nullable: true),
                 ModifiedDate = table.Column<DateTime>(nullable: false),
                 ModifiedBy = table.Column<string>(nullable: true)
             },
             constraints: table =>
             {
                 table.PrimaryKey("PK_Role", x => x.RoleId);
             });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Read = table.Column<bool>(nullable: false),
                    Write = table.Column<bool>(nullable: false),
                    ReadWrite = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionId);
                    table.ForeignKey("FK_Role_Permission", x => x.RoleId, "Role", "RoleId");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
