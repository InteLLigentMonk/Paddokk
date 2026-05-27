using FluentAssertions;
using Paddokk.Core.Common.Pagination;

namespace Paddokk.Tests.Common.Pagination;

public class PagedResultTests
{
    [Fact]
    public void Create_NormalizesPageZeroToOne()
    {
        var result = PagedResult<int>.Create(items: [], totalCount: 0, page: 0, pageSize: 20);

        result.Page.Should().Be(1);
    }

    [Fact]
    public void Create_NormalizesNegativePageToOne()
    {
        var result = PagedResult<int>.Create(items: [], totalCount: 0, page: -5, pageSize: 20);

        result.Page.Should().Be(1);
    }

    [Fact]
    public void Create_ClampsPageSizeAboveMaxToMax()
    {
        var result = PagedResult<int>.Create(items: [], totalCount: 0, page: 1, pageSize: 500);

        result.PageSize.Should().Be(PaginationDefaults.MaxPageSize);
    }

    [Fact]
    public void Create_ReplacesNonPositivePageSizeWithDefault()
    {
        var result = PagedResult<int>.Create(items: [], totalCount: 0, page: 1, pageSize: 0);

        result.PageSize.Should().Be(PaginationDefaults.DefaultPageSize);
    }

    [Fact]
    public void Create_EmptyResult_HasNextPageIsFalse()
    {
        var result = PagedResult<int>.Create(items: [], totalCount: 0, page: 1, pageSize: 20);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_ExactPageMultiple_HasNextPageIsFalseOnLastPage()
    {
        // totalCount=40 with pageSize=20 -> 2 pages exactly. Last page should have no next.
        var lastPage = PagedResult<int>.Create(items: Enumerable.Range(1, 20).ToList(), totalCount: 40, page: 2, pageSize: 20);

        lastPage.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_ExactPageMultiple_HasNextPageIsTrueOnFirstPage()
    {
        var firstPage = PagedResult<int>.Create(items: Enumerable.Range(1, 20).ToList(), totalCount: 40, page: 1, pageSize: 20);

        firstPage.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Create_PartialLastPage_HasNextPageIsFalse()
    {
        // totalCount=25, pageSize=20, page=2 -> 5 items on page 2, no next page.
        var result = PagedResult<int>.Create(items: Enumerable.Range(21, 5).ToList(), totalCount: 25, page: 2, pageSize: 20);

        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_PreservesItemsAndTotalCount()
    {
        var items = new[] { "a", "b", "c" };

        var result = PagedResult<string>.Create(items, totalCount: 7, page: 1, pageSize: 3);

        result.Items.Should().Equal(items);
        result.TotalCount.Should().Be(7);
        result.HasNextPage.Should().BeTrue();
    }
}

public class PaginationDefaultsTests
{
    [Theory]
    [InlineData(0, 20, 1, 20)]
    [InlineData(-1, 20, 1, 20)]
    [InlineData(1, 0, 1, 20)]
    [InlineData(1, -10, 1, 20)]
    [InlineData(1, 500, 1, 100)]
    [InlineData(3, 50, 3, 50)]
    public void Normalize_AppliesBoundaryRules(int page, int pageSize, int expectedPage, int expectedSize)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        p.Should().Be(expectedPage);
        s.Should().Be(expectedSize);
    }
}
