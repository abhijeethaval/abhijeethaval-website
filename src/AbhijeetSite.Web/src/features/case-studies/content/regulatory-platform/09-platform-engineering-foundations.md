# Platform Engineering Foundations

## Platform engineering objective

Make the safe shared-core path the fastest delivery path. Platform engineering provides paved roads,
not a central team that implements every customer feature.

The platform product has five initial capabilities:

1. Software catalog and ownership
2. Golden-path templates and reusable delivery components
3. Contract and configuration governance
4. Secure CI/CD and progressive delivery
5. Observability, reliability, and operational self-service

## Golden paths

### 1. Bounded-context module or service

Creates:

- module/service skeleton in the existing stack;
- owner and bounded-context metadata;
- domain, application, adapter, and contract boundaries;
- module-owned persistence and migration structure;
- health/readiness endpoints;
- OpenAPI or AsyncAPI skeleton;
- architecture, unit, contract, integration, and smoke tests;
- OpenTelemetry instrumentation;
- CI/CD pipeline, security scanning, runbook, and SLO template;
- catalog registration and documentation.

Default output is a module. Independent deployment is an explicit architectural decision.

### 2. Domain configuration package

Creates:

- typed schema and namespace;
- applicability and effective-date model;
- draft/review/publish/deprecate lifecycle;
- validation and reference checks;
- preview and simulation environment;
- migration and rollback hooks;
- audit metadata;
- promotion pipeline and compatibility tests.

### 3. Integration adapter

Creates:

- anti-corruption translation boundary;
- typed input/output contract;
- secret and credential integration;
- idempotency, timeout, retry, circuit-breaking, and reconciliation;
- dead-letter handling;
- security and privacy checklist;
- synthetic probe, dashboard, alert, and runbook;
- vendor sandbox test harness.

### 4. Regulatory process definition

Creates:

- versioned process-definition package;
- stable command/event references;
- timers, escalation, retry, and compensation policies;
- configuration validation;
- simulation and replay tests;
- process telemetry and support view;
- runtime deployment descriptor hidden behind the Process context.

### 5. Data projection

Creates:

- subscribed domain-event contracts;
- idempotent projector;
- rebuild and backfill mechanism;
- data classification and retention controls;
- freshness and completeness metrics;
- read-only API or query model.

## Internal developer platform strategy

Treat the IDP as a product and start with workflows, not a portal procurement.

### Minimum viable IDP

| Capability | Purpose |
|---|---|
| Software catalog | Components, contexts, APIs, events, owners, criticality, SLOs, dependencies |
| Golden-path creation | Generate compliant modules, adapters, configuration, and projections |
| Documentation | Context maps, ADRs, APIs/events, runbooks, onboarding |
| Scorecards | Ownership, tests, telemetry, security, contract, and lifecycle compliance |
| Environment self-service | Ephemeral test environments and approved deployment actions |
| Operational view | Deployments, SLOs, incidents, traces, logs, and runbooks |
| Exception visibility | Custom exceptions, expiry, convergence target, and sponsor |

A developer portal such as Backstage may implement catalog and software-template capabilities, but
tool selection follows workflow validation. The Regulatory Platform should not adopt a large portal
before proving
that templates, catalog metadata, and scorecards remove measurable developer friction.

### Catalog entity minimum

Every component records:

- bounded context and subdomain classification;
- owner and support channel;
- repository, build, and deployable artifact;
- APIs, events, dependencies, and consumers;
- data classification and system-of-record status;
- tenant and jurisdiction scope;
- SLOs, dashboards, alerts, and runbook;
- lifecycle: experimental, active, deprecated, retired;
- custom exceptions and known architectural debt.

## CI/CD standard

### Source and change model

- Trunk-based development with short-lived branches.
- No permanent customer branches.
- Customer behavior is supplied by versioned definitions, configuration, extensions, or adapters.
- Every change links to its owning context and classification.
- CODEOWNERS or equivalent enforce context ownership.
- ADRs are required for boundary, contract, persistence, and new extension-point decisions.

### Pipeline stages

```mermaid
flowchart LR
    A["Source"] --> B["Format, lint, compile"]
    B --> C["Unit and domain behavior"]
    C --> D["Architecture and dependency rules"]
    D --> E["API, event, and consumer contracts"]
    E --> F["Configuration and migration validation"]
    F --> G["Security, license, secret, and supply-chain checks"]
    G --> H["Package once: artifact, SBOM, provenance"]
    H --> I["Ephemeral integration environment"]
    I --> J["Smoke, journey, resilience tests"]
    J --> K["Progressive production deployment"]
    K --> L["SLO and business-outcome verification"]
```

