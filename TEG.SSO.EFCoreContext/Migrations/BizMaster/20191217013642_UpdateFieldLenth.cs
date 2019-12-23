using Microsoft.EntityFrameworkCore.Migrations;

namespace TEG.SSO.EFCoreContext.Migrations.BizMaster
{
    public partial class UpdateFieldLenth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ObjectName",
                table: "AuthorizationObject",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ObjectName",
                table: "AuthorizationObject",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
