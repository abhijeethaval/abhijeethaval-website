namespace AbhijeetSite.Api.Features.Profile;

public static class ProfileContentProvider
{
    public static ProfileResponse GetProfile()
    {
        return new ProfileResponse(
            "Abhijeet Haval",
            "Principal Applied AI Architect for agentic AI platforms and forward-deployed AI solutions",
            "Principal Applied AI Architect with 17 years designing distributed enterprise platforms, regulated-domain systems, and governed agentic AI products across Azure, .NET, Python, TypeScript, and enterprise APIs.",
            GetAbout(),
            GetExpertise(),
            GetExperiences(),
            GetEducations());
    }

    private static IReadOnlyList<string> GetAbout()
    {
        return
        [
            "I design and build enterprise AI platforms that turn ambiguous workflow problems into governed agentic systems with tool use, HITL controls, MCP/A2A integration, structured outputs, evaluations, and decision auditability.",
            "At Icertis, I lead applied AI architecture across agentic platform, Composer Agent, GovCon, and developer platform workstreams, with production coding ownership rather than architecture-only advisory work.",
            "My operating style combines domain-driven design, regulated enterprise delivery, rapid POCs, customer beta feedback loops, and reusable platform patterns for teams shipping AI into real customer workflows."
        ];
    }

    private static IReadOnlyList<string> GetExpertise()
    {
        return
        [
            "Agentic AI architecture",
            "Agent Harness",
            "MCP and A2A interoperability",
            "HITL orchestration",
            "AI governance and auditability",
            "Azure OpenAI",
            "Azure AI Foundry",
            "Azure AI Document Intelligence",
            "LangChain Deep Agents",
            "Microsoft Agent Framework",
            "evals",
            "Domain-driven design",
            "Event-driven microservices",
            "Multi-tenant SaaS"
        ];
    }

    private static IReadOnlyList<ExperienceResponse> GetExperiences()
    {
        return
        [
            CreateIcertisExperience(),
            CreateCreditSuisseExperience(),
            CreateVarianExperience(),
            CreateMastekExperience()
        ];
    }

    private static ExperienceResponse CreateIcertisExperience()
    {
        return new ExperienceResponse("Icertis", "Pune District, Maharashtra, India", GetIcertisRoles());
    }

    private static IReadOnlyList<RoleResponse> GetIcertisRoles()
    {
        return
        [
            CreatePrincipalAppliedAiRole(),
            CreateSeniorAppliedAiRole(),
            CreateSeniorArchitectRole(),
            CreateArchitectRole(),
            CreateAssociateArchitectRole()
        ];
    }

    private static RoleResponse CreatePrincipalAppliedAiRole()
    {
        return new RoleResponse(
            "Principal Architect, Applied AI Engineering",
            "Jul 2025",
            "Present",
            "Lead applied AI architecture for enterprise agentic platform patterns on ICI Business APIs.",
            PrincipalAppliedAiAchievements,
            AgenticAiFocusAreas);
    }

    private static RoleResponse CreateSeniorAppliedAiRole()
    {
        return new RoleResponse(
            "Senior Architect, Applied AI Engineering",
            "Jun 2024",
            "Jul 2025",
            "Led GenAI architecture for converting natural-language business intent into controlled contract actions.",
            SeniorAppliedAiAchievements,
            GovernanceFocusAreas);
    }

    private static RoleResponse CreateSeniorArchitectRole()
    {
        return new RoleResponse(
            "Senior Architect",
            "Jul 2023",
            "Jun 2024",
            "Architected the GovCon vertical on ICI for compliance-heavy federal contracting workflows.",
            SeniorArchitectAchievements,
            GovConFocusAreas);
    }

    private static RoleResponse CreateArchitectRole()
    {
        return new RoleResponse(
            "Architect",
            "Jan 2020",
            "Jul 2023",
            "Architected the Icertis Developer Network and contract-driven sourcing capabilities.",
            ArchitectAchievements,
            DeveloperPlatformFocusAreas);
    }

