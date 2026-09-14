using FluentValidation;
using Scalar.AspNetCore;
using StockFlow.Data;
using StockFlow.Endpoints;
using StockFlow.Exceptions; // IMPORTANTE: Agregar esta directiva

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos PostgreSQL con EF Core
builder.Services.AddNpgsql<StockFlowDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

// Registrar servicio nativo de OpenAPI (.NET 10)
builder.Services.AddOpenApi();

// Registrar todos los validadores de FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 1. Registrar el manejador de excepciones y los servicios de ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 2. Activar el middleware de manejo global de excepciones
app.UseExceptionHandler();

// Configuración de la documentación interactiva en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseHttpsRedirection();

// Registrar los endpoints de Productos
app.MapProductEndpoints();

app.Run();