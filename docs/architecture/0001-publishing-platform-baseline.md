# 0001 - Publishing Platform Architecture Baseline

## Status

Accepted

## Context

The site is deployed as Azure Container Apps:

- `abhijeetsite-web` serves the React application through Nginx.
- `abhijeetsite-api` is an internal ASP.NET Core API.
- Nginx proxies browser `/api` requests to the internal API.

Container filesystems are not durable in this topology. Article drafts, published
articles, external login mappings, and comment moderation state must survive container
replacement and scale-out.

## Decision

Use durable application storage for authoring state and expose public article reads from a
published read model.

| Concern | Baseline decision | Consequence |
|---|---|---|
| Backend ownership | Modular monolith with module-owned feature slices. | Domain boundaries are visible without distributed-system overhead. |
| Frontend ownership | Module-aligned feature folders for `auth`, `articles`, `comments`, and `admin`. | Public and owner workflows can evolve without coupling to profile UI. |
| Persistence | PostgreSQL managed locally through Aspire, isolated in tests with Testcontainers, and externally in production. | One database shape covers local development, tests, and Azure deployment. |
| Draft source | Store MDX source as durable text on `ArticleDraft`. | Runtime authoring does not depend on checked-in files or container disk. |
| Public read model | Store render-ready `PublishedArticle` revisions. | Public reads avoid parsing drafts or executing untrusted MDX at request time. |
| Authentication | API-owned external login with secure cookies. | OAuth secrets and provider tokens stay out of React. |
| Authorization | Local role or claim policies. | Admin publishing is separated from authenticated commenting. |
| Comments | Plain text first; restricted Markdown only by later decision. | User content cannot execute MDX, HTML, scripts, or React components. |

## Module Architecture

The backend is a modular monolith. Modules own their domain model, persistence mapping,
endpoints, feature workflows, and tests. Features live inside modules when the behavior is
large enough to deserve a named workflow.

```text
SharedKernel/
  Result/
  Time/
Features/
  Identity/
    Login/
    Logout/
    CurrentUser/
    AdminAuthorization/
  Articles/
    DraftArticle/
    ValidateDraft/
    PublishArticle/
    ReadPublishedArticle/
  Comments/
    SubmitComment/
    ModerateComment/
    ReadApprovedComments/
```

Top-level modules are stable boundaries. Inner features are replaceable workflow slices.
Cross-module access should go through IDs, policies, events, or explicit application
contracts instead of shared mutable domain objects.

Shared kernel code must stay small and stable. It may contain primitives that are truly
cross-cutting, such as `Result`, `Error`, and the application clock abstraction. It must
not contain module-specific entities, services, validation rules, or error catalogs.
Domain-specific IDs are module contracts, not shared-kernel primitives.

## Domain Boundaries

| Boundary | Owns | Does not own |
|---|---|---|
| Identity | `User`, `ExternalLogin`, sign-in timestamps, admin authorization. | Provider tokens in React or provider-specific checks inside content features. |
| Articles | `ArticleDraft`, `PublishedArticle`, validation, publish transitions. | User-generated comments or external identity mapping. |
| Comments | `Comment`, moderation state, anti-abuse state. | Article publishing or local user provisioning. |

## Cross-Module References

Cross-module relationships use scalar IDs only. EF navigation properties are allowed
inside a module, but not across module boundaries.

| From | To | Reference |
|---|---|---|
| `identity.ExternalLogin` | `identity.User` | Same-module navigation allowed. |
| `articles.PublishedArticle` | `articles.ArticleDraft` | Same-module navigation allowed. |
| `comments.Comment` | `identity.User` | `UserId` only. |
| `comments.Comment` | `articles.PublishedArticle` | `PublishedArticleId` only. |

IDs should be strongly typed value objects backed by `Guid`, such as `UserId`,
`ArticleDraftId`, `PublishedArticleId`, and `CommentId`. Database columns remain UUIDs;
type safety exists in application code and EF mapping converts at the persistence boundary.
Application/domain code generates IDs before persistence. EF and PostgreSQL must not be
the source of identity for domain entities.

Strongly typed IDs are owned by the module that owns the concept. For example, `UserId`
lives in `Identity`, `PublishedArticleId` lives in `Articles`, and `CommentId` lives in
`Comments`. Shared kernel must not define domain-specific ID types.

