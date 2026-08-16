# Case-Study Premise

## Fictional scenario

The Regulatory Platform has a product-line problem, not only a code-quality problem. Customer
deployments share business capabilities but have diverged through direct code customization. The
fictional transformation must create governed variability while preserving delivery commitments and
existing Customer operations.

## Synthetic outcome targets

These targets are fictional planning hypotheses. Discovery establishes baselines and calibrates the
thresholds before they become delivery commitments.

| Outcome | Target | Proposed evidence |
|---|---:|---|
| Reduce customer-specific code | 50% | Baseline and quarterly customer-code ratio |
| Reduce release regression incidents | 40% within 12 months | Escaped-defect and change-failure metrics |
| Increase reusable capabilities | Directional | Adoption and reuse scorecard by capability |
| Establish architecture governance | Operational | Decision rights, review workflow, and ADR evidence |
| Reduce centrally escalated decisions | Directional | Decision latency and escalation-rate trend |
| Convert a major capability | At least one | Forms Management and Application Intake shared-core adoption |

## Fictional constraints

- Feature development cannot freeze.
- The platform cannot be rewritten wholesale.
- The technology stack cannot be replaced within 12 months.
- Existing customers must remain operational.
- Annual release commitments must be maintained.
- Engineering capacity is limited.

## Case-study structure

| # | Area | Required content |
|---:|---|---|
| 1 | Platform Assessment and Shared-Core Strategy | Assessment framework, divergence classification, target architecture, capability map, principles |
| 2 | Customization Governance Framework | Core/configuration/extension/integration/custom decision model, governance, approval workflow, examples |
| 3 | Shared-Core Migration Roadmap | Migration strategy, prioritization, 12-18 month roadmap, risks, dependencies, trade-offs |
| 4 | Platform Engineering Foundations | Golden path, internal developer platform strategy, CI/CD, observability, API governance |
| 5 | Architecture Leadership and Operating Model | Team model, decision ownership, governance, stakeholder engagement |

## Recommended transformation thesis

Adopt a domain-oriented product-line architecture with five explicit layers:

1. A context map that separates stable business capabilities into bounded contexts.
2. Rich domain models within those contexts, including entity behavior, value objects, invariants,
   and domain services for behavior spanning multiple entities.
3. Thin, customer-tailored workflow orchestration that composes capabilities across bounded
   contexts without owning their business rules.
4. Governed configuration and extension points for variation within a bounded context.
5. Standard adapters for external systems and infrastructure concerns.

Treat direct customer forks as time-bounded exceptions with an owner, cost, expiry date, and
convergence plan.

## Workflow position in the target architecture

Workflow definitions are expected to differ by customer and jurisdiction. The shared-core objective
is therefore not one canonical workflow. It is a stable set of domain capabilities that workflows
invoke.

The workflow layer should:

- coordinate long-running business processes;
- invoke explicit application-level commands on bounded contexts;
- react to domain events through stable contracts;
- hold process state, retries, deadlines, and compensating actions;
- avoid duplicating domain invariants, fee logic, eligibility logic, or authorization policy.

The synthetic hypothesis that 70-80% of workflow behavior is common must be tested during
discovery. Commonality may
indicate reusable orchestration primitives and templates, but it must not be assumed to imply a
single universal process definition.

## Initial bounded-context hypothesis

This is a discovery hypothesis, not a final domain model:

