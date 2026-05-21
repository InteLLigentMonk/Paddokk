using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJourneysTrigramSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchText",
                table: "Journeys",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"CREATE INDEX ix_journeys_searchtext_trgm ON ""Journeys"" USING GIN (""SearchText"" gin_trgm_ops);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ix_journeys_searchtext_trgm;");

            migrationBuilder.DropColumn(
                name: "SearchText",
                table: "Journeys");
        }
    }
}
