# Iteration 00 - Architecture Baseline

## Goal

Lock the core architecture decisions before adding persistence, OAuth, MDX compilation,
or comment moderation.

## Architectural Decisions

| Concern | Decision | Tradeoff |
|---|---|---|
| Backend structure | Continue vertical slices under `src/AbhijeetSite.Api/Features` | Avoids premature layer ceremony while keeping feature ownership clear. |
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

## Data Model Draft

| Entity | Purpose |
|---|---|
| `User` | Local identity for external-login users. |
| `ExternalLogin` | Provider subject mapping for Google and LinkedIn. |
| `ArticleDraft` | Editable owner-authored MDX source and metadata. |
| `PublishedArticle` | Immutable or versioned public read model. |
| `Comment` | Authenticated user comment with moderation state. |

## Security Invariants

- OAuth client secrets never ship to React.
- React never stores provider access tokens.
- Auth cookies are `HttpOnly`, `Secure`, and `SameSite=Lax` or stricter where possible.
- Only admin users can create, edit, preview, or publish articles.
- Public article endpoints return published articles only.
- User comments cannot contain executable MDX, HTML, or script content.

## Acceptance Criteria

- A short architecture decision note exists for MDX storage and published read models.
- Entity names, states, and ownership boundaries are agreed before implementation.
- No implementation starts with filesystem-backed runtime drafts.

## Validation

- Review this plan against current deployment topology in `README.md`.
- Confirm the plan does not require writable container filesystem state.