    private static RoleResponse CreateAssociateArchitectRole()
    {
        return new RoleResponse(
            "Associate Architect",
            "Jun 2018",
            "Jan 2020",
            "Designed and built configurable contract-driven business application capabilities.",
            AssociateArchitectAchievements,
            ContractApplicationFocusAreas);
    }

    private static ExperienceResponse CreateCreditSuisseExperience()
    {
        return new ExperienceResponse(
            "Credit Suisse via Cognizant",
            "Pune/Pimpri-Chinchwad Area",
            [CreateCreditSuisseRole()]);
    }

    private static RoleResponse CreateCreditSuisseRole()
    {
        return new RoleResponse(
            "IT Consultant / Senior Associate / Software Associate",
            "Jan 2013",
            "May 2018",
            "Led India-side design and development for FX Options trade processing and broker integration.",
            CreditSuisseAchievements,
            FinancialSystemsFocusAreas);
    }

    private static ExperienceResponse CreateVarianExperience()
    {
        return new ExperienceResponse(
            "Varian Medical Systems",
            "Pune/Pimpri-Chinchwad Area",
            [CreateVarianRole()]);
    }

    private static RoleResponse CreateVarianRole()
    {
        return new RoleResponse(
            "Software Engineer II",
            "Aug 2011",
            "Jan 2013",
            "Built WCF middleware and diagnostics capabilities for Proton Therapy systems in a regulated medical technology environment.",
            VarianAchievements,
            RegulatedSystemsFocusAreas);
    }

    private static ExperienceResponse CreateMastekExperience()
    {
        return new ExperienceResponse(
            "Mastek Ltd",
            "Mumbai Metropolitan Region",
            [CreateMastekRole()]);
    }

    private static RoleResponse CreateMastekRole()
    {
        return new RoleResponse(
            "Software Engineer",
            "Jun 2008",
            "Aug 2011",
            "Worked on defence-domain Air Movement Operations scheduling workflows.",
            MastekAchievements,
            EnterpriseDeliveryFocusAreas);
    }

    private static IReadOnlyList<EducationResponse> GetEducations()
    {
        return
        [
            new EducationResponse(
                "K.I.T's College of Engineering, Kolhapur, Maharashtra",
                "Bachelor of Engineering, Electronics, Shivaji University - First Class with Distinction",
                "2003 - 2007",
                ["Project Exhibition", "Sky observation"]),
            new EducationResponse(
                "Vivekanand Junior College, Kolhapur",
                "HSC, Science",
                "2001 - 2003",
                []),
            new EducationResponse(
                "Shahu Dayanand High School",
                "SSC",
                "1991 - 2001",
                ["Drama", "Singing", "Painting", "NCC", "National Integration Camp"])
        ];
    }

    private static readonly string[] PrincipalAppliedAiAchievements =
    [
        "Architected and coded key components of the org-wide Agentic AI Platform on top of ICI Business APIs.",
        "Defined reference architecture, tool abstractions, MCP integration, HITL controls, and agent governance models.",
        "Supported Composer Agent enterprise beta deployments through workflow validation, feedback capture, issue debugging, and solution hardening.",
        "Drove workflow-with-agents patterns for contract type selection, attribute mapping, template selection, and final confirmation.",
        "Implemented a security gate for user prompts to reduce malicious-input risk in agent workflows.",
        "Built a KPMG document extraction POC using Microsoft Agent Framework, tool calling, structured outputs, and workflow orchestration.",
        "Built MCP server and client capabilities for governed external tool and customer system access.",
        "Built the next-generation ICI agentic platform MVP with LangChain Deep Agents, MCP servers, skills, structured outputs, and evaluation-driven development.",
        "Delivered internal talks on Agentic AI and the Flipped Interaction Pattern."
    ];

    private static readonly string[] SeniorAppliedAiAchievements =
    [
        "Led the GenAI solution for contract-intent realization on ICI.",
        "Wrote production code for agent orchestration, tool execution, structured outputs, workflow integration, and enterprise API interactions.",
        "Applied Azure OpenAI, prompt engineering, tool calling, structured outputs, and evaluations to improve enterprise AI workflow reliability.",
        "Converted ambiguous business requirements into controlled agent specifications and reusable platform capabilities."
    ];

