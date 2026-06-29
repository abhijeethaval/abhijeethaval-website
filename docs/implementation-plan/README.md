# Iterative Implementation Plan

This plan expands the current personal website foundation into a full-stack publishing
platform that demonstrates architecture, implementation depth, and production discipline.

## Target Capability

Authenticated users can sign in with external providers, comment on published articles,
and the site owner can draft, preview, and publish MDX-based articles.

## Architecture Position

| Decision | Choice | Rationale |
|---|---|---|
| Article source format | MDX | Shows technical writing depth and supports curated interactive components. |
| Runtime article storage | Database or blob-backed durable storage | Container filesystems are not durable in Azure Container Apps. |
| Public article rendering | Published read model | Keeps public reads fast, stable, and isolated from drafts. |
| Auth ownership | API-owned auth | Keeps OAuth secrets and callback validation out of React. |
| Session mechanism | Secure HTTP-only cookie | Avoids browser token storage and simplifies same-origin deployment. |
| Comment moderation | Pending before public visibility | Protects a personal brand site from spam and abuse. |

## Iterations

| Iteration | File | Outcome |
|---|---|---|
| 00 | [Architecture Baseline](./iteration-00-architecture-baseline.md) | Establish cross-cutting decisions before persistence/auth. |
| 01 | [Persistence Foundation](./iteration-01-persistence-foundation.md) | Add durable storage, migrations, and core entities. |
| 02 | [External Login](./iteration-02-external-login.md) | Add Google login first, with a provider seam for LinkedIn. |
| 03 | [MDX Article Drafting](./iteration-03-mdx-article-drafting.md) | Create admin draft and preview workflow. |
| 04 | [Publishing Pipeline](./iteration-04-publishing-pipeline.md) | Publish versioned MDX to a stable public read model. |
| 05 | [Comments](./iteration-05-comments.md) | Add authenticated comments with moderation. |
| 06 | [LinkedIn And Hardening](./iteration-06-linkedin-and-hardening.md) | Add LinkedIn and production hardening. |

## Runbooks

| Runbook | Purpose |
|---|---|
| [Google Login](../google-login.md) | End-to-end Google OAuth setup, Azure configuration, code walkthrough, and production troubleshooting. |

## Non-Goals Until After Iteration 06

- Public user registration with passwords.
- Multi-author editorial workflow.
- Rich text editor.
- User-submitted MDX or HTML.
- Likes, reactions, subscriptions, newsletters, or analytics dashboards.
- Full-text search.
