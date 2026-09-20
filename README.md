# StockFlow API

API RESTful para la gestión y trazabilidad de inventarios y control de stock construida con **.NET 10** y **PostgreSQL 16**.

Este proyecto implementa una arquitectura ligera basada en Minimal APIs y manejo global de excepciones, diseñada para la administración de productos, categorías y movimientos de stock con persistencia relacional aislada en Docker.

---

## 🛠️ Stack y Tecnologías

* **Runtime & Framework:** .NET 10 (Minimal APIs)
* **Persistencia:** Entity Framework Core 10 (PostgreSQL 16)
* **Validación:** FluentValidation
* **Pruebas HTTP:** Archivo ejecutable `.http` integrado
* **Infraestructura:** Docker & Docker Compose

---

## 📂 Estructura del Proyecto

```text
StockFlow/
├── docker-compose.yml
├── README.md
├── .gitignore
└── StockFlow.Api/
    ├── Data/                 # DbContext y Migraciones EF Core
    ├── Dtos/                 # Data Transfer Objects
    ├── Endpoints/            # Minimal APIs (Products, Stock)
    ├── Exceptions/           # Middleware de manejo global de errores
    ├── Models/               # Entidades de Dominio
    ├── Validators/           # Reglas de validación
    ├── StockFlow.http        # Cliente HTTP para pruebas de endpoints
    ├── Dockerfile            # Compilación multicapa de .NET 10
    └── .dockerignore         # Exclusión de binarios y temporales
