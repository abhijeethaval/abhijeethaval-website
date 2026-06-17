# Iteration 05 - Comments

## Goal

Allow authenticated users to comment on published articles while protecting the public site
through moderation.

## Recommended Architecture

Users authenticate through external providers, but comments belong to local `User` records.
New comments enter `Pending` state and become public only after admin approval.

## Backend Work

- Add authenticated comment endpoint:
  - `POST /api/articles/{slug}/comments`
- Add public approved-comments endpoint:
  - `GET /api/articles/{slug}/comments`
- Add admin moderation endpoints:
  - `GET /api/admin/comments?status=pending`
  - `POST /api/admin/comments/{id}/approve`
  - `POST /api/admin/comments/{id}/reject`
  - `DELETE /api/admin/comments/{id}`
- Add anti-abuse controls:
  - maximum body length
  - per-user rate limit
  - per-IP rate limit
  - duplicate submission guard

## Frontend Work

- Add comment list to article detail page.
- Add signed-in comment form.
- Add sign-in prompt for anonymous readers.
- Add pending-submission state after posting.
- Add admin moderation queue.

## Content Policy

| Input | Decision |
|---|---|
| Comment body | Plain text first |
| Markdown in comments | Defer |
| Links | Store plain text; linkify later only after moderation |
| HTML | Reject or encode |
| MDX | Reject |

## Tests

- Anonymous users cannot post comments.
- Authenticated users can submit pending comments.
- Pending comments are hidden from public readers.
- Approved comments are visible.
- Rejected/deleted comments are hidden.
- Admin-only moderation endpoints reject non-admin users.

## Acceptance Criteria

- Logged-in users can submit comments on published articles.
- Comments do not appear publicly until approved.
- Admin can approve and reject comments.
- User-generated content is safely encoded.

## Risks

- Cookie-authenticated POST endpoints need CSRF mitigation.
- Spam control should be present before public deployment, even if basic.
- Moderation queue must avoid leaking email addresses unnecessarily.
