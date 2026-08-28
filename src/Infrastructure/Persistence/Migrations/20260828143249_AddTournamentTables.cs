using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Raga.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tour");

            migrationBuilder.EnsureSchema(
                name: "conf");

            migrationBuilder.CreateTable(
                name: "TournamentCupType",
                schema: "conf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentCupType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournament",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RefNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    TournamentCupTypeId = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournament_TournamentCupType_TournamentCupTypeId",
                        column: x => x.TournamentCupTypeId,
                        principalSchema: "conf",
                        principalTable: "TournamentCupType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournament_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "usr",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "conf",
                table: "TournamentCupType",
                columns: new[] { "Id", "Code", "created_at", "Description", "is_active", "Name", "updated_at" },
                values: new object[,]
                {
                    { 1, "SL", new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(5307), new TimeSpan(0, 0, 0, 0, 0)), "Single fixture league tournament format", true, "Single Fixture League", null },
                    { 2, "HAD", new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(9364), new TimeSpan(0, 0, 0, 0, 0)), "Home and away league tournament format", true, "Home and Away League", null },
                    { 3, "SK", new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(9369), new TimeSpan(0, 0, 0, 0, 0)), "Single fixture knockout tournament format", true, "Single Fixture Knockout", null },
                    { 4, "HAK", new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(9371), new TimeSpan(0, 0, 0, 0, 0)), "Home and away knockout tournament format", true, "Home and Away Knockout", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_OwnerId",
                schema: "tour",
                table: "Tournament",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_RefNumber",
                schema: "tour",
                table: "Tournament",
                column: "RefNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_TournamentCupTypeId",
                schema: "tour",
                table: "Tournament",
                column: "TournamentCupTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentCupType_Code",
                schema: "conf",
                table: "TournamentCupType",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tournament",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "TournamentCupType",
                schema: "conf");
        }
    }
}
