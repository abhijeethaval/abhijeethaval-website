# Iteration 01 - Persistence Foundation

## Goal

Add durable persistence for users, articles, published article revisions, and comments.

## Recommended Architecture

Use Entity Framework Core with PostgreSQL for production shape and Aspire-managed local
resources. SQLite is acceptable only as a temporary local bridge if PostgreSQL setup slows
the first iteration.

| Option | Recommendation | Reason |
|---|---|---|
| PostgreSQL | Preferred | Matches production SaaS patterns and supports JSON/search evolution. |
| SQLite | Temporary only | Useful for fast local work but weaker production signal. |
| Files | Reject | Not durable in container deployment and weak for auth/comment state. |

## Backend Work

- Add persistence packages to `AbhijeetSite.Api`.
- Add database registration in `Program.cs`.
- Add an `AppDbContext` or feature-owned persistence context with explicit entity sets.
- Add migrations and startup migration guidance.
- Add typed statuses:
  - `ArticleDraftStatus`: `Draft`, `ReadyToPublish`, `Archived`
  - `PublishedArticleStatus`: `Published`, `Unpublished`
  - `CommentStatus`: `Pending`, `Approved`, `Rejected`, `Deleted`
- Add integration-test database strategy.

## Initial Schema

| Table | Key Fields |
|---|---|
| `Users` | `Id`, `DisplayName`, `Email`, `AvatarUrl`, `CreatedAt`, `LastSignedInAt`, `IsAdmin` |
| `ExternalLogins` | `Id`, `UserId`, `Provider`, `ProviderSubject`, `EmailAtLogin` |
| `ArticleDrafts` | `Id`, `Title`, `Slug`, `Summary`, `MdxSource`, `Status`, timestamps |
| `PublishedArticles` | `Id`, `DraftId`, `Slug`, `Title`, `Summary`, `RenderedHtml`, `PublishedAt`, `Revision` |
| `Comments` | `Id`, `ArticleId`, `UserId`, `Body`, `Status`, timestamps, moderation fields |

## Frontend Work

- No major UI feature work in this iteration.
- Add API DTO types only if needed for smoke endpoints.

## Tests

- Migration can create schema.
- Repository or endpoint smoke test can insert and read a draft.
- Slug uniqueness is enforced.
- Invalid statuses cannot enter the domain model.

## Acceptance Criteria

- API starts with database configuration.
- Integration tests run against a disposable or isolated test database.
- No draft/article/comment state is stored in process memory.

## Risks

- EF Core package versions must match the repo target framework.
- Aspire database wiring and ACA production configuration must be kept separate.
- Automatic migrations in production should be an explicit deployment decision, not a hidden startup side effect.
