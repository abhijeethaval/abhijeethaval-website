# Shared-Core Migration Roadmap

## Roadmap thesis

Migrate by vertical business capability and customer cohort, not by rewriting horizontal technical
layers.

The first tracer is:

```text
Regulatory Product Version
  -> Application Questionnaire
  -> Draft, Party Reference, and Evidence
  -> Immutable Submission
  -> Published Submission Contract
  -> Anti-Corruption Adapter into Legacy Case Processing
```

The first customer proves production fitness. The second customer proves shared-core reuse.

## Synthetic planning assumptions

All capacity ranges, targets, cohort sizes, and dates in this roadmap are fictional planning
hypotheses. Assessment evidence must calibrate them before delivery commitment.

- Protect 20-25% of relevant engineering capacity for platform convergence during the first two
  quarters. This is an initial planning range and must be replaced with an explicit capacity model.
- Continue annual commitments by delivering shared-core work through committed product slices.
- Use existing languages, hosting, databases, and delivery tooling unless a component cannot meet a
  required capability.
- Begin with enforceable modules and contracts; extract services only when operational evidence
  justifies it.
- Do not migrate all historical data or in-flight work by default.
- Fund platform capabilities as products with owners, adoption measures, and support expectations.

If protected capacity is repeatedly consumed by Customer escalation, the synthetic 50% reduction
target is
not credible. Leadership must choose the target or the capacity; architecture cannot compensate for
an unfunded transformation.

## Synthetic outcomes and timeline

| Outcome | Proposed target |
|---|---|
| Customer-specific code | Reduce 50% from assessed baseline by month 18 |
| Release regression incidents | Reduce 40% from assessed baseline by month 12 |
| Customer-specific engineering effort | Reduce from approximately 60% to at most 40% by month 18 |
| First major shared capability | Production for reference customer by month 7 |
| Reuse proof | Same capability in production for second customer by month 10 without a customer fork |
| Governance | 100% of new non-trivial demand classified from month 2 |
| Exception control | All active custom exceptions owned, costed, and expiring by month 4 |
| Platform adoption | All new modules/adapters/process definitions use golden paths by month 6 |

These are synthetic portfolio goals. Baselines and exact Customer cohorts are confirmed during a
fictional assessment.

## Roadmap

### Phase 0 - Mobilize and stop new divergence (month 0-2)

Objectives:

- establish evidence and decision ownership;
- prevent new unmanaged customization;
- select the tracer and reference customer.
- route in-flight and new customer demand without turning the tracer into a bottleneck.

Deliverables:

- customer-capability inventory and divergence heat map;
- customization taxonomy, intake workflow, and exception register;
- target domain landscape and context map;
- component catalog with owners and criticality;
- regression, delivery, and customization baselines;
- tracer scorecard and signed reference-customer scope;
- protected capacity and named context owners;
- architecture tests for obvious cross-module violations;
- initial API/event and telemetry standards.
- new-demand routing policy for configuration, extension, integration, core, and temporary custom
  exceptions.

Exit gate:

- baseline is reproducible;
- tracer dependencies and legacy seam are understood;
- no new unclassified customer code enters production.
- urgent non-tracer requirements have a classified route: existing mechanism, shared-core work,
  deferred backlog, or time-boxed custom exception.

### Phase 1 - Establish minimum platform foundations (month 2-4)

Objectives:

- make the preferred path cheaper than customer-specific implementation;
- create a deployable skeleton for the tracer.

Deliverables:

- Product and Intake module boundaries;
- versioned configuration publication pipeline;
- Forms Platform minimum schema, renderer, and accessibility primitives;
- Document and Party adapters;
- OpenAPI/AsyncAPI contract registry;
- outbox/inbox messaging library or equivalent;
- standard telemetry and correlation;
- service/module, adapter, and configuration golden-path templates;
- contract, architecture, configuration, and migration test harnesses;
- legacy Case anti-corruption adapter contract.

Exit gate:

- a generated skeleton passes the complete delivery pipeline;
- Product requirements can generate a versioned Intake questionnaire;
- a submission contract is validated end to end in a non-production environment.

### Phase 2 - Build and prove the tracer (month 4-7)

Objectives:

- deliver one real product family end to end;
- preserve existing operations and release commitments.

Deliverables:

