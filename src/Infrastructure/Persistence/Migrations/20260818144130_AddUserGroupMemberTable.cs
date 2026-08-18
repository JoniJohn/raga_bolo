using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Raga.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGroupMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGroupMember",
                schema: "usr",
                columns: table => new
                {
                    UserGroupId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupMember", x => new { x.UserGroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserGroupMember_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalSchema: "usr",
                        principalTable: "UserGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroupMember_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "usr",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupMember_UserId",
                schema: "usr",
                table: "UserGroupMember",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroupMember",
                schema: "usr");
        }
    }
}
