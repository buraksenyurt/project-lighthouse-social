namespace LighthouseSocial.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int TotalCount { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PagedResult(IEnumerable<T> items,int totalCount, int currentPage,int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