Domain values with rules are module-owned value objects. `ArticleSlug` lives in the
`Articles` module because it carries URL identity, validation, and uniqueness semantics.
The database stores the slug as text, while EF converts at the persistence boundary.
Slug uniqueness is checked in application code to return typed duplicate-slug errors and
also enforced by a database unique index as the concurrency backstop.

Cross-module ID dependencies must be one-way. Bidirectional module references require a
design review and should usually be replaced by clearer ownership, an application
orchestration boundary, or a read model/projection.

Database foreign keys may cross module schemas to protect data integrity, but EF
navigation properties must not cross module boundaries. Cross-module delete behavior is
restricted by default; lifecycle changes should use explicit statuses such as `Archived`,
`Unpublished`, `Rejected`, or `Deleted`.

## Architecture Enforcement

Module boundaries should be enforced with `NetArchTest.Rules` architecture tests once the
modules exist. Conventions are acceptable while the repository only contains the baseline
plan, but implementation iterations must add executable checks for dependency direction.

| Rule | Enforcement target |
|---|---|
| `SharedKernel` does not reference feature modules. | `NetArchTest.Rules` test. |
| `Identity` does not reference `Articles` or `Comments`. | `NetArchTest.Rules` test. |
| `Articles` does not reference `Comments`. | `NetArchTest.Rules` test. |
| `Comments` may reference module-owned ID contracts from `Identity` and `Articles`. | Explicit allowlist. |
| Bidirectional module references fail by default. | `NetArchTest.Rules` test with documented exception path. |

Timestamps come from an injected application clock. Domain and application workflows must
not call wall-clock APIs directly, and PostgreSQL defaults must not be the primary source
of business timestamps.

## Entity Encapsulation

Entities must not expose public setters for EF convenience. Use private setters, private
constructors for EF materialization, and named factories or methods for domain state
transitions.

| Entity behavior | Pattern |
|---|---|
| Creation | Static factory or constructor that receives typed IDs and timestamps. |
| State transition | Named method such as `MarkReadyToPublish`, `Publish`, `Approve`, or `Reject`. |
| EF materialization | Private parameterless constructor and private setters. |
| Invalid transition | `Result` with an explicit typed domain error. |

## Domain Error Handling

Expected business rule failures return `Result` values with typed domain errors. Domain
and application code must not throw exceptions for expected validation or state-transition
failures.

| Error class | Examples | Handling |
|---|---|---|
| Validation error | Invalid slug, missing title, oversized MDX source. | Return `Result` with actionable validation error. |
| Business error | Duplicate slug, invalid status transition, non-publishable draft. | Return `Result` with typed domain error. |
| Infrastructure error | Database failure, provider callback failure, MDX compiler process failure. | Catch at infrastructure boundary and map to structured application error. |

`Result` and base error primitives live in `SharedKernel`. Concrete domain errors remain
module-owned, such as `ArticlesErrors.DuplicateSlug` or
`CommentsErrors.InvalidModerationTransition`.

HTTP endpoints map `Result` failures to `ProblemDetails` at the transport boundary.
Domain and application handlers should not know HTTP status codes.

## Canonical Names And States

| Type | Values |
|---|---|
| `ArticleDraftStatus` | `Draft`, `ReadyToPublish`, `Archived` |
| `PublishedArticleStatus` | `Published`, `Unpublished` |
| `CommentStatus` | `Pending`, `Approved`, `Rejected`, `Deleted` |
| External providers | `Google`, `LinkedIn` |
| Authorization policies | `AuthenticatedUser`, `AdminOnly` |

Statuses are stored as text with database check constraints rather than integer enum
ordinals. Text values keep persisted state readable and check constraints prevent invalid
raw database writes without creating PostgreSQL enum migration friction.

## Persistence Boundaries

Use one PostgreSQL database with module-owned schemas:

| Schema | Tables | Module |
|---|---|---|
| `identity` | `Users`, `ExternalLogins` | `Identity` |
| `articles` | `ArticleDrafts`, `PublishedArticles` | `Articles` |
| `comments` | `Comments` | `Comments` |

Use one `AppDbContext` initially, with entity configuration files owned by each module.
Separate EF contexts or separate databases are rejected until behavior requires independent
transaction boundaries.
Application feature handlers may use `AppDbContext` directly. Repository abstractions are
deferred until a real aggregate persistence abstraction appears; they must not wrap EF
only to hide EF.

Application workflows are implemented as plain per-use-case feature handler classes. Do
not introduce MediatR unless the application needs real pipeline behavior such as
cross-cutting validation, authorization, retries, or notifications that plain composition
cannot keep clear. Avoid broad module services that collect unrelated workflows.

