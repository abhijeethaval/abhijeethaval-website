# Iteration 00 - Architecture Baseline

## Goal

Lock the core architecture decisions before adding persistence, OAuth, MDX compilation,
or comment moderation.

## Decision Record

The accepted baseline lives in
[0001 - Publishing Platform Architecture Baseline](../architecture/0001-publishing-platform-baseline.md).

## Architectural Decisions

| Concern | Decision | Tradeoff |
|---|---|---|
| Backend structure | Use a modular monolith with module-owned feature slices under `src/AbhijeetSite.Api/Features` | Makes domain boundaries visible while avoiding distributed-system overhead. |
| Frontend structure | Add `features/auth`, `features/articles`, `features/comments`, and `features/admin` | Public and admin flows stay separate without a router-heavy rewrite first. |
| Article source | Store MDX source as durable text, not runtime filesystem files | Supports create/publish in ACA where container disks are ephemeral. |
| Public rendering | Compile and persist a published read model | Slightly more pipeline work, but public reads avoid draft parsing and unsafe execution. |
| User auth | External login only | Avoids password storage and account recovery scope. |
| Authorization | Role/claim based admin policy | Keeps owner-only publishing separate from authenticated commenting. |
| Comments | Plain text or restricted Markdown only | MDX is owner-authored only; user content must not execute components. |

## Domain Boundaries

- `Identity`: local users, external provider links, admin authorization.
- `Articles`: drafts, MDX source, publishing, public article reads.
- `Comments`: authenticated comment creation, moderation, public approved comments.

## Module And Feature Shape

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

Modules are the durable boundaries. Features are workflow slices inside those boundaries.
Shared kernel primitives are allowed only for stable cross-cutting concepts such as
`Result`, `Error`, and application time.

## Data Model Draft

| Entity | Purpose |
|---|---|
| `User` | Local identity for external-login users. |
| `ExternalLogin` | Provider subject mapping for Google and LinkedIn. |
| `ArticleDraft` | Editable owner-authored MDX source and metadata. |
| `PublishedArticle` | Immutable or versioned public read model. |
| `Comment` | Authenticated user comment with moderation state. |

## Canonical States

| Type | Values |
|---|---|
| `ArticleDraftStatus` | `Draft`, `ReadyToPublish`, `Archived` |
| `PublishedArticleStatus` | `Published`, `Unpublished` |
| `CommentStatus` | `Pending`, `Approved`, `Rejected`, `Deleted` |

## Canonical Policies

| Policy | Purpose |
|---|---|
| `AuthenticatedUser` | Allows signed-in users to create comments. |
| `AdminOnly` | Allows owner-only article drafting, preview, publishing, and moderation. |

## Security Invariants

- OAuth client secrets never ship to React.
- React never stores provider access tokens.
- Auth cookies are `HttpOnly`, `Secure`, and `SameSite=Lax` or stricter where possible.
- Only admin users can create, edit, preview, or publish articles.
- Public article endpoints return published articles only.
- User comments cannot contain executable MDX, HTML, or script content.

## Acceptance Criteria

- [x] A short architecture decision note exists for MDX storage and published read models.
- [x] Entity names, states, and ownership boundaries are agreed before implementation.
- [x] No implementation starts with filesystem-backed runtime drafts.

## Validation

- [x] Review this plan against current deployment topology in `README.md`.
- [x] Confirm the plan does not require writable container filesystem state.
