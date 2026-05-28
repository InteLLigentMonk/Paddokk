using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnsureFkIndexes : Migration
    {
        // Defensive FK-index migration. These indexes match the EF model snapshot and are auto-created by EF
        // for new databases; the explicit `CREATE INDEX IF NOT EXISTS` guards against environments where the
        // initial migrations were modified or the indexes were dropped manually. Idempotent by design.
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_UserCarLikes_UserCarId"" ON ""UserCarLikes"" (""UserCarId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_JourneySubscriptions_JourneyId"" ON ""JourneySubscriptions"" (""JourneyId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_JourneyPostImages_JourneyPostId"" ON ""JourneyPostImages"" (""JourneyPostId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_PostComments_JourneyPostId"" ON ""PostComments"" (""JourneyPostId"");");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: dropping these indexes would silently regress query performance and they were defensively
            // re-created (`IF NOT EXISTS`) in Up — the indexes pre-existed in earlier migrations.
        }
    }
}
