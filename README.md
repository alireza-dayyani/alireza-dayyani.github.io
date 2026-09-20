# Alireza Dayyani — Backend Engineering Portfolio

A portfolio built as a small .NET 10 application: a typed ASP.NET Core API serves resume content to a responsive HTML, CSS, JavaScript, and Bootstrap frontend.

## What this repository demonstrates

- Minimal APIs with typed results and grouped endpoints
- Dependency injection and a JSON-backed content repository
- Output caching, response compression, rate limiting, and problem details
- A bounded `Channel<T>` contact queue processed by a `BackgroundService`
- Health and OpenAPI endpoints
- Unit tests for content integrity and request validation
- A framework-free frontend that consumes the API and falls back to static JSON on GitHub Pages
- Container and CI support

## Run locally

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
dotnet restore AlirezaDayyani.Portfolio.slnx
dotnet run --project src/Portfolio.Api/Portfolio.Api.csproj --launch-profile http
```

Open `http://localhost:5226`.

Useful endpoints:

- `GET /api/portfolio` — complete portfolio content
- `GET /api/projects` — selected engineering projects
- `POST /api/contact` — validated, rate-limited contact queue
- `GET /health` — application health
- `GET /openapi/v1.json` — OpenAPI document

The contact pipeline is intentionally a demo integration: requests are validated and processed by the in-process worker, but no email provider or durable message store is configured. The public page always includes a direct email link.

## Test

```powershell
dotnet test AlirezaDayyani.Portfolio.slnx
```

## Run with Docker

```powershell
docker build -t alireza-portfolio .
docker run --rm -p 8080:8080 alireza-portfolio
```

Open `http://localhost:8080`.

## Edit content and presentation

- Resume data: `src/Portfolio.Api/wwwroot/data/portfolio.json`
- Page structure: `src/Portfolio.Api/wwwroot/index.html`
- Visual system: `src/Portfolio.Api/wwwroot/css/portfolio.css`
- API integration and rendering: `src/Portfolio.Api/wwwroot/js/app.js`
- Backend endpoints: `src/Portfolio.Api/Endpoints/PortfolioEndpoints.cs`

Set `linkedInUrl` in `portfolio.json` when the final profile URL is available; the LinkedIn link remains hidden while that value is `null`.

## Deployment modes

The Pages workflow publishes `wwwroot` as a static site. In that mode, the frontend automatically reads `data/portfolio.json`, the API-only footer links are hidden, and contact remains available through the direct email link.

Deploy the Docker image or the published .NET application to a service that supports ASP.NET Core when the live API, health check, OpenAPI document, and contact endpoint are required.
