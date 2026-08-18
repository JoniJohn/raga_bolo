using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Raga.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "usr",
                table: "UserGroupMember",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "usr",
                table: "UserGroupMember",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "usr",
                table: "UserGroupMember",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "usr",
                table: "UserGroup",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "usr",
                table: "UserGroup",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "usr",
                table: "UserGroup",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "usr",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "usr",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "usr",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "usr",
                table: "UserGroupMember");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "usr",
                table: "UserGroupMember");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "usr",
                table: "UserGroupMember");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "usr",
                table: "UserGroup");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "usr",
                table: "UserGroup");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "usr",
                table: "UserGroup");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "usr",
                table: "User");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "usr",
                table: "User");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "usr",
                table: "User");
        }
    }
}
