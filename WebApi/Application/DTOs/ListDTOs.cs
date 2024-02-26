namespace WebApi.Application.DTOs;

public record ListResponse<T>(long Count, IEnumerable<T> Records);

public record ListQuery()
{
    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
    public string[]? SortBy { get; set; } = null;
};
