using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCarEngagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCarLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserCarId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCarLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCarLikes_UserCars_UserCarId",
                        column: x => x.UserCarId,
                        principalTable: "UserCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCarLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCarSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserCarId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCarSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCarSubscriptions_UserCars_UserCarId",
                        column: x => x.UserCarId,
                        principalTable: "UserCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCarSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCarLikes_UserCarId",
                table: "UserCarLikes",
                column: "UserCarId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCarLikes_UserId_UserCarId",
                table: "UserCarLikes",
                columns: new[] { "UserId", "UserCarId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCarSubscriptions_UserCarId",
                table: "UserCarSubscriptions",
                column: "UserCarId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCarSubscriptions_UserId_UserCarId",
                table: "UserCarSubscriptions",
                columns: new[] { "UserId", "UserCarId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCarLikes");

            migrationBuilder.DropTable(
                name: "UserCarSubscriptions");
        }
    }
}
