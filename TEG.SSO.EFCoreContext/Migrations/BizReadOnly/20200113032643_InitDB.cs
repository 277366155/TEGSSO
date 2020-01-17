using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TEG.SSO.EFCoreContext.Migrations.BizReadOnly
{
    public partial class InitDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSystem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SystemCode = table.Column<string>(maxLength: 64, nullable: true),
                    SystemName = table.Column<string>(maxLength: 64, nullable: true),
                    SystemType = table.Column<int>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSystem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    OrgName = table.Column<string>(maxLength: 64, nullable: true),
                    ParentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Organization_Organization_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Organization",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RoleName = table.Column<string>(maxLength: 64, nullable: true),
                    ParentID = table.Column<int>(nullable: true),
                    IsSuperAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Role_Role_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecurityQuestion",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    QuestionContent = table.Column<string>(maxLength: 512, nullable: true),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityQuestion", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AccountName = table.Column<string>(maxLength: 64, nullable: true),
                    UserName = table.Column<string>(maxLength: 64, nullable: true),
                    Password = table.Column<string>(maxLength: 256, nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Mobile = table.Column<string>(maxLength: 32, nullable: true),
                    Telphone = table.Column<string>(maxLength: 32, nullable: true),
                    Email = table.Column<string>(maxLength: 64, nullable: true),
                    QQ = table.Column<string>(maxLength: 16, nullable: true),
                    ValidTime = table.Column<DateTime>(nullable: false),
                    FirstChange = table.Column<bool>(nullable: false),
                    PasswordModifyPeriod = table.Column<int>(nullable: false),
                    PasswordModifyTime = table.Column<DateTime>(nullable: false),
                    IsNew = table.Column<bool>(nullable: false),
                    IsMemberShipPassword = table.Column<bool>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserSessionLog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    UserToken = table.Column<string>(maxLength: 256, nullable: true),
                    SystemID = table.Column<int>(nullable: false),
                    SystemName = table.Column<string>(maxLength: 64, nullable: true),
                    AccessHost = table.Column<string>(maxLength: 64, nullable: true),
                    ValidTime = table.Column<int>(nullable: false),
                    RealExpirationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessionLog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    MenuCode = table.Column<string>(maxLength: 64, nullable: true),
                    MenuName = table.Column<string>(maxLength: 512, nullable: true),
                    SystemID = table.Column<int>(nullable: false),
                    ParentID = table.Column<int>(nullable: true),
                    SortID = table.Column<int>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Menu_Menu_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Menu",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Menu_AppSystem_SystemID",
                        column: x => x.SystemID,
                        principalTable: "AppSystem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDeptRel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    DeptID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeptRel", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserDeptRel_Organization_DeptID",
                        column: x => x.DeptID,
                        principalTable: "Organization",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDeptRel_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleRel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    RoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleRel", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserRoleRel_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleRel_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurityQuestion",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false),
                    Answer = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityQuestion", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestion_SecurityQuestion_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "SecurityQuestion",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestion_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationObject",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ObjectCode = table.Column<string>(maxLength: 64, nullable: true),
                    ObjectName = table.Column<string>(maxLength: 512, nullable: true),
                    ObjectType = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationObject", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AuthorizationObject_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menu",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleRight",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    LastUpdateAccountName = table.Column<string>(maxLength: 64, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RoleID = table.Column<int>(nullable: false),
                    IsMenu = table.Column<bool>(nullable: false),
                    MenuID = table.Column<int>(nullable: true),
                    AuthorizationObjectID = table.Column<int>(nullable: true),
                    PermissionValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRight", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RoleRight_AuthorizationObject_AuthorizationObjectID",
                        column: x => x.AuthorizationObjectID,
                        principalTable: "AuthorizationObject",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleRight_Menu_MenuID",
                        column: x => x.MenuID,
                        principalTable: "Menu",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleRight_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationObject_MenuId",
                table: "AuthorizationObject",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_ParentID",
                table: "Menu",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_SystemID",
                table: "Menu",
                column: "SystemID");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ParentID",
                table: "Organization",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ParentID",
                table: "Role",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRight_AuthorizationObjectID",
                table: "RoleRight",
                column: "AuthorizationObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRight_MenuID",
                table: "RoleRight",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRight_RoleID",
                table: "RoleRight",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserDeptRel_DeptID",
                table: "UserDeptRel",
                column: "DeptID");

            migrationBuilder.CreateIndex(
                name: "IX_UserDeptRel_UserID",
                table: "UserDeptRel",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleRel_RoleID",
                table: "UserRoleRel",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleRel_UserID",
                table: "UserRoleRel",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityQuestion_QuestionID",
                table: "UserSecurityQuestion",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityQuestion_UserID",
                table: "UserSecurityQuestion",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleRight");

            migrationBuilder.DropTable(
                name: "UserDeptRel");

            migrationBuilder.DropTable(
                name: "UserRoleRel");

            migrationBuilder.DropTable(
                name: "UserSecurityQuestion");

            migrationBuilder.DropTable(
                name: "UserSessionLog");

            migrationBuilder.DropTable(
                name: "AuthorizationObject");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "SecurityQuestion");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "AppSystem");
        }
    }
}
