using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomBuildAndFTS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "UserCars",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CarModelId",
                table: "UserCars",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CarMakeId",
                table: "UserCars",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "CustomBuildName",
                table: "UserCars",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomBuild",
                table: "UserCars",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SearchText",
                table: "UserCars",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_SearchText",
                table: "UserCars",
                column: "SearchText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCars_SearchText",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "CustomBuildName",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "IsCustomBuild",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "SearchText",
                table: "UserCars");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "UserCars",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarModelId",
                table: "UserCars",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarMakeId",
                table: "UserCars",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
