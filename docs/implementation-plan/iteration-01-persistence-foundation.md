# Iteration 01 - Persistence Foundation

## Goal

Add durable persistence for users, articles, published article revisions, and comments.

## Recommended Architecture

Use Entity Framework Core with PostgreSQL from the start. Local development should use
Aspire-managed PostgreSQL resources, integration tests should use Testcontainers
PostgreSQL, and production should use externally managed PostgreSQL-compatible
infrastructure.

| Option | Recommendation | Reason |
|---|---|---|
| PostgreSQL | Required | Matches production shape and supports JSON/search evolution. |
| SQLite | Reject | Creates a second persistence shape before auth, publishing, and moderation depend on database behavior. |
| Files | Reject | Not durable in container deployment and weak for auth/comment state. |

## Backend Work

- Add persistence packages to `AbhijeetSite.Api`.
- Add database registration in `Program.cs`.
- Add Aspire PostgreSQL resource registration in `AbhijeetSite.AppHost`.
- Use Aspire-managed PostgreSQL for local application runtime.
- Use Testcontainers PostgreSQL for integration tests.
- Add one `AppDbContext` with module-owned entity configuration files.
- Use `AppDbContext` directly from application feature handlers.
- Do not add repository abstractions in iteration 01.
- Implement application workflows as plain per-use-case feature handler classes.
- Do not add broad module services for unrelated workflows.
- Do not add MediatR in iteration 01.
- Map module tables to PostgreSQL schemas:
  - `identity`
  - `articles`
  - `comments`
- Add strongly typed IDs backed by `Guid`:
  - `UserId`
  - `ExternalLoginId`
  - `ArticleDraftId`
  - `PublishedArticleId`
  - `CommentId`
- Keep strongly typed IDs in their owning modules.
- Add EF value converters for strongly typed IDs.
- Generate entity IDs in application/domain code before persistence.
- Add an injected application clock for business timestamps.
- Use the application clock for `CreatedAt`, `UpdatedAt`, `PublishedAt`,
  `LastSignedInAt`, and moderation timestamps.
- Use scalar IDs only for cross-module references.
- Model entities with private setters and explicit factory/transition methods.
- Add `Result`-based domain operation outcomes for expected business failures.
- Add shared kernel primitives for `Result`, base errors, and application time.
- Keep concrete domain errors inside their owning modules.
- Reject bidirectional module references unless a design review accepts the dependency.
- Add `NetArchTest.Rules` architecture tests for module dependency direction.
- Add one narrow test-only `CreateArticleDraft` path below HTTP that exercises:
  - typed IDs
  - application clock
  - `Result` outcomes
  - EF value conversion
  - PostgreSQL schema mapping
- Add migrations and explicit migration execution guidance.
- Do not apply migrations automatically during production API startup.
- Allow explicit migration application in local development and tests.
- Add typed statuses:
  - `ArticleDraftStatus`: `Draft`, `ReadyToPublish`, `Archived`
  - `PublishedArticleStatus`: `Published`, `Unpublished`
  - `CommentStatus`: `Pending`, `Approved`, `Rejected`, `Deleted`
- Add integration-test database strategy based on Testcontainers.

## Initial Schema

| Table | Key Fields |
|---|---|
| `identity.Users` | `Id`, `DisplayName`, `Email`, `AvatarUrl`, `CreatedAt`, `LastSignedInAt`, `IsAdmin` |
| `identity.ExternalLogins` | `Id`, `UserId`, `Provider`, `ProviderSubject`, `EmailAtLogin` |
| `articles.ArticleDrafts` | `Id`, `Title`, `Slug`, `Summary`, `MdxSource`, `Status`, timestamps |
| `articles.PublishedArticles` | `Id`, `DraftId`, `Slug`, `Title`, `Summary`, `RenderedHtml`, `PublishedAt`, `Revision` |
| `comments.Comments` | `Id`, `ArticleId`, `UserId`, `Body`, `Status`, timestamps, moderation fields |

## Frontend Work

- No major UI feature work in this iteration.
- Do not add public or temporary smoke endpoints.

## CreateArticleDraft Path

Add the smallest useful vertical path in the `Articles` module:

1. Create an `ArticleDraft` through a domain factory.
2. Persist it through an application feature handler using `AppDbContext`.
3. Read it back by `ArticleDraftId` or slug.
4. Return a typed success or domain error result.

This path is not the full admin drafting workflow from iteration 03. It exists to prove
the persistence architecture, module layout, typed IDs, `Result`, and clock integration
under one executable slice. It should be exercised below HTTP by tests, not exposed as a
temporary API endpoint. Name the use case `CreateArticleDraft` so iteration 03 can extend
the real workflow instead of replacing a disposable smoke artifact.

Iteration 01 validates persistence-level invariants only:

- title is required
- slug is required and URL safe
- summary is required
- MDX source is non-empty
- slug is unique
- initial status is valid

Full MDX parsing, component policy validation, and compilation are deferred to iterations
03 and 04.

## Tests

- Integration tests run against an isolated Testcontainers PostgreSQL database.
- Migration can create schema.
- Production startup does not run migrations automatically.
- Repository/application smoke test can insert and read a draft through `CreateArticleDraft`.
- `CreateArticleDraft` returns typed validation errors for persistence-level invalid input.
- `CreateArticleDraft` does not parse or compile MDX.
- Slug uniqueness is enforced.
- Invalid statuses cannot enter the domain model.
- Module dependency rules are enforced by architecture tests.

## Acceptance Criteria

- API starts with database configuration.
- Integration tests run against a disposable or isolated test database.
- No draft/article/comment state is stored in process memory.

## Risks

- EF Core package versions must match the repo target framework.
- Aspire database wiring and ACA production configuration must be kept separate.
- Testcontainers requires Docker availability in local and CI environments.
- Production migrations require an explicit deployment or operator action.
