using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TEG.SSO.LogDBContext.Migrations
{
    public partial class InitLogDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Token = table.Column<string>(maxLength: 256, nullable: true),
                    Url = table.Column<string>(maxLength: 1024, nullable: true),
                    Request = table.Column<string>(type: "text", nullable: true),
                    IP = table.Column<string>(maxLength: 128, nullable: true),
                    ErrorMsg = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OperationLog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    AccountName = table.Column<string>(maxLength: 64, nullable: true),
                    UserToken = table.Column<string>(maxLength: 256, nullable: true),
                    SystemCode = table.Column<string>(maxLength: 64, nullable: true),
                    AccessHost = table.Column<string>(maxLength: 64, nullable: true),
                    ActionCode = table.Column<string>(maxLength: 64, nullable: true),
                    URL = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLog", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLog");

            migrationBuilder.DropTable(
                name: "OperationLog");
        }
    }
}
