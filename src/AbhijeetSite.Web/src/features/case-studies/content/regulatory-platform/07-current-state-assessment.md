# Current-State Platform Assessment

## Assessment objective

Create an evidence-based portfolio view of where customer variation is legitimate, where divergence
is accidental, and which capabilities offer the highest-value convergence path.

The unit of assessment is:

```text
Customer Deployment x Business Capability x Version
```

A repository-only assessment is insufficient. The analysis must combine code, configuration,
database schemas, workflows, operational data, incidents, support demand, and domain-expert input.

## Synthetic six-week assessment approach

The timebox and sample sizes below are fictional planning ranges, not observed delivery data.

| Week | Activity | Output |
|---:|---|---|
| 1 | Establish taxonomy, inventory sources, and metric definitions | Assessment workbook and ownership map |
| 1-2 | Scan repositories, branches, schemas, deployment manifests, and configuration | Deployment and component inventory |
| 2-3 | Mine change history, defects, incidents, and support demand | Change-coupling and regression baseline |
| 2-4 | Run event-storming and language workshops for representative customers | Domain landscape and context hypotheses |
| 3-5 | Compare workflows, forms, rules, permissions, and integrations | Capability divergence heat map |
| 5 | Score candidate tracer slices and validate dependencies | Prioritized migration options |
| 6 | Confirm target architecture, roadmap, and baseline scorecard | Executive assessment and investment decision |

Assessment and delivery overlap. During these six weeks, governance starts for new demand and CI/CD
captures baseline metrics; the organization does not wait for a perfect inventory.

## Representative sampling

Select a synthetic sample of 4-6 Customers that covers:

- high and low customization;
- different jurisdictions and regulatory product lines;
- recent and older implementation generations;
- high transaction or support volume;
- distinct identity, payment, ERP, GIS, and document integrations;
- a customer willing to act as the tracer reference.

The sample creates a hypothesis. Automated portfolio scans then test whether the findings generalize.

## Divergence dimensions

Score each customer-capability pair from `0` to `4`.

| Dimension | 0 - shared | 1 - minor | 2 - moderate | 3 - major | 4 - forked |
|---|---|---|---|---|---|
| Domain model | Same language and invariants | Additive metadata | Policy variation | Different lifecycle/invariants | Incompatible model |
| Process | Same definition | Parameters/SLA differ | Branches differ | Major sequence variation | Bespoke implementation |
| User experience | Same experience | Branding/labels | Questionnaire/layout variation | Custom screens | Separate application |
| Data model | Same schema | Additive fields | Extension tables | Custom migrations | Separate schema/model |
| Rules/policy | Same version | Effective values | Configured expressions | Custom code rules | Independent framework |
| Authorization | Same IAM and authority model | Role mappings | Domain policy variation | Custom permission code | Separate auth system |
| Integration | Shared adapter | Endpoint/config differs | Mapping differs | Custom adapter | Embedded vendor model |
| Deployment | Same artifact | Config differs | Feature set differs | Customer branch/build | Independent release train |
| Testing | Shared suite | Customer test data | Configuration tests | Custom regression pack | Manual/isolated validation |
| Operations | Shared telemetry/SLO | Dashboard filters | Custom alerts | Customer-specific runbook | Separate operations model |

The score is descriptive, not automatically negative. A process score of `3` may be legitimate
jurisdictional variation if represented through a shared process model. A deployment score of `4`
usually indicates accidental divergence.

## Divergence classification

Every identified difference receives one cause classification.

| Classification | Meaning | Intended disposition |
|---|---|---|
| Essential regulatory variation | Law, policy, authority, effective date, or jurisdiction differs | Configuration, domain policy, or process definition |
| Customer operating-model variation | Agency organization, SLA, routing, or local procedure differs | Configuration or bounded extension |
| External ecosystem variation | ERP, payment, GIS, identity, document, or notification provider differs | Integration adapter |
| Product gap | Reusable capability is missing from the shared model | Core roadmap |
| Accidental duplication | Equivalent behavior was implemented more than once | Consolidate into shared capability |
| Architectural erosion | Boundary bypass, shared-table coupling, tenant branch, or internal API use | Remediate and enforce guardrail |
| Obsolete customization | Requirement no longer applies or customer no longer uses it | Retire |
| Temporary contractual exception | Deadline forced a bounded one-off | Register, isolate, expire, converge |

## Portfolio divergence index

For capability `c`:

```text
DivergenceIndex(c) =
  Sum(customer weight x dimension weight x normalized divergence score)
  / Sum(customer weight x dimension weight)
```

