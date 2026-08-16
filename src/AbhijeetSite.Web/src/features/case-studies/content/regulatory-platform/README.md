# Regulatory Platform Case-Study Snapshot

This directory is a website-owned publication snapshot of the fictional architecture thesis at
<https://github.com/abhijeethaval/Regulatory-Case-Arch-Case-Study>.

The canonical source revision is recorded in `caseStudyCatalog.ts`. To refresh the website:

1. Validate and commit the standalone source repository.
2. Copy `docs/*.md` and `CONTEXT.md` into this directory without editorial changes.
3. Regenerate any changed Mermaid blocks as reviewed SVGs under
   `public/case-studies/regulatory-platform-modernization/diagrams`.
4. Update the source signatures in `architectureDiagrams.ts` and `sourceRevision` in
   `caseStudyCatalog.ts`.
5. Run the frontend test, lint, and build scripts.

The website bundle must remain self-contained; do not fetch thesis content from GitHub at runtime
or weaken the production content-security policy to accommodate diagram rendering.
