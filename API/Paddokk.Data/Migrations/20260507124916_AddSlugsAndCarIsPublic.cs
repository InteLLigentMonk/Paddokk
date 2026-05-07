using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugsAndCarIsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCars_PrincipalId",
                table: "UserCars");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_PrincipalId",
                table: "Journeys");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "UserCars",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "UserCars",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Journeys",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_PrincipalId_Slug",
                table: "UserCars",
                columns: new[] { "PrincipalId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_PrincipalId_Slug",
                table: "Journeys",
                columns: new[] { "PrincipalId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCars_PrincipalId_Slug",
                table: "UserCars");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_PrincipalId_Slug",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Journeys");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_PrincipalId",
                table: "UserCars",
                column: "PrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_PrincipalId",
                table: "Journeys",
                column: "PrincipalId");
        }
    }
}
