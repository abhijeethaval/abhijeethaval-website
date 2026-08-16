# Decision and Assumption Log

Statuses:

- **Proposed** - recommended but not yet confirmed.
- **Accepted** - adopted direction for the case study.
- **Open** - information or decision still required.
- **Rejected** - considered and deliberately not selected.

## Architecture decisions

| ID | Status | Decision | Rationale | Consequence |
|---|---|---|---|---|
| D-001 | Accepted | Frame the problem as product-line divergence | Common capabilities exist, but variability is encoded through customer code | The strategy must govern variability, not merely consolidate repositories |
| D-002 | Accepted | Treat workflows as customer-tailored orchestration over reusable bounded-context capabilities | Jurisdictions have distinct processes while much of the underlying domain behavior remains stable | Shared-core migration targets domain capabilities and contracts rather than a universal workflow |
| D-003 | Accepted | Use an incremental strangler migration | Feature delivery and customer operations cannot freeze | Legacy and shared-core paths must coexist temporarily |
| D-004 | Accepted | Separate core, configuration, extension, integration, and exceptional custom code | Each variability type needs distinct ownership, testing, and lifecycle rules | Architecture governance becomes part of the delivery path |
| D-005 | Accepted | Begin target architecture with subdomain discovery, bounded contexts, and a context map | Domain boundaries must precede service, database, and workflow implementation choices | The case study works from domain architecture toward migration mechanics |
| D-006 | Accepted | Place invariants and behavior in bounded-context domain models | Reusable business behavior should not be duplicated in customer workflows | Workflows remain thin coordinators; entities, value objects, and domain services own business behavior |
| D-007 | Accepted | Use commands and domain events as explicit cross-context contracts | Shared entities or cross-context database access would couple models and release cycles | Contexts can evolve independently behind versioned contracts and anti-corruption layers |
| D-008 | Accepted | Treat submitted applications as immutable records | Submission changes authority, purpose, mutability, and audit obligations | Corrections use versioned amendments or additional evidence; original submissions are never overwritten |
| D-009 | Accepted | Separate Application Intake and Regulatory Case Management bounded contexts | Applicant-controlled preparation and agency-controlled adjudication use different language, invariants, actors, and lifecycles | Submission and amendment events form the translation boundary between the contexts |
| D-010 | Accepted | Route regulator-requested amendments through Application Intake | Case Management owns the regulatory request, while Intake owns applicant authoring, validation, declarations, and submission | An immutable amendment package is correlated to the case and request; Case Management never edits applicant evidence |
| D-011 | Accepted | Model Regulatory Product as a versioned bounded context | Product definitions carry regulatory meaning, effective dates, requirements, and lifecycle behavior | Applications and cases reference immutable product versions |
| D-012 | Accepted | Support typed, versioned metadata extensions within Product, Intake, and Case contexts | Jurisdictions require additional data without requiring customer forks | Each context owns its extension vocabulary, validation, security, lifecycle, and API representation |
| D-013 | Accepted | Prohibit a global schema-less custom-field model | A universal metadata model would leak semantics across contexts and allow configuration to bypass domain design | Shared metadata infrastructure may exist, but definitions and meaning remain context-owned |
| D-014 | Accepted | Keep intake questionnaire semantics inside Application Intake rather than creating a Forms bounded context | Questionnaires, answers, completeness, declarations, and submission share one language and lifecycle | A generic Forms Platform may provide reusable schema and rendering infrastructure without owning the domain model |
| D-015 | Accepted | Separate statutory information requirements from applicant-facing questionnaires | Regulatory obligations and user-experience composition have different ownership and change drivers | Regulatory Product owns official requirements and versions; Application Intake maps them into questionnaire versions with end-to-end traceability |
| D-016 | Accepted | Keep decision semantics in the responsible domain context and use a generic Rules Platform only for execution mechanics | Eligibility, compliance, fees, and approval have domain-specific language, evidence, outcomes, and effective-date semantics | Domain contexts expose meaningful decision operations; the rules engine remains replaceable infrastructure |
| D-017 | Accepted | Do not presume a single Decisioning bounded context | The label combines policies with different language, evidence, lifecycle, ownership, and consequences | Decision behavior remains with Product, Intake, Case, Fees, Review/Inspection, and Issuance unless discovery reveals a cohesive decisioning model |
| D-018 | Accepted | Separate identity and technical access control from regulatory business authority | Authentication and coarse entitlements cannot determine contextual approval, inspection, waiver, suspension, or issuance authority | IAM identifies actors and grants platform access; each domain context enforces action-specific authority and separation-of-duty invariants |
| D-019 | Accepted | Use a command/event-based Process Orchestration bounded context for long-running customer workflows | Customer processes vary, while participating contexts must retain their own invariants and release independence | Orchestration owns sequence, correlation, timers, retries, and compensation; domain contexts own behavior and outcomes |
| D-020 | Accepted | Avoid distributed transactions across bounded contexts | Long-running regulatory processes include human work, external systems, and irreversible actions | Each context commits locally; reliable messaging, idempotency, and explicit business compensation provide consistency |
| D-021 | Accepted | Use a Party and Organization master model plus immutable evidentiary snapshots | Current identity data changes independently from facts legally declared or evaluated at a point in time | Mutable work references `PartyId`; submissions, cases, and credentials preserve relevant identity snapshots |
| D-022 | Accepted | Separate Regulatory Review from Inspection Management | Desk review and field inspection have different actors, scheduling, evidence, findings, and lifecycles | They may share checklist infrastructure but expose independent domain capabilities |
| D-023 | Accepted | Separate Regulatory Notice from communication delivery infrastructure | Legal notice meaning, recipients, deadlines, and proof of service differ from channel delivery mechanics | Regulatory Notice owns the official record; Communications Platform owns email, SMS, print, retries, and provider adapters |
| D-024 | Accepted | Keep evidence meaning in domain contexts and binary storage in a Document Platform | A document can represent different legal evidence in Intake, Case, Review, or Inspection | Domain records reference immutable document versions; the platform owns storage, scanning, encryption, retention execution, and retrieval |
| D-025 | Accepted | Separate Configurable Regulatory Process from its workflow runtime | Process definitions and business tasks have domain meaning; timers, persistence, and worker execution are technical mechanics | Process Orchestration remains portable across workflow engines and does not expose engine-specific concepts to other contexts |
| D-026 | Accepted | Use context-specific anti-corruption adapters for external integrations | A universal enterprise canonical model would accumulate every customer and vendor variation | Integration tooling can be shared, but each domain owns translation to and from its published language |
| D-027 | Accepted | Build reporting and search from event-fed projections rather than operational cross-context queries | Shared reporting joins would bypass boundaries and couple schemas | Read models are disposable projections; domain systems remain systems of record |
| D-028 | Accepted | Treat tenant, customer, and jurisdiction as distinct concepts | A customer may operate across jurisdictions, while regulatory applicability follows jurisdiction and effective date | All contracts carry explicit scope; product policy is jurisdiction-scoped and deployment/configuration is tenant-scoped |
| D-029 | Accepted | Implement bounded contexts initially as enforceable modules in the existing stack | Constraints prohibit a rewrite and capacity is limited; bounded contexts do not require microservices | Module ownership, schema isolation, contracts, and tests precede selective service extraction |
| D-030 | Accepted | Model renewal as a new regulated request referencing an existing credential | Renewal requires current declarations, policy versions, fees, reviews, and potentially inspections | Intake and Case run a new process; Credential Lifecycle owns the resulting continuation, replacement, lapse, suspension, or revocation |
| D-031 | Accepted | Migrate Forms Management and Application Intake as the first Shared Core Tracer Capability | A synthetic scale indicator of hundreds of duplicated forms creates measurable value, while the slice establishes product requirements, metadata, versioning, documents, and submission contracts | Migrate one high-volume Product Family for one reference Customer, then scale by reuse score |
| D-032 | Accepted | Set the second customer as the shared-core proof gate | The first customer can still produce a disguised bespoke implementation | Production reuse without a customer fork is required before scaling the migration factory |
| D-033 | Accepted | Migrate new business lifecycles before historical and in-flight records | Legal records and long-running work make wholesale data migration high risk | Existing drafts/cases remain legacy by default; new drafts, renewals, or product cohorts enter the shared path |
| D-034 | Accepted | Protect an initial synthetic 20-25% capacity range for convergence | A platform transformation cannot succeed through opportunistic spare capacity | Leadership must explicitly trade scope, target, and capacity; the illustrative range is recalibrated after assessment |
| D-035 | Accepted | Treat configuration and regulatory definitions as independently versioned deployable artifacts | Rebuilding application binaries per customer recreates deployment divergence | Shared application artifacts are promoted once; definitions follow audited domain publication pipelines |
| D-036 | Accepted | Establish golden paths for modules, configuration, adapters, processes, and projections | Governance must reduce developer effort rather than add manual approvals | Templates include contracts, tests, telemetry, security, ownership, CI/CD, and runbooks |
| D-037 | Accepted | Use a catalog-first, workflow-driven IDP strategy | A portal without reliable ownership, templates, and metadata does not improve delivery | Validate developer workflows before selecting or expanding a portal implementation |
| D-038 | Accepted | Standardize OpenAPI, AsyncAPI, contract testing, and compatibility gates | Cross-context independence depends on explicit and evolvable contracts | Breaking changes require managed versioning and consumer migration |
| D-039 | Accepted | Separate regulatory audit records from operational telemetry | Logs and traces have different retention, integrity, and semantic requirements from legal decisions | Domain contexts own immutable audit evidence; OpenTelemetry supports diagnosis and reliability |
| D-040 | Accepted | Use federated architecture governance with tiered decision rights | Central approval of routine changes would replace one escalation dependency with another | Teams decide local reversible changes; context and cross-context decisions escalate according to scope and reversibility |