Local application runtime uses Aspire-managed PostgreSQL. Integration tests use
Testcontainers PostgreSQL so each test run gets an isolated database without depending on
a manually running service.

Production API startup must not apply migrations automatically. Local development and
tests may apply migrations explicitly, but production schema changes are a deployment or
operator action to avoid hidden rollout failures and scale-out races.

Business timestamps are represented as UTC `DateTimeOffset` values in application code
and persisted using PostgreSQL `timestamp with time zone`. Database defaults may exist for
operational safety, but they are not the source of business time.

Iteration 01 does not add optimistic concurrency tokens because `CreateArticleDraft` is a
create-only path. Add optimistic concurrency when update workflows arrive, starting with
draft editing in iteration 03 and comment moderation in iteration 05.

## Publishing Model

Publishing is a state transition from an admin-owned draft to a public read model.

1. The admin edits durable MDX source on `ArticleDraft`.
2. Validation checks metadata, slug rules, allowed MDX constructs, and component policy.
3. Publishing compiles the approved source into render-ready public content.
4. The system writes a new `PublishedArticle` revision or updates the current revision
   atomically with an incremented revision number.
5. Public article endpoints read only `PublishedArticle` records in `Published` state.

Draft edits after publishing do not change public content until a later publish
transition succeeds.

The first admin publishing implementation uses a constrained Markdown subset stored in
`ArticleDraft.MdxSource`. It rejects imports, exports, MDX expressions, JSX, and raw HTML
at publish time, then writes escaped render-ready HTML to `PublishedArticle.RenderedHtml`.
Draft edits use `ArticleDraft.Version` for optimistic concurrency. A draft slug remains
editable until first publish and is locked after that to preserve public URL stability.
Republishing updates the current `PublishedArticle` row and increments `Revision`; it does
not introduce historical revision rows yet.

## Security Invariants

- OAuth client secrets are API configuration only.
- React never receives provider access tokens.
- Session cookies are `HttpOnly`, `Secure`, and `SameSite=Lax` or stricter.
- Admin endpoints require the `AdminOnly` policy.
- Authenticated comment endpoints require the `AuthenticatedUser` policy.
- Public article endpoints never expose draft MDX.
- User comments are encoded as text and never interpreted as MDX, HTML, or script.
- Runtime drafts, article state, comments, and identity records are never stored on the
  container filesystem.

## Rejected Options

| Option | Reason rejected |
|---|---|
| Runtime filesystem drafts | Container disks are ephemeral and scale-out would split authoring state. |
| SQLite bridge | Creates a second persistence shape before auth, publishing, and moderation depend on database behavior. |
| Database-generated domain IDs | Entities need stable identity before persistence for typed references, tests, and future domain events. |
| Database-generated business timestamps | Workflow tests and audit behavior need an explicit application time source. |
| Production startup migrations | Hide rollout failures and can race when multiple API replicas start. |
| Public entity setters | Allow persistence convenience to bypass domain invariants. |
| Exceptions for expected business failures | Obscures control flow and weakens typed domain contracts. |
| Generic repositories over EF | Add indirection without protecting a domain boundary. |
| MediatR in the baseline | Adds dispatch indirection before pipeline behavior exists. |
| Broad module services | Hide use-case boundaries and tend to collect unrelated behavior. |
| Raw strings for article slugs | Scatter URL identity validation across handlers and entities. |
| Integer-backed persisted statuses | Make persisted state opaque and allow accidental reorder bugs. |
| Premature concurrency tokens in create-only paths | Adds persistence ceremony before update conflicts exist. |
| Public runtime MDX compilation | Exposes readers to draft parsing failures and increases execution risk. |
| React-owned OAuth flow | Pushes secrets and provider token handling into the browser boundary. |
| User-authored MDX comments | Turns untrusted user input into executable content. |

## Implementation Impact

- Iteration 01 must introduce PostgreSQL persistence through Aspire before articles,
  auth, or comments, and prove it with a narrow test-only `CreateArticleDraft` path below
  HTTP.
- Iteration 02 must keep OAuth callbacks and cookie issuance in the API.
- Iteration 03 must persist draft MDX source instead of writing files.
- Iteration 03 owns MDX parsing and draft validation beyond persistence-level invariants.
- Iteration 04 must publish from drafts into `PublishedArticle` read records.
- Iteration 05 must keep comments pending until moderation approves them.
