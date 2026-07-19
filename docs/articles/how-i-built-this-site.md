# How I Built This Site as a Modular Monolith

This site started as a personal website, but I did not want it to behave like a
static resume with a contact link bolted on. I wanted the site itself to show how
I think about product engineering: clear boundaries, durable state, observable
runtime behavior, and deployment choices that can survive beyond a demo.

The result is a full-stack application built around a modular monolith, vertical
feature slices, a React frontend, an ASP.NET Core API, PostgreSQL persistence,
and deployment on Azure Container Apps.

## The design goal

The first decision was to make the site a working product surface, not only a
portfolio page. That changed the architecture immediately.

The site needs to do a few things well today:

- Present a curated profile and architecture page.
- Serve published articles from durable storage.
- Keep browser traffic on a simple same-origin path.
- Support authenticated owner workflows without pushing secrets into the frontend.
- Leave room for comments, richer publishing, and external login providers.

Those requirements do not justify microservices. They do justify explicit module
boundaries. A distributed architecture would add deployment, networking, tracing,
and failure-mode overhead before the product needs independent services. A
modular monolith keeps one runtime boundary while still making ownership visible
in the code.

## Current architecture

The application has four main runtime pieces:

- React and Vite for the public web experience.
- ASP.NET Core Minimal APIs for profile, auth, articles, and admin workflows.
- PostgreSQL for identity, article, comment, and publishing state.
- .NET Aspire for local orchestration, service defaults, telemetry, health checks,
  and PostgreSQL composition.

In production, Azure Container Apps provides the operating boundary:

- The web container is public and serves the built React app through Nginx.
- The API container runs ASP.NET Core and listens behind the web app.
- Nginx proxies browser `/api` requests to the API through `API_UPSTREAM`.
- Azure Container Apps ingress handles the browser-facing TLS edge.
- The API owns persistence, auth, cookies, and provider secrets.

The practical request flow is:

```text
Browser
Web Container App
Nginx static files or /api proxy
API Container App
PostgreSQL
```

Locally, Aspire composes the same shape with a developer-friendly loop. The
AppHost starts the API, the Vite web app, and PostgreSQL, then passes the API
endpoint to the web app through `VITE_API_URL`. In Azure, the same browser path
is kept stable with relative `/api` calls and an Nginx upstream.

## Why a modular monolith

The backend is organized as a modular monolith because the domain is still one
product. The important boundary is not process isolation yet. The important
boundary is ownership.

The current modules are:

- Profile, which owns curated professional content for the profile and home views.
- Identity, which owns users, external login mappings, sessions, and admin policy.
- Articles, which owns drafts, slugs, publishing transitions, and public reads.
- Comments, which owns comment text, moderation state, and approved public output.
- SharedKernel, which stays deliberately small and contains cross-cutting
  primitives such as `Result` and the application clock abstraction.

Each module owns its domain model, persistence mapping, endpoints, workflows, and
tests. Cross-module relationships use scalar IDs instead of shared mutable domain
objects. That keeps the model honest: a comment can reference a user or a
published article, but it does not get to reach across the boundary and mutate
identity or publishing behavior.

This is the part I care about most: a monolith does not have to mean a pile of
coupled code. The bad version of a monolith is an application where every feature
can reach into every other feature. The useful version is one deployable unit
with internal contracts that are strict enough to delay service extraction until
there is a real operational reason.

## Vertical slices instead of broad layers

The code is sliced by behavior first. A feature workflow such as creating,
updating, or publishing an article lives near the article model, article errors,
article response contracts, and article tests. That keeps the change surface
small when a product behavior changes.

I intentionally avoided generic repositories and broad application services. EF
Core is already the persistence abstraction at this scale, and wrapping it with a
repository that exposes the same query behavior would add ceremony without
protecting a boundary.

The application handlers use the database context directly, but they still
respect the module boundaries:

- Business failures return typed `Result` values.
- Infrastructure failures are caught at the boundary and mapped to explicit
  errors.
- Timestamps come from an injected application clock.
- Article slugs and IDs are value objects owned by the Articles module.
- Public endpoints map application results to HTTP responses at the transport
  edge.

That gives the site a straightforward use-case structure without hiding the real
business decisions behind framework indirection.

