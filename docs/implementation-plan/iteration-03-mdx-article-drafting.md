# Iteration 03 - MDX Article Drafting

## Goal

Add an admin-only article drafting workflow using MDX as the source format.

## Core Decision

MDX is the authoring format. Drafts are durable records containing MDX text, metadata,
and validation state. They are not files written to the deployed container filesystem.

## Backend Work

- Add admin-only draft endpoints:
  - `POST /api/admin/articles/drafts`
  - `PUT /api/admin/articles/drafts/{id}`
  - `GET /api/admin/articles/drafts`
  - `GET /api/admin/articles/drafts/{id}`
  - `POST /api/admin/articles/drafts/{id}/validate`
- Add request validation:
  - title required
  - slug required and URL safe
  - summary required
  - MDX source required
  - maximum MDX size
- Add domain errors for:
  - duplicate slug
  - invalid slug
  - invalid MDX
  - non-admin access

## Frontend Work

- Add admin route or view for draft list.
- Add draft editor:
  - title
  - slug
  - summary
  - MDX source
  - save draft
  - validate
- Add preview pane only after validation support exists.

## MDX Component Policy

| Component Type | Decision |
|---|---|
| Standard Markdown | Allow |
| Code blocks | Allow with syntax highlighting later |
| Curated React components | Allow only from an explicit registry |
| Arbitrary imports | Reject |
| Raw HTML | Reject or sanitize before rendering |

## Tests

- Admin can create and update draft.
- Anonymous user cannot access draft endpoints.
- Duplicate slug returns a typed validation error.
- Invalid slug returns a typed validation error.
- Existing published articles are not affected by draft edits.

## Acceptance Criteria

- An admin can create and save an MDX draft.
- Drafts persist across API restarts.
- Draft validation produces actionable errors.
- No public endpoint exposes draft content.

## Risks

- Full MDX compilation inside .NET may require a Node-based build service, a restricted Markdown subset, or a publish-time frontend compilation pipeline.
- If interactive MDX components are required, the allowed component registry must be designed before publishing.
