# 0002 - Public Ideas Experience Catalog

## Status

Accepted

## Context

The public site now presents more than text articles. Some ideas are bespoke interactive
experiences implemented as typed React components, while written essays and build notes
continue through the durable article publishing workflow.

Calling the public surface "Articles" leaks one storage and presentation format into the
site's information architecture. Renaming the backend Articles module would create churn
without changing its domain responsibility for drafting and publishing written content.

## Decision

Use **Ideas** as the public experience and keep **Articles** as the backend publishing
domain.

| Concern | Decision |
|---|---|
| Public navigation | Use `Ideas`. |
| Canonical public routes | Use `/ideas` and `/ideas/{slug}`. |
| Compatibility routes | Continue resolving `/articles` and `/articles/{slug}`. |
| Interactive experiences | Register reviewed React components in a typed frontend catalog. |
| Written content | Continue reading published summaries and HTML from `/api/articles`. |
| Index presentation | Render one catalog; show experience formats as item metadata. |
| Duplicate slugs | The code-backed interactive experience takes public precedence. |

The Ideas index is an editorial composition rather than a count-driven archive. It may
feature one strong interactive thesis without presenting the catalog as incomplete.
Interactive theses, essays, and build notes do not create separate index sections or
navigation categories; their format labels help readers scan one coherent catalog.

## Boundaries

- Interactive React code is reviewed, built, and deployed with the web application.
- Draft MDX and published HTML remain durable data owned by the Articles module.
- The database never stores executable React or arbitrary client code.
- The public Ideas vocabulary does not rename API endpoints, database schemas, or domain
  types that still accurately describe article publishing.
- A future publishing workflow may add a typed experience key when authors need to attach
  published metadata to registered interactive experiences. The first experience does not
  require that schema change.

## Consequences

- The frontend owns a small typed registry of code-backed interactive experiences.
- Public routing resolves that registry before falling back to published article detail.
- The Ideas index can render even when the article API is temporarily unavailable, while
  reporting the written-catalog failure explicitly.
- Existing article URLs remain valid while new links and the sitemap use canonical Ideas
  routes.
