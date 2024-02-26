using WebApi.Application.DTOs;
using WebApi.Infrastructure.Database;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Application.Abstractions;

public class Repository<TModel>(AppDbContext appContext)
    where TModel : class
{
    private readonly AppDbContext _appContext = appContext;

    public async Task<ListResponse<TModel>> Matching(
        ListQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var records = _appContext.Set<TModel>().AsQueryable().AsNoTracking();

        if (query.SortBy is not null && query.SortBy.Length > 0)
        {
            records = records.OrderBy(
                string.Join(
                    ", ",
                    query.SortBy.Select(
                        x =>
                            $"{x.Replace("-", "").Replace("+", "")} {(x.StartsWith('-') ? "desc" : "")}"
                    )
                )
            );
        }

        var count = await records.LongCountAsync(cancellationToken);

        if (query is { Page: not null, PageSize: not null })
        {
            var page = query.Page < 1 ? 1 : query.Page.Value;

            records = records.Skip((page - 1) * query.PageSize.Value);

            if (query.PageSize > 0)
            {
                records = records.Take(query.PageSize.Value);
            }
        }

        return new ListResponse<TModel>(count, records.ToList());
    }
}