### Required controls

- deterministic build and dependency lock;
- artifact signing, SBOM, and build provenance;
- static analysis, dependency, secret, and infrastructure scanning;
- architecture tests for module boundaries and persistence access;
- OpenAPI/AsyncAPI linting and breaking-change detection;
- consumer-driven contract verification;
- database expand/migrate/contract checks;
- configuration schema, reference, policy, and effective-date validation;
- automated smoke and synthetic regulatory journeys;
- progressive delivery with observable health gates;
- automatic halt or rollback for technical regressions;
- auditable approvals for production configuration and regulated definitions.

Build once and promote the same artifact. Rebuilding per customer recreates deployment divergence.

## Release and configuration model

| Artifact | Versioned independently | Promotion requirement |
|---|---|---|
| Application/module artifact | Yes | Technical pipeline and progressive deployment |
| Regulatory product version | Yes | Domain approval, effective date, compatibility validation |
| Questionnaire version | Yes | Requirement traceability, accessibility, preview, acceptance |
| Process definition | Yes | Simulation, contract compatibility, domain approval |
| Policy specification | Yes | Scenario suite, explanation, effective date, approval |
| Integration adapter configuration | Yes | Connectivity, mapping, security, reconciliation test |
| Feature flag | Temporary lifecycle | Owner, purpose, expiry, telemetry |

Feature flags separate deployment from exposure. They are not a substitute for tenant configuration,
regulatory policy, or permanent product variation. Expired flags fail the engineering scorecard.

## Test architecture

### Within a bounded context

- domain behavior tests for invariants and typed errors;
- aggregate and domain-service tests;
- application-command tests;
- persistence adapter integration tests;
- authority and audit tests;
- property-based tests for complex policy and metadata boundaries where valuable.

### Across contexts

- schema compatibility tests;
- provider and consumer contract tests;
- event fixture tests;
- process simulation;
- duplicate, delayed, reordered, missing, and poison-message tests;
- synthetic critical journeys;
- minimal end-to-end tests for highest-risk regulatory outcomes.

Avoid relying on a large brittle end-to-end suite. Contract tests verify each integration boundary in
isolation, while a small journey suite verifies the assembled platform.

### Customer configuration

Each supported customer configuration produces an executable test pack:

- product and questionnaire validity;
- policy scenarios;
- process graph reachability and terminal-state checks;
- authority matrix;
- integration mappings;
- representative golden submissions;
- upgrade compatibility.

The pack is generated from configuration and runs before publication and every shared-core release.

## Observability model

Use vendor-neutral instrumentation and propagate one correlation context through synchronous APIs,
messages, workflow steps, and external adapters.

### Technical signals

- traces for request and process paths;
- metrics for rates, errors, latency, saturation, queue depth, and event age;
- structured logs for diagnostic events;
- profiles where supported and operationally justified.

OpenTelemetry is the preferred instrumentation standard because it defines vendor-neutral signals
and context propagation. Backend vendor selection remains replaceable.

### Required correlation attributes

- trace and span identifiers;
- tenant identifier;
- jurisdiction identifier;
- bounded context and component;
- process instance identifier;
- case, submission, notice, assessment, or credential identifier where applicable;
- contract and definition version;
- deployment and artifact version.

Do not place applicant answers, document content, secrets, or unnecessary personal information in
telemetry.

### Business and domain signals

| Journey | Signals |
|---|---|
| Product publication | Validation failures, publication latency, active versions |
| Intake | Draft completion, validation failures, submission success, amendment turnaround |
| Process | Step duration, waiting reason, overdue tasks, retry and compensation count |
| Case | Queue age, information requests, decision cycle time, authority rejection |
| Integration | Success, latency, timeout, reconciliation backlog, duplicate outcome |
| Notice | Delivery attempts, proof-of-service status, deadline risk |
| Credential | Issuance latency, failed prerequisites, standing changes |

### Audit versus observability

Observability data supports diagnosis and reliability. The regulatory audit trail is a separate
domain record that captures:

- actor and authority;
- command and decision;
- evidence and policy versions;
- before/after domain state where required;
- timestamp and correlation;
- explanation and approval;
- immutable record retention.

Logs are not the system of record for regulated decisions.

## SLO model

Define SLOs per user journey and customer tier, then derive component objectives.

Initial examples:

