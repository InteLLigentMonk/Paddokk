using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameChangeTrackingAndReservedUsernames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsernameChangeAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservedUsernames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OriginalUserId = table.Column<string>(type: "text", nullable: true),
                    ReservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReleaseAfter = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedUsernames", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservedUsernames_ReleaseAfter",
                table: "ReservedUsernames",
                column: "ReleaseAfter");

            migrationBuilder.CreateIndex(
                name: "IX_ReservedUsernames_Slug",
                table: "ReservedUsernames",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedUsernames");

            migrationBuilder.DropColumn(
                name: "LastUsernameChangeAt",
                table: "Users");
        }
    }
}
