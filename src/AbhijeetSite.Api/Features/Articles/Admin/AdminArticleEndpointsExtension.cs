using AbhijeetSite.Api.Features.Articles.CreateArticleDraft;
using AbhijeetSite.Api.Features.Identity;
using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Registers admin article endpoints.
/// </summary>
public static class AdminArticleEndpointsExtension
{
    private const string DraftsRoute = "/api/admin/articles/drafts";

    /// <summary>
    /// Maps admin article draft routes.
    /// </summary>
    public static IEndpointRouteBuilder MapAdminArticleEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup(DraftsRoute)
            .RequireAuthorization(AuthPolicies.AdminOnly);

        group.MapGet("", GetDraftsAsync).WithName("GetAdminArticleDrafts");
        group.MapGet("/{id:guid}", GetDraftAsync).WithName("GetAdminArticleDraft");
        group.MapPost("", CreateDraftAsync).WithName("CreateAdminArticleDraft");
        group.MapPut("/{id:guid}", UpdateDraftAsync).WithName("UpdateAdminArticleDraft");
        group.MapPost("/{id:guid}/publish", PublishDraftAsync).WithName("PublishAdminArticleDraft");

        return app;
    }

    private static async Task<IResult> GetDraftsAsync(
        GetArticleDraftsHandler handler,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<ArticleDraftSummaryResponse>> result =
            await handler.HandleAsync(cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error);
    }

    private static async Task<IResult> GetDraftAsync(
        Guid id,
        GetArticleDraftHandler handler,
        CancellationToken cancellationToken)
    {
        Result<ArticleDraftResponse?> result = await handler.HandleAsync(
            new ArticleDraftId(id),
            cancellationToken);
        if (result.IsFailure)
        {
            return ToProblem(result.Error);
        }

        return result.Value is null ? Results.NotFound() : Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateDraftAsync(
        CreateArticleDraftRequest request,
        CreateArticleDraftHandler handler,
        CancellationToken cancellationToken)
    {
        CreateArticleDraftCommand command = new(
            request.Title,
            request.Slug,
            request.Summary,
            request.MdxSource);
        Result<CreateArticleDraftResult> result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess ? ToCreated(result.Value) : ToProblem(result.Error);
    }

    private static async Task<IResult> UpdateDraftAsync(
        Guid id,
        UpdateArticleDraftRequest request,
        UpdateArticleDraftHandler handler,
        CancellationToken cancellationToken)
    {
        UpdateArticleDraftCommand command = new(
            new ArticleDraftId(id),
            request.Title,
            request.Slug,
            request.Summary,
            request.MdxSource,
            request.ExpectedVersion);
        Result<ArticleDraftResponse> result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error);
    }

    private static async Task<IResult> PublishDraftAsync(
        Guid id,
        PublishArticleDraftRequest request,
        PublishArticleDraftHandler handler,
        CancellationToken cancellationToken)
    {
        PublishArticleDraftCommand command = new(new ArticleDraftId(id), request.ExpectedVersion);
        Result<PublishedArticleResponse> result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error);
    }

    private static IResult ToCreated(CreateArticleDraftResult result)
    {
        ArticleDraftResponse response = new(
            result.Id.Value.ToString(),
            result.Slug.Value,
            result.Title,
            result.Summary,
            result.MdxSource,
            result.Status.ToString(),
            result.CreatedAt,
            result.UpdatedAt,
            result.Version,
            false);
        return Results.Created($"{DraftsRoute}/{response.Id}", response);
    }

    private static IResult ToProblem(Error? error)
    {
        if (error is null)
        {
            throw new InvalidOperationException("A failed result must include an error.");
        }

        Dictionary<string, object?> extensions = new()
        {
            ["code"] = error.Code,
            ["category"] = error.Category.ToString()
        };
        return Results.Problem(
            title: error.Code,
            detail: error.Message,
            statusCode: GetStatusCode(error),
            extensions: extensions);
    }

    private static int GetStatusCode(Error error)
    {
        if (error.Code == ArticlesErrors.DraftNotFoundCode)
        {
            return StatusCodes.Status404NotFound;
        }

        return error.Category switch
        {
            ErrorCategory.Validation => StatusCodes.Status400BadRequest,
            ErrorCategory.Business => StatusCodes.Status409Conflict,
            ErrorCategory.Infrastructure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