| Journey | SLI | Initial objective hypothesis |
|---|---|---|
| Applicant submission | Successful durable submissions / valid attempts | 99.9% monthly |
| Submission acknowledgement | Time to durable acknowledgement | 95% below 3 seconds |
| Process command delivery | Accepted commands delivered within threshold | 99.9% below 60 seconds |
| Event projection freshness | Age of latest processed event | 99% below 5 minutes |
| Configuration publication | Successful valid publications | 99.5% |
| Critical external adapter | Successful completed/reconciled operations | Customer/provider-specific |

These are proposed discovery values, not current commitments. Final SLOs follow customer contracts,
transaction criticality, provider constraints, and measured baseline. Error budgets control release
risk and reliability investment.

## API governance

### Contract standards

- OpenAPI for synchronous HTTP APIs.
- AsyncAPI for commands, events, and message-driven interfaces.
- JSON Schema or equivalent typed schema for configuration and message payloads.
- Domain-language operations rather than table-shaped CRUD.
- One accountable provider owner and known consumers for every contract.

### API checklist

- tenant and jurisdiction scope is explicit;
- authentication and coarse entitlement are declared;
- domain authority remains in the receiving context;
- idempotency exists for commands and mutating external requests;
- correlation identifiers propagate;
- pagination, filtering, time, money, and error semantics are standardized;
- errors distinguish validation, domain rejection, authorization, conflict, and infrastructure
  failure;
- personal and sensitive fields are classified;
- rate limits and resilience expectations are documented;
- examples and contract tests exist;
- lifecycle and deprecation date are visible in the catalog.

### Event checklist

- event name is past tense and domain meaningful;
- producer is the system of record for the fact;
- event identifier, occurred time, correlation, causation, tenant, jurisdiction, and schema version
  are present;
- consumers tolerate additive fields;
- sensitive payload is minimized;
- ordering assumptions and partition key are explicit;
- duplicate delivery is expected;
- replay and retention policy are declared;
- event evolution is compatibility-checked.

### Versioning

- Prefer additive backward-compatible change.
- Breaking HTTP contracts require a new major contract or parallel endpoint.
- Breaking event changes require a new event/schema version and migration plan.
- Deprecation must cover at least one supported annual customer upgrade cycle unless a security or
  regulatory emergency requires faster action.
- Providers may retire a contract only after cataloged consumers have migrated or accepted risk.

## Architecture and delivery scorecards

Each active component is scored on:

- accountable ownership;
- context and lifecycle classification;
- test coverage by required test type;
- architecture-boundary compliance;
- OpenAPI/AsyncAPI publication and compatibility;
- telemetry, SLO, dashboard, alert, and runbook;
- vulnerability and dependency posture;
- artifact signing, SBOM, and provenance;
- deployment automation and rollback;
- data classification, retention, and backup;
- active exception and feature-flag hygiene.

Scorecards create visibility and prioritization. They should not block teams on low-risk cosmetic
standards while critical reliability or boundary violations remain.

## Delivery performance measures

Track current DORA software delivery metrics per application or service:

- change lead time;
- deployment frequency;
- failed deployment recovery time;
- change fail rate;
- deployment rework rate.

Do not aggregate them into one organization-wide number without preserving application context.
Correlate them with platform adoption, regression incidents, customer upgrade delta, and developer
wait time.

## Adoption strategy

1. Build golden paths with the tracer team, not in isolation.
2. Measure setup time, pipeline wait, deployment friction, and support demand before and after.
3. Make the path self-service for the second customer.
4. Publish scorecards and office hours.
5. Improve the platform backlog from product-team feedback and operational evidence.
6. Deprecate legacy paths only after a viable replacement and migration support exist.

Platform success is adoption and improved delivery outcomes, not portal feature count.

## Standards references

- [DORA software delivery performance metrics](https://dora.dev/guides/dora-metrics/)
- [OpenTelemetry signals](https://opentelemetry.io/docs/concepts/signals/)
- [OpenAPI Specification](https://spec.openapis.org/oas/latest.html)
- [AsyncAPI Specification](https://www.asyncapi.com/docs/reference/specification/latest)
- [Pact contract testing](https://docs.pact.io/)
- [SLSA build provenance](https://slsa.dev/spec/v1.2/build-provenance)
- [OpenFeature](https://openfeature.dev/)
- [Backstage Software Catalog](https://backstage.io/docs/features/software-catalog/)
- [Backstage Software Templates](https://backstage.io/docs/features/software-templates/)
