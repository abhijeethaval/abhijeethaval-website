namespace AbhijeetSite.Api.Features.Profile;

public static class ProfileContentProvider
{
    public static ProfileResponse GetProfile()
    {
        return new ProfileResponse(
            "Abhijeet Haval",
            "Principal Architect for Generative AI, Agentic AI, and enterprise distributed systems",
            "Principal / Senior Architect with 15+ years building enterprise-scale distributed systems on Azure, .NET, Python, APIs, and microservices, now focused on governed enterprise AI agents.",
            GetAbout(),
            GetExpertise(),
            GetExperiences(),
            GetEducations());
    }

    private static IReadOnlyList<string> GetAbout()
    {
        return
        [
            "I specialise in Generative AI, Agentic AI, and enterprise-scale distributed systems, with deep experience building production platforms on Azure using .NET, Python, and microservices architectures.",
            "At Icertis, I lead architecture for AI-powered enterprise products that combine LLMs, multi-agent systems, and scalable APIs for contracting, procurement, and regulated-industry workflows.",
            "My work emphasises production readiness, governance, auditability, human-in-the-loop controls, and business-aligned AI adoption across enterprise product teams."
        ];
    }

    private static IReadOnlyList<string> GetExpertise()
    {
        return
        [
            "Agentic AI",
            "AI Agents",
            "Semantic Kernel",
            "MCP",
            "A2A",
            "Azure AI Foundry",
            "HITL orchestration",
            "AI governance",
            "Enterprise SaaS",
            "Distributed systems"
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
            CreateSeniorArchitectRole()
        ];
    }

    private static RoleResponse CreatePrincipalAppliedAiRole()
    {
        return new RoleResponse(
            "Principal Architect, Applied AI Engineering",
            "Jul 2025",
            "Present",
            "Leading architecture for enterprise Agentic AI capabilities on the Icertis Contract Intelligence platform.",
            PrincipalAppliedAiAchievements,
            AgenticAiFocusAreas);
    }

    private static RoleResponse CreateSeniorAppliedAiRole()
    {
        return new RoleResponse(
            "Senior Architect, Applied AI Engineering",
            "Feb 2025",
            "Jul 2025",
            "Led architecture for GenAI and agentic capabilities that translate business intent into governed contract lifecycle actions.",
            SeniorAppliedAiAchievements,
            GovernanceFocusAreas);
    }

    private static RoleResponse CreateSeniorArchitectRole()
    {
        return new RoleResponse(
            "Senior Architect",
            "Jul 2023",
            "Feb 2025",
            "Architected the Government Contracting vertical on the Icertis platform.",
            SeniorArchitectAchievements,
            GovConFocusAreas);
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
            "IT Consultant / Senior Associate",
            "Jan 2013",
            "May 2018",
            "Worked on capital markets technology for FX Options trade processing, broker integration, message parsing, validation, and operational tooling.",
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
            "Built middleware and platform components for Proton Therapy systems in a regulated medical technology environment.",
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
            "Built critical enterprise software systems in the defense domain.",
            MastekAchievements,
            EnterpriseDeliveryFocusAreas);
    }

    private static IReadOnlyList<EducationResponse> GetEducations()
    {
        return
        [
            new EducationResponse(
                "K.I.T's College of Engineering, Kolhapur, Maharashtra",
                "Bachelor of Engineering, Electronics",
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
        "Architecting an org-wide Agentic AI Platform on top of ICI Business APIs.",
        "Designed workflow-with-agents patterns for contract type identification, attribute mapping, template selection, confirmation, and guided execution.",
        "Established HITL checkpoints, policy controls, and audit constructs as first-class AI workflow concerns.",
        "Drove Composer Agent architecture for enterprise beta customers across NDA and SOW flows.",
        "Drove Azure AI Foundry proof-of-concept strategy and integration planning."
    ];

    private static readonly string[] SeniorAppliedAiAchievements =
    [
        "Led GenAI architecture for contract intent realization on ICI Business APIs.",
        "Created risk-bounded autonomy patterns across retrieval, guided creation, and higher-trust autonomous execution.",
        "Architected an AI decision audit microservice capturing prompts, evidence, model metadata, and human approvals.",
        "Converted ambiguous AI use cases into reusable enterprise platform capabilities."
    ];

    private static readonly string[] SeniorArchitectAchievements =
    [
        "Established a framework for compliance-heavy federal contracting solutions.",
        "Designed configurable platform capabilities balancing product standardization and customer compliance needs.",
        "Translated complex federal contracting requirements into scalable platform components."
    ];

    private static readonly string[] CreditSuisseAchievements =
    [
        "Led India-side design and development for the FX Options STP Gateway.",
        "Designed FIX and FpML message parsers for trade booking workflows.",
        "Built a fluent API validation rule engine for trade booking integrity.",
        "Built a FIX Protocol message analyzer and editor recognized in an internal hackathon.",
        "Owned reference data sourcing, parsing, transformation, and UI delivery."
    ];

    private static readonly string[] VarianAchievements =
    [
        "Implemented WCF communication middleware features and component tests.",
        "Built a severity-and-verbosity logging library for diagnostics and troubleshooting.",
        "Worked in a controlled-release environment where reliability, traceability, and testing discipline were critical."
    ];

    private static readonly string[] MastekAchievements =
    [
        "Worked on Air Movement Operations workflows for flights, passengers, cargo, and arrivals.",
        "Contributed to onsite peripheral integration with barcode printers and scanners.",
        "Supported unit testing, security review, code analysis, and defect resolution."
    ];

    private static readonly string[] AgenticAiFocusAreas =
    [
        "Agentic AI",
        "Semantic Kernel",
        "MCP",
        "A2A",
        "Azure AI Foundry",
        "HITL orchestration",
        "Enterprise SaaS"
    ];

    private static readonly string[] GovernanceFocusAreas =
    [
        "GenAI architecture",
        "Risk-bounded autonomy",
        "Decision audit",
        "Explainability",
        "Enterprise AI governance"
    ];

    private static readonly string[] GovConFocusAreas =
    [
        "Government contracting",
        "Compliance platforms",
        "Product architecture",
        "Configurable SaaS"
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
