# Abhijeet Haval Website

Personal website for Abhijeet Haval, implemented as a full-stack .NET Aspire application:

- **API**: ASP.NET Core Minimal APIs on `net10.0`
- **Web**: React 18 + Vite + TypeScript
- **Orchestration**: .NET Aspire AppHost with service defaults for health, telemetry, and discovery
- **Deployment**: Azure Container Apps with ACR-hosted container images and GitHub Actions OIDC

The current production-facing capability is a curated professional profile site. The planned next capability is a publishing platform for authenticated article drafting, MDX publishing, and moderated comments.

---

## Current Reality

| Area | Current state |
|---|---|
| Public experience | Single-page profile site with hero, about, experience, and education sections. |
| API surface | `GET /api/profile` for the full profile and `GET /api/home/summary` for backward-compatible summary data. |
| Frontend data flow | React loads `/api/profile` through a relative API client. Vite proxies `/api` during local development; Nginx proxies `/api` in Azure. |
| Backend data source | Curated in-process profile content in `ProfileContentProvider`; PostgreSQL persistence baseline for publishing modules. |
| Observability | Aspire service defaults, OpenTelemetry wiring, health endpoints, and development OpenAPI/Scalar UI. |
| Persistence/auth/articles | Persistence foundation implemented for identity, articles, and comments; auth and public publishing workflows remain planned. |
| Deployment | Containerized API and Web images deployed to Azure Container Apps. |

---

## Repository Layout

```text
src/
  AbhijeetSite.AppHost/          .NET Aspire orchestration host
  AbhijeetSite.ServiceDefaults/  Shared health, telemetry, resilience, and discovery defaults
  AbhijeetSite.Api/              ASP.NET Core Minimal API backend
  AbhijeetSite.Web/              React + Vite + TypeScript frontend
tests/
  AbhijeetSite.Api.Tests/        xUnit integration tests for API endpoints
deploy/
  build-and-push-images.ps1      Azure ACR cloud-build helper for API and Web images
docs/
  deploy-github-actions.md       Azure Container Apps and GitHub Actions setup guide
  implementation-plan/           Iterative roadmap for persistence, auth, articles, and comments
.github/workflows/
  publish-images.yml             Publishes images on pushes to main or manual trigger
  deploy-aca.yml                 Manually deploys an image tag to Azure Container Apps
```

---

## Prerequisites

| Dependency | Version / note |
|---|---|
| .NET SDK | `10.0.x` |
| .NET Aspire workload | Required for `AbhijeetSite.AppHost` |
| Container runtime | Required for Aspire-managed PostgreSQL and Testcontainers-backed integration tests |
| Node.js | Node 20+ recommended |
| npm | Required for `src/AbhijeetSite.Web` |
| Azure CLI | Required only for Azure deployment workflows from a local machine |

Install Aspire if it is not already available:

```powershell
dotnet workload install aspire
```

Install frontend dependencies:

```powershell
cd src\AbhijeetSite.Web
npm ci
```

---

## Run Locally

Run the full app through Aspire:

```powershell
dotnet run --project src\AbhijeetSite.AppHost
```

Aspire starts:

- API service named `api`
- Vite development server named `web`
- PostgreSQL server named `postgres`
- PostgreSQL database named `abhijeetsite-db`
- Aspire dashboard with logs, health, traces, and resource endpoints

The AppHost passes the API endpoint to Vite through `VITE_API_URL`, and `vite.config.ts` proxies `/api` to that backend.

Run individual services when debugging a narrower surface:

```powershell
dotnet run --project src\AbhijeetSite.Api
```

```powershell
cd src\AbhijeetSite.Web
npm run dev
```

---

## API Endpoints

| Endpoint | Purpose |
|---|---|
| `GET /api/profile` | Full curated profile: headline, summary, about text, expertise, experience, and education. |
| `GET /api/home/summary` | Compatibility summary derived from the same profile content source. |
| `/health` | Health probe endpoint. |
| `/alive` | Liveness probe endpoint. |

