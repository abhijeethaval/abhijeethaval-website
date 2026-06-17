# Iteration 06 - LinkedIn And Production Hardening

## Goal

Add LinkedIn login after Google proves the local identity model, then harden the site for
public use.

## LinkedIn Work

- Add LinkedIn provider configuration.
- Add endpoints:
  - `GET /api/auth/login/linkedin`
  - `GET /api/auth/callback/linkedin`
- Map LinkedIn provider subject to existing `ExternalLogin`.
- Support account linking when provider emails match an existing verified user.
- Add UI button alongside Google sign-in.

## Production Hardening

| Area | Work |
|---|---|
| Cookies | Confirm `Secure`, `HttpOnly`, `SameSite`, expiration, sliding policy. |
| Forwarded headers | Configure for ACA/Nginx callback correctness. |
| CSRF | Add anti-forgery strategy for authenticated mutations. |
| Rate limits | Apply to login callbacks, comment creation, and admin mutations. |
| Secrets | Move provider secrets to ACA secrets or Key Vault references. |
| Audit | Record admin publish and moderation actions. |
| Observability | Add structured logs around auth, publish, and moderation transitions. |
| Backups | Define database backup and restore path before storing real articles/comments. |

## Frontend Hardening

- Add route guards for admin views.
- Add robust empty/error states.
- Add public loading skeletons for article and comments.
- Add accessible button labels and focus states.
- Add SEO metadata support for published articles.

## Tests

- Google and LinkedIn map to stable local users.
- A user cannot impersonate another provider identity.
- Admin routes reject normal authenticated users.
- CSRF protection blocks missing/invalid tokens.
- Published article pages expose expected metadata.

## Acceptance Criteria

- Users can sign in with Google or LinkedIn.
- Comments and article publishing remain provider-agnostic.
- Production deployment has documented secret, callback, and forwarded-header configuration.
- Audit logs capture publish and moderation actions.

## Risks

- LinkedIn app approval/configuration can slow delivery; it should not block article/comment capability.
- Account linking can create takeover risk if email verification semantics differ by provider.
- Excessive provider abstraction can become speculative; keep the seam at normalized external-login claims.
