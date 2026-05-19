using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCarsTrigramSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCars_SearchText",
                table: "UserCars");

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            migrationBuilder.Sql(@"CREATE INDEX ix_user_cars_searchtext_trgm ON ""UserCars"" USING GIN (""SearchText"" gin_trgm_ops);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ix_user_cars_searchtext_trgm;");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_SearchText",
                table: "UserCars",
                column: "SearchText");
        }
    }
}
