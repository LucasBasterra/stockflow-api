namespace StockFlow.Dtos;

public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    int Stock
);