Customer weighting should reflect transaction volume, annual support burden, revenue exposure, and
regulatory criticality. Dimension weights are agreed before scoring to prevent teams from changing
the model to favor a preferred migration.

The index is used with strategic value and feasibility; it is not a standalone prioritization score.

## Baseline measures

### Customization surface

Inventory every deployable component, module, script, workflow, configuration package, and adapter
with one of the five governance classifications.

Track:

```text
CustomerSpecificCodeRatio =
  executable source classified as active Custom exceptions
  / total production executable source
```

Lines of code are only an inventory proxy. The executive scorecard also tracks customer-specific
change effort, regression burden, and upgrade delta to prevent gaming through code generation or
moving behavior into scripts.

### Customer-specific engineering load

```text
CustomerSpecificEffort =
  engineering time spent on customer-specific build, test, release, and support
  / total engineering time
```

The fictional scenario uses a synthetic baseline assumption that approximately 60% of engineering
effort is Customer-specific. Validate or replace it using work-item, support, incident, and release
data rather than relying only on time sheets.

### Upgrade delta

For each customer:

- commits or patches outside the shared release line;
- customer-only database migrations;
- customer-only test cases and manual release steps;
- elapsed engineering time from shared release to customer production;
- defects caused by merge, configuration, or environment divergence.

### Regression baseline

Classify each escaped regression by:

- affected capability and customer;
- variation type;
- detection stage;
- change source;
- blast radius;
- recovery time;
- whether a contract, configuration, architecture, or regression test could have prevented it.

### Reuse

For each shared capability:

```text
Adoption = active customers using supported capability / eligible customers
ConfigurationReuse = shared artifact volume / total artifact volume
ForkAvoidance = eligible implementations delivered without customer code / eligible implementations
```

## Capability heat-map hypothesis

This synthetic initial hypothesis must be replaced by assessment evidence.

| Capability | Strategic differentiation | Observed divergence | Migration value | Initial disposition |
|---|---:|---:|---:|---|
| Regulatory Product | High | High | High | Establish versioned core model |
| Application Intake and Forms | High | Very high | Very high | First tracer |
| Regulatory Case Management | High | High | High | Establish seam after tracer |
| Regulatory Process | High | Very high | High | Introduce typed process definitions incrementally |
| Business policy/rules | High | Very high | High | Domain-owned policies plus shared execution platform |
| Credential Lifecycle | High | Medium/high | High | Follow Case and Product contracts |
| Party and Organization | Medium | Medium | Medium | Master identity plus snapshots |
| Fees and Receivables | Medium | High | Medium/high | Standard assessment contract and payment adapters |
| Review and Inspection | Medium | High | Medium | Separate domain models, migrate by process need |
| Identity and Access | Low differentiation | High implementation variance | High risk reduction | Standard platform plus domain authority |
| Integrations | Low/medium | Very high | High | Shared adapter standards; migrate opportunistically |
| Notifications/Documents | Low differentiation | Medium/high | Medium | Standard technical platforms with domain ownership |

## Architecture assessment questions

For each capability:

1. What language and invariants are stable across customers?
2. Which differences are regulatory, operational, external, accidental, or obsolete?
3. Where does customer code enter the shared execution path?
4. Which data and APIs are directly shared across supposed boundaries?
5. How is configuration versioned, tested, promoted, audited, and rolled back?
6. Which releases require customer branches or manual reconciliation?
7. Which incidents resulted from divergence rather than core defects?
8. Who owns the model, contracts, runtime, and production outcome?
9. Can a second customer adopt the capability without modifying shared code?
10. What is the safest strangler seam?

## Assessment deliverables

- Customer-capability inventory
- Divergence heat map and cause taxonomy
- Current-state context map and dependency graph
- Customer branch and database-schema topology
- Customization and exception register
- Regression and support baseline
- Capability ownership map
- Prioritized tracer scorecard
- Target context map and architecture principles
- Initial migration and retirement backlog

## Exit criteria

The assessment is sufficient to begin the tracer when:

- at least 80% of active production components have an owner and classification;
- all reference-customer capabilities and integrations are mapped;
- customer-specific code and regression baselines are reproducible;
- the tracer product family and reference customer are selected;
- Product, Intake, Document, Party, and legacy Case seams are understood;
- a rollback/cutover approach is approved;
- protected capacity and accountable owners are assigned.

The 80% threshold is an illustrative execution gate, not a claim about a current portfolio. Unknown
components are explicitly tracked rather than silently treated as shared.