## Assumptions

| ID | Status | Assumption | Validation needed |
|---|---|---|---|
| A-001 | Open | Deployments have identifiable customer-specific code or configuration boundaries | Repository and deployment topology |
| A-002 | Open | Workflow variants mostly compose stable domain capabilities, while process sequence and policy vary | Event-storming and workflow/change-history analysis |
| A-003 | Open | The Regulatory Platform can introduce platform APIs and metadata without replacing the current stack | Current stack and deployment constraints |
| A-004 | Open | Product teams can contribute to shared capabilities under platform standards | Team topology and allocation model |
| A-005 | Open | Existing customers can migrate incrementally by workflow or bounded capability | Release and tenancy architecture |

## Open domain-design decisions

| ID | Status | Question |
|---|---|---|
| Q-001 | Accepted | Application Intake owns authoring and immutable submission packages; Regulatory Case Management owns agency adjudication |
| Q-002 | Accepted | Regulatory Product is a versioned domain capability with governed metadata extensions |
| Q-003 | Accepted | Domain contexts own decision meaning and policy contracts; a generic Rules Platform owns expression execution, versioning mechanics, simulation, and traces |
| Q-004 | Accepted | Identity and Access owns authentication and coarse entitlements; regulatory authority remains in the context owning the regulated action |
| Q-005 | Accepted | Process Orchestration owns long-running coordination and process-level compensation while participating contexts own domain operations and compensation behavior |
| Q-006 | Accepted | Forms Management and Application Intake form the first shared-core tracer, starting with one high-volume licensing family |
| Q-007 | Accepted | Intake questionnaire definitions and submitted answers belong to Application Intake; reusable form rendering is a generic platform capability |