| Bounded context | Primary responsibility | Example concepts |
|---|---|---|
| Regulatory Case Management | Agency-controlled adjudication of a regulated request | Regulatory Case, Information Request, Assignment, Decision |
| Party and Organization | Applicants, businesses, licensees, representatives | Party, Organization, Contact, Relationship |
| Regulatory Product | What may be applied for and jurisdictional requirements | License Type, Permit Type, Requirement, Term |
| Application Intake | Applicant authoring, questionnaires, structured answers, and submission packages | Questionnaire Definition, Draft Application, Answer, Evidence, Submission |
| Domain-owned decisioning | Eligibility, validation, compliance, fee, and approval policy within the relevant context | Policy, Evaluation, Evidence, Decision |
| Fees and Payments | Assessment and settlement of financial obligations | Fee Schedule, Assessment, Invoice, Payment, Refund |
| Regulatory Review | Desk-based evaluation and recommendation | Review, Assignment, Finding, Recommendation |
| Inspection Management | Scheduling and field verification | Inspection, Inspector, Checklist, Finding |
| Identity and Access Platform | Authentication, tenant membership, and coarse technical entitlements | Identity, Principal, Tenant Membership, Role, Permission |
| Domain authority | Context-specific authority to perform regulated actions | Delegation, Approval Limit, Jurisdiction Authority, Separation of Duty |
| Issuance and Credential | Granted regulatory rights and their lifecycle | License, Permit, Credential, Renewal, Suspension |
| Regulatory Notice | Legally significant communications and proof of service | Notice, Recipient, Delivery Requirement, Service Status |
| Configurable Regulatory Process | Customer-tailored process coordination | Process Definition, Process Instance, Task, Timer |
| Integration Management | Translation between platform contracts and external systems | Connector, Mapping, External Reference, Synchronization |

Context boundaries must follow business language, invariants, ownership, and change cadence rather
than UI screens or database tables.

## Configurable metadata strategy

Regulatory Product, Application Intake, and Regulatory Case Management each require customer- and
jurisdiction-specific metadata. This must be implemented as a governed extension mechanism inside
each bounded context, not as one shared, schema-less property bag.

### Stable core plus typed extensions

| Context | Stable domain model | Configurable extension examples |
|---|---|---|
| Regulatory Product | Product identity, jurisdiction, lifecycle, version, effective period, publication state | Regulatory classification, reporting codes, local labels, product-specific attributes |
| Application Intake | Draft identity, applicant, product version, questionnaire definition, submission state, declaration, evidence references | Form sections, questions, conditional visibility, field validation, supplemental data |
| Regulatory Case Management | Case identity, source submission, assignments, deadlines, reviews, findings, decision state | Local tracking fields, agency classifications, prioritization tags, reporting dimensions |

### Required metadata capabilities

- Namespaced field definitions owned by a bounded context.
- Explicit data types, cardinality, constraints, labels, and allowed values.
- Immutable schema versions after publication.
- Effective dates and jurisdiction/customer applicability.
- Draft, review, publish, deprecate, and retire lifecycle.
- Role-based visibility and editability.
- Classification for sensitive and personally identifiable information.
- Search, indexing, retention, and reporting declarations.
- Migration rules when a published definition evolves.
- Contract-safe event and API representation.
- Audit history for definition and value changes.

### Placement rule

Metadata is appropriate when the variation adds descriptive data or declarative policy that existing
domain behavior already knows how to interpret.

Metadata is not appropriate when the requirement introduces:

- a new invariant;
- a new state transition;
- materially different authorization;
- new calculations or decision semantics;
- a new lifecycle;
- behavior crossing multiple aggregates.

Those changes require an explicit domain-model enhancement, a governed policy extension, or a new
bounded context. They must not be hidden in scripts attached to metadata fields.

### Separate extension vocabularies

Each context owns its extension definitions and validation semantics. A global `CustomField` entity
shared by every context would create a universal model, couple releases, weaken language, and allow
cross-context data access. Shared infrastructure may provide storage, schema tooling, and rendering,
but the domain meaning remains context-owned.

## Forms boundary

The intake form has a strong semantic and lifecycle dependency on Application Intake. Therefore,
`QuestionnaireDefinition`, `DraftApplication`, `Answer`, and `SubmissionPackage` belong to the
Application Intake bounded context.

A reusable Forms Platform may provide technical capabilities:

- schema primitives and editors;
- rendering and accessibility;
- layout and conditional presentation;
- typed field widgets;
- client-side structural validation;
- schema migration tooling.

The Forms Platform does not own regulatory meaning, applicant answers, completeness policy,
eligibility, or submission. It is a generic platform capability consumed by Application Intake and
potentially other bounded contexts.

Regulatory Product publishes business requirements such as required information and evidence.
Application Intake translates those requirements into a versioned questionnaire and owns the
complete applicant experience.

### Regulatory requirement versus questionnaire

