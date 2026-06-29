# Iteration 02 - External Login

## Status

Implemented as the Google-login baseline. Live Google sign-in still requires Google
OAuth credentials and a configured callback URL in the Google Cloud console.

## Goal

Add external login with Google first and a provider abstraction that can support LinkedIn
without redesigning identity.

## Recommended Architecture

The API owns the OAuth/OpenID Connect flow and establishes a local secure cookie session.
React only redirects users to backend login endpoints and calls `/api/auth/me` to render
session state.

## Backend Work

- Configure authentication and authorization middleware.
- Add cookie authentication with secure production settings.
- Add Google external login.
- Add provider callback handling that upserts:
  - `User`
  - `ExternalLogin`
  - sign-in timestamp
- Add endpoints:
  - `GET /api/auth/login/google`
  - `GET /api/auth/callback/google`
  - `POST /api/auth/logout`
  - `GET /api/auth/me`
- Add authorization policies:
  - `AuthenticatedUser`
  - `AdminOnly`

## Frontend Work

- Add `features/auth`.
- Add login buttons.
- Add current-user loading state.
- Add authenticated shell affordances:
  - Sign in
  - Signed-in user display
  - Sign out

## Configuration

| Setting | Local Source | Production Source |
|---|---|---|
| Google client ID | user-secrets | ACA secret or Key Vault reference |
| Google client secret | user-secrets | ACA secret or Key Vault reference |
| Admin email allowlist | `appsettings.Development` | ACA environment variable or secret |
| Data Protection keys | repo-local ignored `artifacts` path | durable mounted path |

## Tests

- `/api/auth/me` returns anonymous state before login.
- Existing home endpoint remains public.
- `AdminOnly` policy rejects anonymous users.
- External-login upsert logic is unit tested with provider claims.

## Acceptance Criteria

- Google login works locally with HTTPS callback.
- Authenticated session survives page refresh.
- React does not receive provider tokens.
- Admin authorization is based on local application state, not provider email checks scattered across endpoints.

## Risks

- OAuth callback URLs break if forwarded headers are not configured behind Nginx or ACA ingress.
- Cookie auth requires CSRF design before mutating endpoints are exposed.
- Admin bootstrap must be explicit, such as configured admin email allowlist or one-time seed.