- published Regulatory Product version;
- versioned questionnaire and metadata;
- draft, evidence, declaration, submission, and amendment behavior;
- immutable submission package and party snapshot;
- reliable submission event;
- legacy Case adapter;
- operational dashboards, SLOs, runbooks, and audit trail;
- characterization and parity tests against the legacy path;
- controlled production rollout for the reference customer.

Cutover policy:

- existing submitted applications and cases remain on the legacy path;
- existing drafts complete on legacy unless migration is individually justified;
- new drafts for the selected product/customer use the new Intake path;
- routing is controlled by a release flag, not permanent tenant branching;
- every submitted package is retained even if downstream processing fails.

Exit gate:

- production submission, amendment, and legacy Case handoff work reliably;
- no critical data-loss or legal-record defect;
- support and rollback procedures have been exercised;
- tracer meets agreed reliability and performance SLOs for four consecutive weeks.

### Phase 3 - Prove reuse with a second customer (month 7-10)

Objectives:

- distinguish shared-core from a new one-customer implementation;
- expose missing configuration and extension mechanisms.

Deliverables:

- second customer and jurisdiction configured on the same Product/Intake core;
- variation classified as configuration, extension, integration, or core enhancement;
- no customer branch or direct domain fork;
- migration factory playbook;
- configuration-difference report;
- reusable customer acceptance pack;
- first duplicate form families retired.

Exit gate:

- second customer is live;
- at least 80% of implementation artifacts are reused unchanged;
- customer differences are represented through approved mechanisms;
- upgrade and regression effort is lower than the legacy baseline.

The 80% reuse gate is proposed to make "shared" falsifiable; it is calibrated after the assessment.

### Phase 4 - Scale and reduce regression risk (month 10-12)

Objectives:

- expand the proven pattern;
- reach the 12-month regression target.

Deliverables:

- 3-5 additional product families prioritized by value and dependency fit;
- self-service configuration authoring and preview;
- compatibility matrix across supported product, questionnaire, and contract versions;
- progressive delivery and automated rollback standards;
- standardized regression packs derived from representative customer configurations;
- event-fed operational reporting;
- removal or expiry of high-risk exceptions;
- monthly architecture and platform scorecard.

Exit gate:

- release regression incidents are 40% below baseline on a rolling comparable period;
- all shared-core releases have contract, configuration, architecture, security, and smoke gates;
- support teams can diagnose a submission across contexts through one correlation trail.

### Phase 5 - Expand shared domain capabilities (month 12-18)

Objectives:

- achieve the customer-specific code reduction target;
- extend the pattern beyond Intake.

Prioritized sequence:

1. Regulatory Case opening and information-request seam
2. Configurable Regulatory Process for selected workflow families
3. Credential issuance and renewal
4. Domain-owned policy migration to the Rules Platform
5. High-volume fee, inspection, notice, and integration adapters

Deliverables:

- customer cohorts migrated using the factory playbook;
- legacy form/rule/workflow implementations retired by explicit inventory item;
- context-owned APIs and events replace database coupling;
- high-cost exceptions converted or removed;
- customer release trains converge on supported platform versions;
- 50% customer-specific code reduction from baseline.

Exit gate:

- target reduction is evidenced by inventory and repository classification;
- at least one complete regulatory journey uses shared Product, Intake, Process, Case, and Credential
  contracts;
- remaining exceptions have funded convergence or explicit commercial acceptance.

## Migration mechanics

### Strangler seams

| Legacy dependency | Migration seam |
|---|---|
| Customer-specific form screens | Route new drafts to versioned Intake questionnaire |
| Form tables read by Case | Immutable submission contract and Case adapter |
| Embedded rules | Domain operation backed by versioned policy adapter |
| Workflow manipulating statuses | Typed commands and outcome events |
| Shared database reporting | Event-fed projection |
| Vendor-specific integration in domain code | Anti-corruption adapter |
| Customer branch deployment | Shared artifact plus versioned configuration |

### Data strategy

- Preserve legacy records in place as historical systems of record until retention permits archival.
- Migrate reference data and definitions before transactional records.
- Prefer migration by business lifecycle boundary: new draft, new case, renewal, or new credential.
- Avoid dual writes. Where coexistence requires synchronization, define one authority and reconcile
  asynchronously.
- Validate data with counts, checksums, domain invariants, and sampled evidentiary comparison.
- Record source identifiers and migration provenance.
- Make migration idempotent and restartable.

