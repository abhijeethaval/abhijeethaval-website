# Iteration 04 - Publishing Pipeline

## Goal

Publish validated MDX drafts into a stable public article read model.

## Recommended Architecture

Publishing is a state transition, not a field update. The system converts an admin draft
into a versioned `PublishedArticle` record with render-ready content and public metadata.

## Backend Work

- Add admin-only publish endpoint:
  - `POST /api/admin/articles/drafts/{id}/publish`
- Add public article endpoints:
  - `GET /api/articles`
  - `GET /api/articles/{slug}`
- Add revisioning:
  - preserve previous published revision or record revision number
  - update public article atomically
- Add publish validation:
  - draft exists
  - draft belongs to publishable status
  - MDX compiles under allowed component policy
  - slug is unique among published articles
- Add metadata:
  - `PublishedAt`
  - `UpdatedAt`
  - `Revision`

## Frontend Work

- Add public article list.
- Add public article detail page.
- Add admin publish button.
- Add publish confirmation with visible slug and public URL.

## MDX Rendering Strategy

| Strategy | Recommendation | Tradeoff |
|---|---|---|
| Compile MDX to sanitized HTML at publish time | Preferred first implementation | Strong public-read performance and simple API responses. |
| Store MDX and render in React at runtime | Avoid initially | Pushes parsing, trust, and errors to visitors. |
| Git-backed checked-in MDX files | Future enhancement | Great for version control, weak for runtime authoring unless paired with GitHub API workflow. |

## Tests

- Published article appears in public list.
- Draft article does not appear in public list.
- Publishing creates or updates a public revision atomically.
- Invalid MDX blocks publishing with actionable error.
- Public article endpoint returns `404` for unknown slug.

## Acceptance Criteria

- Admin can publish an article from MDX source.
- Anonymous visitors can read published articles.
- Public readers never execute arbitrary MDX.
- Draft edits after publish do not mutate the public article until republished.

## Risks

- Sanitization and component rendering must be explicit; MDX can become code execution if treated casually.
- Public article URLs must remain stable after republishing.
