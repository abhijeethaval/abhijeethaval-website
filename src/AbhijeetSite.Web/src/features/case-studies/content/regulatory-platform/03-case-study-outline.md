# Case-Study Outline

Target format: a concise public Markdown narrative supported by architecture diagrams, decision
records, and validation hypotheses.

| Section | Purpose | Supporting artifacts |
|---|---|---|
| 1. Executive thesis | Establish the transformation argument and measurable outcomes | One-page strategy |
| 2. Current-state assessment | Show a repeatable method for measuring divergence | Heat map, divergence taxonomy |
| 3. Domain architecture | Identify subdomains, bounded contexts, and relationships | Domain landscape and context map |
| 4. Target platform architecture | Define stable domain capabilities and governed variability | Container and interaction diagrams |
| 5. Customization governance | Turn architecture policy into delivery decisions | Decision tree, approval workflow, examples |
| 6. Migration roadmap | Demonstrate practical sequencing under constraints | Waves, milestones, dependencies, scorecard |
| 7. Engineering foundations | Make shared capabilities adoptable and operable | Golden path, IDP, CI/CD, observability, API standards |
| 8. Operating model | Clarify decision rights and team interactions | Team topology, RACI, governance cadence |
| 9. Risks and trade-offs | Demonstrate architectural judgment | Risk register, rejected alternatives |
| 10. Validation backlog | Identify next validation steps | Discovery backlog |

## Working narrative

1. Customer-specific code is an unmanaged variability mechanism.
2. The target is not "one workflow for every jurisdiction"; it is a shared set of bounded-context
   capabilities with explicit, testable variation.
3. Customer workflows remain tailored but become thin orchestration over stable domain contracts.
4. Migration proceeds by bounded capability and workflow seam, not by horizontal rewrite.
5. Governance is embedded in product intake, architecture decisions, CI/CD, and platform telemetry.
6. Success is measured by adoption, convergence, delivery performance, and customer outcomes.
