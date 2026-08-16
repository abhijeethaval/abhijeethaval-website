# Bounded-Context Catalog

## Subdomain classification

Classification is a strategic investment decision, not a statement about implementation size.

| Capability | Classification | Rationale |
|---|---|---|
| Regulatory Product and Policy | Core | Encodes jurisdictional products, requirements, applicability, and effective versions |
| Application Intake | Core | Converts regulatory requirements into a compliant applicant experience and immutable submission |
| Regulatory Case Management | Core | Owns agency adjudication, information requests, authority, and final decisions |
| Configurable Regulatory Process | Core | Enables tailored customer processes over stable domain capabilities |
| Credential Lifecycle | Core | Owns the granted regulatory right and its legal standing |
| Party and Organization | Supporting | Reusable master identity and relationships; snapshots preserve domain evidence |
| Fees and Receivables | Supporting | Assesses and settles regulatory financial obligations |
| Regulatory Review | Supporting | Provides structured desk review, findings, and recommendations |
| Inspection Management | Supporting | Provides field-verification scheduling, evidence, findings, and outcomes |
| Regulatory Notice | Supporting | Creates official notices and determines proof-of-service satisfaction |
| Integration Management | Supporting | Operates external adapters and synchronization without owning domain semantics |
| Forms Platform | Generic | Schema, widgets, rendering, accessibility, and structural validation |
| Rules Platform | Generic | Expression execution, policy version mechanics, simulation, and traces |
| Workflow Runtime | Generic | Durable execution, timers, retries, workers, and persistence |
| Identity and Access Platform | Generic | Authentication, federation, tenant membership, and coarse entitlements |
| Document Platform | Generic | Immutable storage, scanning, encryption, retention, and retrieval |
| Communications Platform | Generic | Channel delivery, providers, retries, and telemetry |
| Eventing and Contract Platform | Generic | Reliable messaging, schema registry, outbox/inbox support, and observability |
| Search and Analytics Platform | Generic | Event-fed projections, indexing, dashboards, and reporting infrastructure |

The classification should be revisited by product line. For example, Fees may become core for tax
administration, but it is supporting for the initial licensing and permitting tracer.

## Context responsibilities

### Regulatory Product and Policy

Owns:

- product definitions and immutable published versions;
- jurisdiction and effective periods;
- information, evidence, review, inspection, and fee requirements;
- product applicability and renewal terms;
- statutory form identity and version.

Does not own:

- applicant questionnaires;
- active cases;
- workflow sequence;
- payment transactions.

### Application Intake

Owns:

- questionnaire definitions and versions;
- applicant drafts and collaborators;
- answers, evidence references, completeness, and declarations;
- immutable submission and amendment packages;
- traceability from submitted material to product requirements.

Does not own:

- agency adjudication;
- mutable party master data;
- generic rendering infrastructure;
- downstream process state.

### Regulatory Case Management

Owns:

- regulatory case identity and lifecycle;
- intake acceptance and case opening;
- assignments and work ownership;
- information and correction requests;
- adjudication evidence references;
- recommendation acceptance and final decision;
- case-specific regulatory authority and separation of duty.

Does not own:

- applicant-authored drafts;
- field inspection internals;
- fee ledger;
- credential standing;
- workflow execution mechanics.

### Configurable Regulatory Process

Owns:

- customer- and jurisdiction-specific process definitions;
- definition publication and versioning;
- process instances, correlation, tasks, timers, and escalations;
- cross-context sequencing and process-level compensation;
- process progress projections.

Does not own:

- participating aggregate state;
- domain calculations, invariants, or authority;
- direct database access to other contexts;
- workflow-engine-specific public contracts.

### Credential Lifecycle

Owns:

- credential identity and grantee snapshot;
- grant conditions and endorsements;
- effective period and standing;
- issuance, replacement, suspension, revocation, and expiration.

Does not own:

- renewal application or adjudication;
- payment processing;
- the historical case decision.

## Context-map relationships

