namespace AbhijeetSite.Api.Infrastructure.Persistence.Migrations;

internal static class SeedArticleContent
{
    internal const string MdxSource = """
        # Building This Site as a Modular Monolith

        A portfolio can be more than a static profile. This site is structured as a
        production application whose architecture is visible in the product itself.

        ## Start with one deployable unit

        The API is a modular monolith. Profile, identity, articles, and comments own
        separate feature boundaries while sharing one operational process and database.

        ## Separate authoring from public reads

        Drafts retain MDX source. Published articles contain render-ready, sanitized HTML,
        so public traffic never compiles or executes author-authored MDX.

        ## Keep infrastructure explicit

        .NET Aspire orchestrates the React frontend, ASP.NET Core API, and PostgreSQL
        container locally. Azure Container Apps provides the production boundary.
        """;

    internal const string RenderedHtml = """
        <p>A portfolio can be more than a static profile. This site is structured as a
        production application whose architecture is visible in the product itself.</p>
        <h2>Start with one deployable unit</h2>
        <p>The API is a modular monolith. Profile, identity, articles, and comments own
        separate feature boundaries while sharing one operational process and database.</p>
        <h2>Separate authoring from public reads</h2>
        <p>Drafts retain MDX source. Published articles contain render-ready, sanitized HTML,
        so public traffic never compiles or executes author-authored MDX.</p>
        <h2>Keep infrastructure explicit</h2>
        <p>.NET Aspire orchestrates the React frontend, ASP.NET Core API, and PostgreSQL
        container locally. Azure Container Apps provides the production boundary.</p>
        """;
}
