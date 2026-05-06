using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorJourneyStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Journeys",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            // Old Archived (4) → Active + hidden
            migrationBuilder.Sql("UPDATE \"Journeys\" SET \"Status\" = 1, \"IsPublic\" = false WHERE \"Status\" = 4");
            // Old OnHold (3) → Active + still public
            migrationBuilder.Sql("UPDATE \"Journeys\" SET \"Status\" = 1 WHERE \"Status\" = 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Journeys");
        }
    }
}