| Concept | Owner | Purpose |
|---|---|---|
| Information requirement | Regulatory Product | Defines the information or evidence required by law or policy |
| Statutory form version | Regulatory Product | Preserves an official form identity, version, effective period, and jurisdiction |
| Intake questionnaire | Application Intake | Presents applicable requirements as an applicant journey |
| Requirement-to-question mapping | Application Intake | Demonstrates how submitted answers satisfy product requirements |
| Rendering schema and widgets | Forms Platform | Provides reusable technical presentation capabilities |

An intake questionnaire may combine requirements from multiple statutory forms, progressively
disclose questions, or reuse an answer across sections. Those experience optimizations cannot erase
the traceability between each submitted answer and the regulatory requirement and version it
satisfies.

## Decisioning and rules boundary

Regulatory meaning remains in the bounded context responsible for the decision. A generic Rules
Platform supplies execution mechanics but does not own policy semantics.

| Concern | Owning domain context | Rules Platform |
|---|---|---|
| Policy name and ubiquitous language | Owns | Does not interpret |
| Inputs and evidence requirements | Owns | Validates against technical schema |
| Outcomes and domain errors | Owns | Returns typed execution result |
| Effective period and applicability | Owns | Stores or executes versioned specification |
| Expression parsing and evaluation | Consumes | Owns |
| Simulation and test harness | Defines scenarios | Executes |
| Audit explanation | Defines domain explanation | Captures execution trace |

For example, an Eligibility capability owns `DetermineEligibility`, its evidence model, outcomes,
and effective policy version. The Rules Platform may execute the versioned expression graph, but it
must not expose generic rule records directly to workflows or user interfaces.

The architecture must avoid a central rules database becoming the undocumented domain model.

## Identity, access, and regulatory authority

Technical access control and regulatory authority are separate models.

| Concern | Owner |
|---|---|
| Authentication and session security | Identity and Access Platform |
| Federation with customer identity providers | Identity and Access Platform |
| Tenant membership and coarse application roles | Identity and Access Platform |
| API and platform-resource authorization | Identity and Access Platform |
| Authority to review, inspect, approve, waive, suspend, or issue | Owning domain context |
| Delegation limits, monetary thresholds, jurisdiction, and product scope | Owning domain context |
| Conflict-of-interest and separation-of-duty policy | Owning domain context |

IAM establishes who the actor is and supplies coarse entitlements. The bounded context decides
whether that actor may perform a specific regulated action against a particular aggregate.

Customer-specific authorization implementations should converge on one identity and entitlement
platform while preserving customer- and jurisdiction-specific authority policies inside the domain.

## Process orchestration boundary

Process Orchestration coordinates customer-specific, long-running processes by invoking explicit
application commands on other bounded contexts and reacting to published outcomes.

It owns:

- workflow definitions and versions;
- process instances and correlation;
- customer-specific sequencing and branching;
- timers, retries, escalation triggers, and process-level compensation;
- human task coordination;
- cross-context process progress.

It does not own:

- aggregate state in participating contexts;
- eligibility, fee, compliance, inspection, or issuance policy;
- regulatory approval authority;
- the meaning of domain outcomes;
- cross-context database transactions.

### Interaction rules

- Workflow steps reference stable domain capabilities such as `RequestInspection`, not tables,
  status codes, rule identifiers, or internal services.
- Commands have one logical receiver and express intent.
- Events describe completed facts and may have multiple consumers.
- Long-running commands acknowledge acceptance; completion is reported asynchronously.
- Every command is idempotent and correlated to the process instance.
- Each bounded context commits locally and publishes reliably through an outbox or equivalent.
- Consumers use inbox/deduplication semantics because message delivery is at least once.
- Domain rejection is a modeled outcome, not an infrastructure retry.
- Compensation invokes explicit business operations; it never attempts distributed rollback.

The orchestrator decides what should happen next. The receiving bounded context retains complete
authority to accept or reject the requested operation.

## Party identity and evidentiary snapshots

Party and Organization owns the current master representation of people, businesses,
representatives, contacts, and relationships. Other contexts reference `PartyId` while work remains
mutable.

At a legally significant transition, the consuming context records an immutable snapshot:

- Application Intake captures the declared applicant and organization details at submission.
- Case Management evaluates the submitted snapshot, not a mutable party profile.
- Credential Lifecycle records the grantee identity applicable when the credential was issued.

Later corrections to master data do not silently rewrite prior evidence. A material change is
represented through an amendment, case action, or explicit synchronization process.