## Publishing model

The article system is designed around a separation between authoring and public
reading.

Drafts keep the source content. Published articles keep render-ready public
content. The public site reads only published records in the `Published` state.
That means reader traffic does not depend on compiling or interpreting draft
source at request time.

The current publishing flow is intentionally conservative:

- The owner creates or updates a durable draft.
- The draft carries a version for optimistic concurrency.
- Publishing validates and renders the approved source.
- The public read model is created or updated as a published revision.
- Editing a draft after publish does not change public content until republish.

The renderer currently supports a constrained Markdown subset and rejects raw
HTML, JSX, imports, exports, and expression syntax. That is a deliberate first
slice. The system can grow toward richer MDX components later, but the public
reader path starts from a safe default.

## Persistence boundaries

The database shape follows the same modular-monolith approach. The application
uses one PostgreSQL database and one `AppDbContext`, with module-owned schemas:

- `identity` for users and external login mappings.
- `articles` for drafts and published article read models.
- `comments` for comment and moderation state.

This keeps transactions simple while still making ownership visible in the
schema. Separate EF contexts or separate databases would be premature until a
module needs an independent transaction boundary, scaling profile, or deployment
cadence.

The site also avoids using the container filesystem for runtime content. That is
important on Azure Container Apps because containers can be replaced, scaled, or
restarted. Drafts, published articles, login mappings, and comments need durable
application storage, not files written inside a running container.

## Deployment on Azure Container Apps

The production shape is two container apps:

- A public web app that serves React assets with Nginx on port `80`.
- An API app that runs ASP.NET Core on port `8080`.

The web app is the browser-facing entry point. It serves static assets directly
and proxies `/api` to the API app. Azure Container Apps ingress terminates public
TLS before traffic reaches the web container. Inside the web container, Nginx is
responsible for static files, SPA fallback, security headers, and API proxying.

The deployment pipeline is split into two GitHub Actions workflows:

- One workflow builds and publishes API and web container images to Azure
  Container Registry.
- Another workflow deploys a selected image tag to Azure Container Apps.

GitHub authenticates to Azure with OpenID Connect, so deployment does not depend
on a long-lived service principal secret stored in the repository. The web
container receives `API_UPSTREAM` as runtime configuration, while PostgreSQL
connection settings belong to the API container.

This split is important. The frontend should not know how to connect to the
database. The browser should not need a public API host. The API should own
business state and secrets. The web container should own static delivery and
same-origin routing.

## What is already working

The current site is already more than a profile page:

- The home and profile pages load curated backend-owned profile content.
- The architecture page explains the system that runs the site.
- Public article list and detail pages read from published article records.
- Google login scaffolding and session endpoints live in the API.
- Admin article draft, update, and publish workflows exist behind an admin policy.
- PostgreSQL persistence is wired for identity, articles, and comments.
- Integration tests exercise the API and database-backed publishing behavior.
- The web and API containers are structured for Azure Container Apps deployment.

That gives the site a real product baseline. It can publish content from durable
state, not only from checked-in static files.

## Future plans

The next phase is to deepen the publishing platform without losing the simplicity
of the current architecture.

The near-term roadmap is:

- Improve the owner drafting experience with preview, validation feedback, and
  safer editing ergonomics.
- Expand the publishing pipeline with richer article components while preserving
  the safe public read model.
- Add authenticated comments with moderation before public visibility.
- Complete external login hardening and add LinkedIn where it adds product value.
- Strengthen production operations around migrations, observability, backups,
  and private database networking.
- Keep architecture tests enforcing module boundaries as the codebase grows.

The extraction rule is simple: keep the modular monolith until a module has a
real reason to move. Independent scale, independent release cadence, a separate
team boundary, or a materially different reliability profile would justify a
service boundary. Until then, service extraction would be theater.

## The main lesson

The most useful architecture choice in this site is not Azure, React, ASP.NET
Core, or PostgreSQL in isolation. It is the decision to make boundaries explicit
without making operations unnecessarily complex.

A personal site can be a static brochure. This one is becoming a small publishing
platform. The architecture reflects that: one deployable product, vertical
slices, durable state, a clear public read model, and a deployment path that
matches how the application actually runs.
