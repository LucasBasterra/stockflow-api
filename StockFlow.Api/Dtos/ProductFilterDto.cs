namespace StockFlow.Dtos;

public record ProductFilterDto(
    string? Search = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int Page = 1,
    int PageSize = 10
);