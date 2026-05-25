# AbhijeetSite - Personal Website Foundation (Step 1)

This is the foundation slice of a personal website built with **.NET Aspire**, **ASP.NET Core Minimal APIs**, **React/Vite (TypeScript)**, and **Vertical Slice Architecture**.

It is designed as a minimal, empty but working foundation, avoiding CQRS boilerplate, Entity Framework, databases, or complex abstractions.

---

## Prerequisites

Before running the application, ensure you have the following installed:
1. **.NET 9 SDK** (or higher)
2. **Node.js** (v20.5.0 or higher) and **NPM**
3. **.NET Aspire Workload** (install via `dotnet workload install aspire`)

---

## How to Run

To run the entire solution (both backend API and frontend Web client orchestrated by Aspire):

```bash
dotnet run --project src/AbhijeetSite.AppHost
```

Once running, the terminal will output the link to the **.NET Aspire Dashboard** (e.g. `https://localhost:17276`). Access it in your browser to view:
- Running API project resources.
- Running React/Vite project resources.
- Console logs, traces, and metrics.

---

## Project Structure

The project has the following structure:

```text
/src
  /AbhijeetSite.AppHost          # .NET Aspire orchestration engine
  /AbhijeetSite.ServiceDefaults  # Shared telemetry, logging, metrics & health checks
  /AbhijeetSite.Api              # ASP.NET Core Minimal API backend
  /AbhijeetSite.Web              # React + Vite + TypeScript frontend
/tests
  /AbhijeetSite.Api.Tests        # xUnit API Integration tests
```

---

## Vertical Slice Architecture in this Solution

Vertical Slice Architecture means organizing the codebase around **features** (what the system does) rather than technical layers (how the system does it, e.g., controllers, services, repositories separated into global folders).

- **Backend API (`/src/AbhijeetSite.Api/Features/Home`)**:
  - The home feature contains all files related to the home summary endpoint in a single folder.
  - `GetHomeSummaryEndpoint.cs`: Mappings and endpoint handler logic.
  - `HomeSummaryResponse.cs`: Typed DTO response structure.
  - Extension registration is called in `Program.cs` via `app.MapHomeEndpoints();`, ensuring Program.cs only does high-level routing setup.

- **Frontend React (`/src/AbhijeetSite.Web/src/features/home`)**:
  - Contains components, state, types, and api services related strictly to the home feature:
    - `HomePage.tsx`: Renders the landing panel, handles loading/error states.
    - `homeApi.ts`: Communicates with backend using the shared API client.
    - `types.ts`: Local DTO types.

---

## Current Scope

- **Backend**:
  - Exposes `GET /api/home/summary` returning the Principal AI Architect details.
  - Configures Swagger UI for endpoint discovery.
  - Implements basic health checks `/health` and `/alive`.
- **Frontend**:
  - Simple, clean single-page site with modern visual design (glassmorphic profile card, glowing ambient background gradients, responsive layouts, custom fonts).
  - Fetches details from API and displays them dynamically.
  - Handles loading and network error states with a retry mechanism.
- **Testing**:
  - Integration tests using `WebApplicationFactory<Program>` validating that `/api/home/summary` returns `HTTP 200` with the correct JSON values.

---

## Next Steps

- **Step 2**: Introduce a persistence layer (Entity Framework Core and SQLite/PostgreSQL database containers inside Aspire).
- **Step 3**: Implement dynamic content editing and a blog/article domain slice.

---

## Deployment to Azure Container Apps

This solution is prepared for containerized deployment to **Azure Container Apps (ACA)** using **Azure Container Registry (ACR)**.

### Architectural Topology

- **Public Frontend (`abhijeetsite-web`)**:
  - Exposes port `80` (public ingress).
  - Built using a multi-stage Docker build: a Node.js stage compiles the static React/Vite assets, and an Nginx stage hosts them.
  - The Nginx server serves the static assets and handles a SPA fallback router (`index.html`).
  - At container startup, an entrypoint script performs environment variable substitution (via `envsubst`) to dynamically inject the `API_UPSTREAM` URL into the Nginx configuration.
  - The browser calls relative `/api/*` endpoints on the web origin, and Nginx reverse-proxies them to the internal API container app.
- **Internal API (`abhijeetsite-api`)**:
  - Exposes port `8080` (internal ingress). Only accessible within the Container Apps Environment.
  - Built **without a Dockerfile** using **.NET SDK container publishing** (`/t:PublishContainer`).
  - Container-ready properties (repository name and exposed ports) are configured directly in the `AbhijeetSite.Api.csproj` project file.

### Prerequisites

- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) installed.
- Access to an Azure Subscription with permissions to create Resource Groups, Managed Identities, ACRs, and ACA environments.

### Local CLI Deployment Flow

> [!NOTE]
> The Azure resources (Resource Group, ACR, ACA Environment, User-Assigned Managed Identity with `AcrPull` permission, and both Container Apps themselves) are assumed to have been created beforehand (e.g. manually via the Azure Portal or through Infrastructure as Code (IaC) templates).

Deployment scripts are provided in the `/deploy` folder to build and push container images and update the Container Apps.

For **Windows (PowerShell)**:
1. **Azure Login**
   ```powershell
   az login
   ```
2. **Build and Push Container Images**
   ```powershell
   .\deploy\build-and-push-images.ps1 [optional-tag]
   ```
3. **Update Existing Container Apps**
   ```powershell
   .\deploy\update-container-apps.ps1 [optional-tag]
   ```

For **Linux / macOS / Git Bash**:
1. **Azure Login**
   ```bash
   az login
   ```
2. **Build and Push Container Images**
   ```bash
   ./deploy/build-and-push-images.sh [optional-tag]
   ```
3. **Update Existing Container Apps**
   ```bash
   ./deploy/update-container-apps.sh [optional-tag]
   ```

---

### GitHub Actions CI/CD Pipeline

A workflow is available in `.github/workflows/deploy-aca.yml` to automatically build, push, and deploy new releases on pushes to the `main` branch.

#### Configuration Required

1. **GitHub Secrets**:
   - `AZURE_CLIENT_ID`: The application (client) ID of a Microsoft Entra ID App Registration configured with Federated Credentials for OIDC.
   - `AZURE_TENANT_ID`: The directory (tenant) ID of your Entra ID tenant.
   - `AZURE_SUBSCRIPTION_ID`: The Azure Subscription ID.

2. **GitHub Variables**:
   - `RESOURCE_GROUP`: `rg-abhijeetsite-dev`
   - `ACR_NAME`: `acrabhijeetsitedev`
   - `ACR_LOGIN_SERVER`: `acrabhijeetsitedev.azurecr.io`
   - `API_APP`: `abhijeetsite-api`
   - `WEB_APP`: `abhijeetsite-web`
