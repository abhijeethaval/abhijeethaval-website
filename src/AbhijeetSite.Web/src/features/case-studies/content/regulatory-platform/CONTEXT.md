# Context and Canonical Language

## Case-study boundary

This repository describes an explicitly fictional regulatory-platform modernization. It does not
represent a real organization, implementation, engagement, or employment history. Every name,
system description, metric, customer cohort, and timeline is a neutral construct used to examine
architecture trade-offs.

## Canonical terms

| Term | Meaning | Boundary rule |
|---|---|---|
| **Regulatory Platform** | The fictional software product family that supports regulatory intake, adjudication, inspection, payment, notice, and credential lifecycles | Use this term for the system as a whole; it is not an organization or product name |
| **Customer** | An organization that contracts for and governs use of the Regulatory Platform | A Customer is a commercial/organizational concept, not a deployment or legal authority |
| **Tenant** | An operational isolation and configuration boundary within the platform | A Customer may have one or more Tenants; tenancy must not be used as a synonym for Jurisdiction |
| **Jurisdiction** | The legal or regulatory authority and scope under which policy applies | Regulatory rules, effective dates, and authority are Jurisdiction-scoped even when infrastructure is Tenant-scoped |
| **Shared Core** | Reusable, supported product behavior and contracts maintained once for all eligible adopters | Shared Core does not mean one universal workflow, database, or deployment |
| **Tracer Capability** | A narrow, production-grade, end-to-end capability used to prove target boundaries, contracts, operability, migration, and reuse | A Tracer Capability is not a prototype; the second Customer adoption is the reuse test |
| **Bounded Context** | A domain boundary with its own language, invariants, model, ownership, and published contracts | Contexts do not share writable domain models or bypass contracts through cross-schema access |
| **Product Family** | A related set of regulated offerings with comparable lifecycle and policy needs | It is a migration cohort, not a universal domain model |
| **Configuration** | Typed, declarative variation anticipated by a Bounded Context | Configuration cannot introduce hidden invariants, arbitrary code, or a new lifecycle |
| **Extension** | Additional behavior executed through a named, versioned, governed contract | Extensions have explicit owners, compatibility tests, resource limits, and retirement paths |
| **Custom Exception** | Customer-specific executable behavior outside supported variation mechanisms | It is time-bounded debt with a sponsor, cost, expiry, and convergence plan |
| **Legacy Estate** | Existing implementations that coexist with Shared Core during incremental migration | Legacy records and in-flight work remain authoritative until an explicit migration or retirement decision |

## Scope relationships

```text
Customer
  -> owns or sponsors one or more Tenants
Tenant
  -> isolates runtime data, configuration, and operations
Jurisdiction
  -> determines legal authority, policy applicability, and effective dates
Product Family
  -> packages regulated offerings adopted within those scopes
```

Customer, Tenant, and Jurisdiction identifiers must remain explicit in contracts. No single
`customerId` field may silently stand for all three concepts.

## Metrics convention

All numbers are synthetic unless a document explicitly identifies them as a measurement to be
collected during discovery. Labels have the following meanings:

- **Synthetic target**: a fictional outcome threshold used to make a decision falsifiable.
- **Synthetic scale indicator**: an order-of-magnitude assumption used to test architecture shape.
- **Illustrative gate**: a proposed control threshold to calibrate after a measured baseline exists.
- **Observed baseline**: prohibited in this case study unless produced from an explicitly fictional
  dataset included in the repository.

## Narrative rules

- Describe architecture choices in terms of the fictional Regulatory Platform.
- Keep personal experience, résumé history, employer names, product names, and assignment provenance
  outside the repository.
- Prefer Customer, Tenant, and Jurisdiction according to the distinctions above.
- Treat diagrams, plans, and examples as hypotheses to validate, not claims of implementation.
- Publish Markdown only; generated and binary artifacts remain outside the public boundary.
