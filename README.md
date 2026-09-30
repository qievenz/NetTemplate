# NetTemplate

.NET 9 Web API Clean Architecture Template


## **1. Agregar una Migración con EF Core**

Crea una nueva migración de base de datos basada en los cambios de tu modelo.
Asegúrate de ejecutar este comando desde la **raíz de tu proyecto** (donde se encuentra el archivo `.sln`).
Hay que realizar una migracion luego de cada cambio que se haga en el esquema.

```bash
dotnet ef migrations add [NombreDeTuMigracion] -p src\NetTemplate.Infrastructure -s src\NetTemplate.API
```

## **2. Levantar los Servicios (Build & Run)**

Este comando detiene y elimina los contenedores anteriores, reconstruye las imágenes (si hay cambios en el Dockerfile o el contexto) y luego levanta todos los servicios en modo *detached* (en segundo plano).


```bash
docker-compose -p nettemplate down ;
docker-compose -p nettemplate up --build -d
```

---

## Quick Access Links

* **API Health Check:** [http://localhost:5000/health](http://localhost:5000/health)
* **API Swagger UI:** [http://localhost:5000/swagger](http://localhost:5000/swagger)