In development, OpenAPI and Scalar are mapped by the API project.

---

## Validation

Run backend tests:

```powershell
dotnet test
```

The API integration tests use Testcontainers PostgreSQL. Tests that require Docker are skipped when a container runtime is not available.

Run frontend checks:

```powershell
cd src\AbhijeetSite.Web
npm run build
npm run lint
```

The solution file currently includes the API, AppHost, ServiceDefaults, and API test projects. The React project is a Vite/npm project under `src/AbhijeetSite.Web`, so validate it with npm scripts.

---

## Architecture Notes

| Decision | Current implementation | Tradeoff |
|---|---|---|
| Modular monolith with feature slices | Runtime code is grouped by module under `Features`: profile/home today, with identity, articles, and comments persistence modules started for publishing. | Makes boundaries visible while keeping one deployable unit. |
| In-process profile content | `ProfileContentProvider` owns curated profile data. | Fast and simple for a personal profile; persistence becomes necessary for articles, editing, comments, or admin workflows. |
| PostgreSQL persistence foundation | Aspire wires PostgreSQL locally; EF Core maps identity, articles, and comments to separate schemas through one `AppDbContext`. | Demonstrates module boundaries without prematurely splitting services or adding repository abstractions. |
| Relative frontend API calls | Browser calls `/api/*`; local Vite and production Nginx provide the proxy. | Avoids browser-visible API host configuration and aligns local/prod routing. |
| API-owned future auth | Planned in `docs/implementation-plan/iteration-02-external-login.md`. | Keeps OAuth secrets and callback validation out of React. |
| Published read model for future articles | Planned in `docs/implementation-plan/iteration-04-publishing-pipeline.md`. | Separates public reads from draft/admin workflows. |

---

## Deployment

The deployed topology is:

| Component | Azure role |
|---|---|
| `abhijeetsite-web` | Public Azure Container App running Nginx and static Vite assets on port `80`. |
| `abhijeetsite-api` | Internal Azure Container App running ASP.NET Core on port `8080`. |
| Azure Container Registry | Stores API and Web images. |
| Nginx `API_UPSTREAM` | Runtime environment variable pointing the Web app to the internal API FQDN. |

Local cloud-build helper:

```powershell
.\deploy\build-and-push-images.ps1 [optional-image-tag]
```

GitHub Actions workflows:

| Workflow | Trigger | Responsibility |
|---|---|---|
| `publish-images.yml` | Push to `main` or manual trigger | Build and publish API/Web images to ACR. |
| `deploy-aca.yml` | Manual trigger | Deploy a supplied image tag to Azure Container Apps. |

Required GitHub secrets:

| Secret | Purpose |
|---|---|
| `AZURE_CLIENT_ID` | Entra application client ID for OIDC login. |
| `AZURE_TENANT_ID` | Entra tenant ID. |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription ID. |

Required GitHub variables:

| Variable | Purpose |
|---|---|
| `RESOURCE_GROUP` | Azure resource group. |
| `ACR_NAME` | Azure Container Registry name. |
| `ACR_LOGIN_SERVER` | Registry login server. |
| `WEB_APP` | Web image repository name. |
| `API_APP` | Optional API Container App override used by deployment workflow. |
| `WEB_APP_CONTAINER_NAME` | Optional Web Container App override used by deployment workflow. |

See `docs/deploy-github-actions.md` for the detailed Azure setup, OIDC trust configuration, Nginx HTTPS upstream behavior, custom domain notes, and troubleshooting.

---

## Roadmap

The publishing-platform plan is tracked in `docs/implementation-plan/`:

| Iteration | Outcome |
|---|---|
| 00 | Architecture baseline and cross-cutting decisions. |
| 01 | Persistence foundation. |
| 02 | External login. |
| 03 | MDX article drafting and preview. |
| 04 | Publishing pipeline. |
| 05 | Authenticated comments with moderation. |
| 06 | LinkedIn integration and production hardening. |

Until the remaining iterations land, this repository should be treated as a deployed personal profile site plus a persistence foundation for the larger publishing platform.
