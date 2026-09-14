Especificación de Arquitectura e Infraestructura: StockFlow APIDocumentación Técnica del Sistema: StockFlow APIVersión: 1.0.0Framework: .NET 10.0Motor de Base de Datos: PostgreSQL v18Patrón Arquitectónico: Clean Architecture con Minimal APIs1. Módulos y Middleware del Pipeline[ Cliente HTTP ]
       │
       ▼
┌─────────────────────────────────────────┐
│ GlobalExceptionHandler (IExceptionHandler)│ ──► Devuelve ProblemDetails (500) en errores no controlados
└────────────────────┬────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────┐
│ Endpoint Routing / Minimal API Group    │
└────────────────────┬────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────┐
│ ValidationFilter<T> (IEndpointFilter)   │ ──► Captura errores con FluentValidation y retorna 400 Bad Request
└────────────────────┬────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────┐
│ EF Core + Npgsql (PostgreSQL v18)       │
└─────────────────────────────────────────┘
2. Definición de EndpointsMétodoRutaParámetros / BodyDescripciónRespuesta ExitosaGET/productsQuery: Search, MinPrice, MaxPrice, Page, PageSizeConsulta paginada y filtrada200 OK (Objeto con Data, Page, TotalCount)GET/products/{id}Route: id (int)Obtiene un producto por su clave primaria200 OK (Objeto ProductDto) / 404 Not FoundPOST/productsBody: CreateProductDtoRegistra un nuevo producto201 Created / 400 Bad Request (ProblemDetails)PUT/products/{id}Route: id (int), Body: UpdateProductDtoActualiza un producto existente204 No Content / 400 Bad Request / 404 Not FoundDELETE/products/{id}Route: id (int)Elimina un producto por ID204 No Content / 404 Not Found3. Esquemas de Datos (DTOs)Filtro y Paginación (ProductFilterDto)C#public record ProductFilterDto(
    string? Search = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int Page = 1,
    int PageSize = 10
);
Respuesta PaginadaJSON{
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1,
  "data": [
    {
      "id": 1,
      "name": "Monitor 24 Pulgadas",
      "price": 185.50,
      "stock": 12
    }
  ]
}
4. Estándares de Manejo de ErroresError de Validación (HTTP 400 - RFC 9110)Devuelto de forma interceptada por ValidationFilter<T> cuando los datos de entrada violan las reglas definidas en FluentValidation.Error Interno del Servidor (HTTP 500 - RFC 7807)Devuelto por GlobalExceptionHandler al interceptar excepciones no controladas mediante la interfaz IExceptionHandler.