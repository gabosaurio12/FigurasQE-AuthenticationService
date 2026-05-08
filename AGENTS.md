# AGENTS.md

Scope: applies to the `FigurasQE-AuthenticationService` project.

## Purpose
REST API de autenticación (.NET 9, MVC). Emite tokens JWT usados por `MicroservicioFiguras` para autorizar acceso.

## Run And Validate
```
dotnet restore
dotnet build
dotnet run --launch-profile http
```
- Swagger disponible en desarrollo: `GET /openapi/v1.json`
- No hay proyecto de tests actualmente.

## Architecture Map

| Carpeta / Archivo | Rol |
|---|---|
| [Controllers/AuthController.cs](Controllers/AuthController.cs) | Único controlador; expone `POST /auth/login` y `POST /auth/register` |
| [Services/AuthService.cs](Services/AuthService.cs) | Orquesta registro/login; delega hash en BCrypt y token en JwtService |
| [Services/JwtService.cs](Services/JwtService.cs) | Genera el JWT con la librería `JWT` (no `Microsoft.AspNetCore.Authentication.JwtBearer`) |
| [Data/Repositories/UserRepository.cs](Data/Repositories/UserRepository.cs) | Busca/crea usuarios en las tablas `Students` y `Tutors` |
| [Data/FigurasqeContext.cs](Data/FigurasqeContext.cs) | DbContext compartido con la misma BD que `MicroservicioFiguras` |
| [Data/Entities/](Data/Entities/) | Entidades EF Core (Student, Tutor, etc.) — espejo de los modelos del dominio |
| [Models/](Models/) | Request/response models: `LoginRequest`, `RegisterRequest`, `AuthUser`, `AuthResponse` |

## Endpoints Expuestos

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/auth/login` | Valida credenciales, devuelve `{ token }` |
| `POST` | `/auth/register` | Crea usuario (student o tutor), devuelve `{ token }` |

**Errores**: `401 { error }` en login fallido. `Signup` actualmente no devuelve error HTTP si el email ya existe (escribe igualmente y devuelve token) — WIP conocido.

## Formato Del Token JWT

Emitido por `JwtService.EncodeToken()`. Payload:

```json
{
  "sub":   "<IdStudent | IdTutor>",
  "email": "user@example.com",
  "role":  "student | tutor",
  "iss":   "figurasqe-auth",
  "aud":   "figurasqe-client",
  "iat":   <unix>,
  "exp":   <iat + 1h>
}
```

> Estos nombres de claim (`sub`, `role`) son los que `MicroservicioFiguras` espera en `JwtClaimsHelper`.

## Configuración Requerida (`appsettings.json`)

```json
"ConnectionStrings": { "DefaultConnection": "<postgres-conn-string>" },
"Jwt": {
  "Issuer":   "figurasqe-auth",
  "Audience": "figurasqe-client",
  "Secret":   "<min-32-chars>"
}
```

> El `Secret`, `Issuer` y `Audience` deben coincidir exactamente con los valores configurados en `MicroservicioFiguras/appsettings.json`.

## Convenciones De Código

- Estilo **MVC con controladores**; no usar Minimal API aquí.
- La lógica de negocio va en `Services/`; el acceso a datos va en `Data/Repositories/`.
- Passwords se hashean con `BCrypt.Net.BCrypt.HashPassword()` antes de persistir.
- La librería JWT usada es `JWT` (NuGet `JWT` v11), **no** `System.IdentityModel.Tokens.Jwt`. Usar `JwtEncoder` / `IJwtAlgorithm`.
- `RegisterRequest.Role` acepta solo `"student"` o `"tutor"` (validado por `[RegularExpression]`).
- `RegisterRequest.Genre` acepta solo `M`, `F` o `O`.
- `Country` es código ISO de 2 letras (ej. `MX`, `US`).

## Known Issues / WIP

- `Signup` no retorna `400` si el email ya está registrado — simplemente no inserta y devuelve token de todas formas.
- `ExpirationMinutes: 10080` en config está ignorado; la expiración real está hardcodeada en `JwtService` a 1 hora (`AddHours(1)`).
- No hay endpoint `DELETE /auth/logout` ni revocación de tokens.
- No hay `[Authorize]` en ningún endpoint de este servicio — todos son públicos por diseño.
