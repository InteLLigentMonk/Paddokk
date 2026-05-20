using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserCar_SpecsAndVitals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserCars");

            migrationBuilder.AddColumn<string>(
                name: "Drive",
                table: "UserCars",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "UserCars",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OdometerKm",
                table: "UserCars",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerNote",
                table: "UserCars",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "UserCars",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecsByCategory",
                table: "UserCars",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Drive",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "Engine",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "OdometerKm",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "OwnerNote",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "UserCars");

            migrationBuilder.DropColumn(
                name: "SpecsByCategory",
                table: "UserCars");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserCars",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true);
        }
    }
}