### Release strategy

- Build one immutable artifact and promote it across environments.
- Keep tenant configuration and domain definitions as separate versioned artifacts.
- Use feature flags for release exposure and rollback, not long-lived customer policy.
- Use canary cohorts by tenant, product, and new business lifecycle.
- Apply expand/migrate/contract database changes.
- Retain a tested kill switch for new entry points while preserving already accepted legal records.

### Testing strategy

| Test layer | Purpose |
|---|---|
| Characterization | Capture existing behavior before changing seams |
| Domain behavior | Verify invariants and outcomes within each context |
| Architecture | Enforce module and persistence boundaries |
| Contract/schema | Verify OpenAPI, AsyncAPI, and event compatibility |
| Consumer-driven contract | Verify real consumer expectations |
| Configuration | Validate schemas, references, effective dates, and migrations |
| Migration rehearsal | Prove idempotency, reconciliation, and rollback |
| Customer acceptance pack | Reuse scenarios across representative configurations |
| Synthetic journey | Continuously test submission-to-case critical path |
| Resilience | Verify retry, duplicate, timeout, poison-message, and recovery behavior |

## Capacity model

Use a stable allocation rather than ad hoc borrowing:

| Capacity | Purpose |
|---:|---|
| 65-70% | Contractual features, customer commitments, and product delivery |
| 20-25% | Shared-core migration and platform capabilities |
| 10% | Reliability, security, and exception retirement |

The percentages are an initial portfolio model, not individual utilization targets. Product work that
ships through the tracer counts toward both customer delivery and platform convergence.

## Dependencies

- Executive agreement on protected capacity and exception authority
- Product and domain-expert availability
- Reference-customer participation
- Repository, deployment, incident, and work-item data access
- Clear ownership of Product, Intake, Case, and platform capabilities
- Reliable contract and telemetry infrastructure
- Security/privacy review for metadata, evidence, and multi-tenant scope
- Customer migration and release communication
- Support and operations readiness

## Principal risks and mitigations

| Risk | Consequence | Mitigation |
|---|---|---|
| Shared core becomes lowest-common-denominator | Core cannot express real regulation | Rich context models plus governed policy/configuration |
| Configuration becomes programming language | Hidden untestable customer code | Typed schemas, no arbitrary code, complexity thresholds |
| First tracer is over-customized | New fork disguised as platform | Second-customer gate and reuse measure |
| Platform team becomes delivery bottleneck | Product teams bypass standards | Self-service golden paths and federated ownership |
| Legacy/new dual operation persists | Cost and cognitive load grow | Retirement item and deadline for every migrated component |
| Cross-context events become inconsistent | Process failures and data ambiguity | Contract ownership, schema compatibility, outbox/inbox |
| Protected capacity is raided | Roadmap misses outcomes | Executive scorecard and explicit target/capacity trade-off |
| Data migration damages legal records | Regulatory and customer risk | Lifecycle-boundary migration, immutable snapshots, rehearsal |
| Too many microservices too early | Operational complexity consumes capacity | Modular-first deployment policy |
| Metrics are gamed | Apparent progress without economic value | Triangulate code, effort, adoption, incidents, and upgrade delta |

## Trade-offs

| Decision | Benefit | Cost accepted |
|---|---|---|
| Modular-first architecture | Faster migration in existing stack | Less deployment autonomy initially |
| Intake-first tracer | Visible value and strong reuse signal | Case/workflow duplication persists temporarily |
| New-work migration | Low data and operational risk | Legacy remains for historical/in-flight work |
| Domain-owned configuration | Preserves language and invariants | Multiple extension vocabularies and tooling adapters |
| Async cross-context coordination | Autonomy and long-running resilience | Eventual consistency and operational sophistication |
| No universal canonical model | Context integrity and adapter isolation | More explicit translation contracts |

## Executive scorecard

Review monthly:

- customer-specific code ratio and engineering effort;
- active, expired, and retired custom exceptions;
- eligible-customer adoption by shared capability;
- implementation reuse for each new customer;
- upgrade delta and release lag per customer;
- regression incidents and escaped defects by variation type;
- deployment frequency, change lead time, change fail rate, failed-deployment recovery time, and
  deployment rework rate per application;
- platform golden-path adoption and developer wait time;
- architecture decision lead time;
- tracer milestone confidence, risks, and capacity variance.
