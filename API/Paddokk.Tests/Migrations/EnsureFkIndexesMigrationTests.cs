using FluentAssertions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Paddokk.Data.Migrations;

namespace Paddokk.Tests.Migrations;

public class EnsureFkIndexesMigrationTests
{
    [Fact]
    public void Up_EmitsCreateIndexIfNotExistsForAllFourForeignKeyIndexes()
    {
        var migration = new EnsureFkIndexes();

        var operations = migration.UpOperations.OfType<SqlOperation>().ToList();

        operations.Should().HaveCount(4, "the migration covers exactly the four FK indexes called out in the PRD");

        operations.Should().AllSatisfy(op => op.Sql.Should().Contain("CREATE INDEX IF NOT EXISTS",
            "CREATE INDEX IF NOT EXISTS guarantees a clean-apply on fresh DBs and a no-op on already-migrated DBs"));

        var combinedSql = string.Join("\n", operations.Select(op => op.Sql));
        combinedSql.Should().Contain("\"IX_UserCarLikes_UserCarId\"").And.Contain("\"UserCarLikes\"").And.Contain("\"UserCarId\"");
        combinedSql.Should().Contain("\"IX_JourneySubscriptions_JourneyId\"").And.Contain("\"JourneySubscriptions\"").And.Contain("\"JourneyId\"");
        combinedSql.Should().Contain("\"IX_JourneyPostImages_JourneyPostId\"").And.Contain("\"JourneyPostImages\"").And.Contain("\"JourneyPostId\"");
        combinedSql.Should().Contain("\"IX_PostComments_JourneyPostId\"").And.Contain("\"PostComments\"").And.Contain("\"JourneyPostId\"");
    }

    [Fact]
    public void Down_IsNoOp_SoReapplyDoesNotRegressIndexes()
    {
        var migration = new EnsureFkIndexes();

        migration.DownOperations.Should().BeEmpty(
            "the indexes pre-existed in earlier migrations; Down dropping them would silently regress query perf");
    }
}