| Upstream | Downstream | Evans relationship/pattern | Contract |
|---|---|---|---|
| Regulatory Product | Application Intake | Customer/Supplier + Published Language | `ProductVersionPublished` and requirement API |
| Party and Organization | Application Intake | Open Host Service + downstream snapshot/ACL | Party API and immutable `PartySnapshot` |
| Forms Platform | Application Intake | Conformist at technical boundary | Versioned schema language and renderer SDK |
| Application Intake | Regulatory Process | Published Language | `ApplicationSubmitted`, `AmendmentSubmitted` |
| Regulatory Process | Domain contexts | Open Host Service | Typed commands and outcome events |
| Regulatory Product | Case, Fees, Credential | Published Language | Immutable product/policy version references |
| IAM | All domain contexts | Open Host Service + local authorization policy | Identity claims and coarse entitlements |
| Domain contexts | Regulatory Notice | Customer/Supplier | Typed notice intent and recipient snapshot |
| Regulatory Notice | Communications Platform | Customer/Supplier | Delivery request and telemetry |
| Domain contexts | Document Platform | Conformist at storage boundary | Immutable document version API |
| Domain contexts | Rules Platform | Conformist behind domain adapter | Typed policy specification and execution trace |
| Integration adapters | External systems | Anti-Corruption Layer | Vendor-specific APIs translated to domain contracts |
| Domain contexts | Search and Analytics | Published Language | Versioned domain events |

## Shared-kernel policy

No business-domain shared kernel is proposed. Shared code is limited to technical primitives such as:

- message envelope and correlation metadata;
- tenant and jurisdiction identifiers;
- time and money primitives where semantics are truly identical;
- contract serialization and observability libraries.

Even these primitives require narrow ownership and compatibility rules. Sharing domain entities,
ORM models, repositories, status enumerations, or validation rules across contexts is prohibited.

## Aggregate candidates

These are discovery hypotheses, not implementation commitments.

| Context | Aggregate candidate | Consistency boundary |
|---|---|---|
| Regulatory Product | `RegulatoryProductVersion` | Publication state, effective scope, and requirement consistency |
| Application Intake | `DraftApplication` | Applicant editing, completeness, declarations, and submission transition |
| Application Intake | `QuestionnaireVersion` | Published questionnaire structure and requirement mappings |
| Case Management | `RegulatoryCase` | Case state, information requests, assignments, adjudication, and decision |
| Regulatory Process | `ProcessDefinitionVersion` | Immutable executable process definition |
| Regulatory Process | `ProcessInstance` | Current step, correlation, timers, and process outcomes |
| Fees and Receivables | `FeeAssessment` | Charge calculation and assessment state |
| Regulatory Review | `Review` | Assignment, findings, recommendation, and completion |
| Inspection Management | `Inspection` | Scheduling, checklist version, observations, findings, and outcome |
| Regulatory Notice | `Notice` | Official content, recipients, service requirements, and service status |
| Credential Lifecycle | `Credential` | Standing, conditions, effective period, suspension, and revocation |

## First tracer slice

Migrate one high-volume licensing family through:

```text
Regulatory Product Version
  -> Application Questionnaire
  -> Draft and Evidence
  -> Immutable Submission
  -> Submission Event
  -> Legacy Case Adapter or new Case opening seam
```

The tracer proves:

- versioned product requirements;
- configurable but typed metadata;
- reusable form rendering;
- immutable submissions and amendments;
- document references;
- party snapshots;
- published contracts;
- coexistence with the legacy case implementation.

Illustrative synthetic selection weights for the reference Product Family:

| Criterion | Weight |
|---|---:|
| Reuse across customers | 25% |
| Current defect/support burden | 20% |
| Form duplication and custom-code volume | 20% |
| Transaction volume/customer visibility | 15% |
| Dependency tractability | 10% |
| Reference-customer willingness | 10% |

Avoid both the simplest form, which proves little, and the most complex workflow, which creates
uncontrolled migration risk.
