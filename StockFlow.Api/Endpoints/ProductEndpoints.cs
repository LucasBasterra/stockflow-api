using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StockFlow.Data;
using StockFlow.Dtos;
using StockFlow.Models;

namespace StockFlow.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Productos"); // Agrupa los endpoints en Scalar

        // GET /products - Con Paginación y Filtros
group.MapGet("/", async ([AsParameters] ProductFilterDto filter, StockFlowDbContext db) =>
{
    var query = db.Products.AsNoTracking().AsQueryable();

    // Filtro por nombre (sensible/insensible según el provider de DB)
    if (!string.IsNullOrWhiteSpace(filter.Search))
    {
        query = query.Where(p => p.Name.ToLower().Contains(filter.Search.ToLower()));
    }

    // Filtro por precio mínimo
    if (filter.MinPrice.HasValue)
    {
        query = query.Where(p => p.Price >= filter.MinPrice.Value);
    }

    // Filtro por precio máximo
    if (filter.MaxPrice.HasValue)
    {
        query = query.Where(p => p.Price <= filter.MaxPrice.Value);
    }

    // Conteo total de elementos filtrados
    var totalCount = await query.CountAsync();

    // Paginación de resultados
    var products = await query
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock))
        .ToListAsync();

    return Results.Ok(new
    {
        TotalCount = totalCount,
        Page = filter.Page,
        PageSize = filter.PageSize,
        TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
        Data = products
    });
})
.WithName("GetProducts")
.WithSummary("Obtiene la lista de productos paginada y filtrada");
        // POST /products - Con ValidationFilter
        group.MapPost("/", async (CreateProductDto dto, StockFlowDbContext db) =>
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            var responseDto = new ProductDto(product.Id, product.Name, product.Price, product.Stock);
            return Results.Created($"/products/{product.Id}", responseDto);
        })
        .AddEndpointFilter<ValidationFilter<CreateProductDto>>()
        .WithName("CreateProduct")
        .WithSummary("Crea un nuevo producto en el catálogo");

        // PUT /products/{id} - Con ValidationFilter
        group.MapPut("/{id:int}", async (int id, UpdateProductDto dto, StockFlowDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .AddEndpointFilter<ValidationFilter<UpdateProductDto>>()
        .WithName("UpdateProduct")
        .WithSummary("Actualiza los datos de un producto existente");

        // DELETE /products/{id}
        group.MapDelete("/{id:int}", async (int id, StockFlowDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .WithSummary("Elimina un producto por su ID");
    }
}

// Filtro genérico para desacoplar FluentValidation de los Handlers de las rutas
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        
        if (validator is not null)
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();
            if (argument is not null)
            {
                var validationResult = await validator.ValidateAsync(argument);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
            }
        }

        return await next(context);
    }
}