## Domain meaning versus technical platform

The same separation used for Forms and Rules applies across the architecture:

| Domain concern | Domain owner | Reusable technical capability |
|---|---|---|
| Applicant questionnaire and answers | Application Intake | Forms Platform |
| Regulatory policy and decision result | Relevant bounded context | Rules Platform |
| Regulatory process definition and instance | Configurable Regulatory Process | Workflow Runtime |
| Evidence classification and legal meaning | Intake, Case, Review, or Inspection | Document Platform |
| Legally significant notice and proof requirements | Regulatory Notice | Communications Delivery Platform |
| Party authority to perform a regulated action | Relevant bounded context | Identity and Access Platform |

Technical platforms are reusable because they remain ignorant of domain meaning. Domain contexts
remain reusable because they expose explicit behavior rather than customer-specific database or
script hooks.

## Deployment boundary

Bounded contexts are model and ownership boundaries, not mandatory microservices. The initial target
is a modular architecture in the existing technology stack with:

- module-owned models and persistence schemas;
- no cross-module table access;
- versioned commands, events, and APIs;
- independently testable modules;
- extraction seams for later service separation.

Independent deployment is introduced only where scale, availability, security, team autonomy, or
release cadence justifies its distributed-systems cost.

## First shared-core tracer capability

The first major capability to migrate should be **Forms Management through Application Intake**:

1. Regulatory Product publishes versioned information and evidence requirements.
2. Application Intake owns versioned questionnaires, drafts, completeness, declarations, and
   immutable submissions.
3. Forms Platform supplies reusable schema, authoring, rendering, and accessibility primitives.
4. One high-volume licensing family is migrated end to end for one reference customer.
5. Subsequent customers reuse the same domain capability while supplying product, questionnaire,
   metadata, and process configuration.

This choice addresses a synthetic scale indicator of hundreds of duplicated forms, produces visible
Customer value, establishes
versioned metadata and submission contracts, and creates the first reliable input into shared Case
Management and Process Orchestration. It is narrower and more reversible than attempting to
standardize complete customer workflows first.

## Quality bar for the case study

Every recommendation must identify:

- the current-state evidence it responds to;
- the decision and rejected alternatives;
- migration mechanics rather than only a target-state diagram;
- ownership and governance;
- measurable leading and lagging indicators;
- risks, dependencies, and reversibility.

## Critical architecture guardrails

- Bounded contexts do not share writable domain entities or a universal persistence model.
- Workflow orchestration does not become an anemic-domain dumping ground.
- Cross-context processes use explicit commands, events, and anti-corruption layers.
- Submitted applications are immutable regulatory records. Corrections are represented as new,
  versioned amendment submissions or additional evidence, never in-place edits.
- A domain service is used only for domain behavior that cannot naturally belong to one entity or
  value object; it is not a generic service-layer abstraction.
- Customer variation is located deliberately: process definition, policy/configuration, extension,
  or integration adapter.

## Intake-to-case lifecycle

1. Application Intake owns applicant-controlled drafting, completeness validation, declarations,
   and submission.
2. `ApplicationSubmitted` publishes an immutable, versioned submission package.
3. Regulatory Case Management opens a case from that package using an anti-corruption translation.
4. A regulator may issue an information or correction request against the case.
5. Case Management owns the request, including its reason, scope, due date, and resolution state.
6. The applicant authors an amendment response through Application Intake.
7. `ApplicationAmendmentSubmitted` publishes another immutable package referencing the original
   submission, case, and regulator request.
8. Case Management evaluates the new package while retaining the complete evidentiary history.

This boundary separates legal record keeping from process coordination. The customer workflow may
vary the timing and sequence of these interactions, but it may not mutate submitted evidence.

### Ownership of the amendment interaction

| Concern | Owning context |
|---|---|
| Deficiency or information request | Regulatory Case Management |
| Request reason, scope, due date, and regulator status | Regulatory Case Management |
| Applicant amendment draft and validation | Application Intake |
| Amendment declarations and submission | Application Intake |
| Immutable amendment package | Application Intake |
| Evaluation and acceptance of the response | Regulatory Case Management |

Neither context shares its aggregate with the other. The request identifier and versioned submission
contract provide correlation across the boundary.
