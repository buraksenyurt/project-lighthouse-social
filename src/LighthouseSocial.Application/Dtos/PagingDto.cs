namespace LighthouseSocial.Application.Dtos;

public record PagingDto(int Page, int PageSize)
{
    public static PagingDto Create(int page = 1, int pageSize = 10)
    {
        return new PagingDto(page, pageSize);
    }
    public PagingDto() : this(1, 10)
    {
    }
    public int Skip => (Page - 1) * PageSize;
}
