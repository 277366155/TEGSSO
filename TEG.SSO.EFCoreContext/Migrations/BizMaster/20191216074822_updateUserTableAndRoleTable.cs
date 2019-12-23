using Microsoft.EntityFrameworkCore.Migrations;

namespace TEG.SSO.EFCoreContext.Migrations.BizMaster
{
    public partial class updateUserTableAndRoleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "Role",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "Role");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "User",
                nullable: false,
                defaultValue: false);
        }
    }
}