    private static readonly string[] SeniorArchitectAchievements =
    [
        "Defined the platform foundation for a compliance-heavy federal contracting solution supporting about 20 engineers.",
        "Converted complex regulatory and functional requirements into scalable workflows and platform components.",
        "Positioned regulated-domain architecture as reusable product capability rather than customer-specific custom code."
    ];

    private static readonly string[] ArchitectAchievements =
    [
        "Architected the Icertis Developer Network as a multi-tenant microservices-based developer platform.",
        "Enabled partners and ISVs to extend ICI through custom applications, APIs, workflows, and integrations.",
        "Balanced ecosystem extensibility with governance, tenant isolation, upgrade safety, and product-boundary protection.",
        "Led development of Contract-Driven Sourcing, anchoring sourcing workflows to the underlying contract."
    ];

    private static readonly string[] AssociateArchitectAchievements =
    [
        "Designed and built components of the Contract-Driven Business Application for sourcing.",
        "Translated requirements with senior architects into scalable, configurable application capabilities.",
        "Established delivery patterns that supported later platform and extension workstreams."
    ];

    private static readonly string[] CreditSuisseAchievements =
    [
        "Led India-side design and development for the FX Options STP Gateway.",
        "Integrated inter-dealer broker flows from Bloomberg and BGC for multi-leg FX options strategies.",
        "Designed FIX and FpML message parsers for trade booking workflows.",
        "Built a fluent validation-rule engine to improve trade-booking integrity.",
        "Built a FIX Protocol analyzer and editor recognized with second prize in a Credit Suisse internal hackathon.",
        "Owned reference data sourcing, parsing, transformation, and UI delivery."
    ];

    private static readonly string[] VarianAchievements =
    [
        "Implemented WCF communication middleware features and component tests.",
        "Built a reusable severity-and-verbosity logging library adopted across the application.",
        "Worked on a regulated cancer-treatment device where reliability, traceability, and testing discipline were critical."
    ];

    private static readonly string[] MastekAchievements =
    [
        "Worked on Air Movement Operations workflows for flights, passengers, cargo, and arrivals.",
        "Contributed to onsite peripheral integration with barcode printers and scanners.",
        "Supported unit testing, security review, code analysis, and defect resolution."
    ];

    private static readonly string[] AgenticAiFocusAreas =
    [
        "Agentic AI Platform architecture",
        "Agent Harness",
        "MCP server/client integration",
        "A2A interoperability",
        "LangChain Deep Agents",
        "Microsoft Agent Framework",
        "Evaluations",
        "HITL orchestration",
        "AI governance"
    ];

    private static readonly string[] GovernanceFocusAreas =
    [
        "GenAI architecture",
        "Azure OpenAI",
        "Structured outputs",
        "Semantic data Modeling for AI",
        "Evaluation-driven development"
    ];

    private static readonly string[] GovConFocusAreas =
    [
        "Government contracting",
        "Product architecture",
        "Orchestration Saga patterns",
        "Configurable SaaS"
    ];

    private static readonly string[] DeveloperPlatformFocusAreas =
    [
        "Developer platforms",
        "Microservices",
        "Event-driven architecture",
        "Multi-tenant SaaS",
        "Partner extensibility"
    ];

    private static readonly string[] ContractApplicationFocusAreas =
    [
        "Contract-driven applications",
        "Sourcing workflows",
        "Configurable capabilities",
        "Enterprise delivery"
    ];

    private static readonly string[] FinancialSystemsFocusAreas =
    [
        "Financial systems",
        "FIX",
        "FpML",
        "Integration architecture",
        "Validation engines",
        "Trade processing"
    ];

    private static readonly string[] RegulatedSystemsFocusAreas =
    [
        "Medical technology",
        "WCF",
        "Middleware",
        "Observability",
        "Regulated systems"
    ];

    private static readonly string[] EnterpriseDeliveryFocusAreas =
    [
        "Defense systems",
        "Enterprise applications",
        "Peripheral integration",
        "Quality practices",
        "Customer delivery"
    ];
}
