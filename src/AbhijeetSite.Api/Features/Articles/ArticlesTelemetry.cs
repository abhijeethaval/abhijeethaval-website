using System.Diagnostics;
using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles;

internal static class ArticlesTelemetry
{
    internal const string CreateDraftActivityName = "articles.create_draft";
    internal const string GetDraftActivityName = "articles.get_draft";
    internal const string GetDraftsActivityName = "articles.get_drafts";
    internal const string GetPublishedArticleActivityName = "articles.get_published_article";
    internal const string GetPublishedArticlesActivityName = "articles.get_published_articles";
    internal const string PublishDraftActivityName = "articles.publish_draft";
    internal const string SourceName = "AbhijeetSite.Api.Articles";
    internal const string UpdateDraftActivityName = "articles.update_draft";

    private const string DraftIdAttribute = "app.article.draft.id";
    private const string ErrorTypeAttribute = "error.type";
    private const string ResultCategoryAttribute = "app.result.category";

    private static readonly ActivitySource Source = new(SourceName);

    internal static Activity? Start(string activityName)
    {
        return Source.StartActivity(activityName, ActivityKind.Internal);
    }

    internal static Activity? StartForDraft(string activityName, ArticleDraftId id)
    {
        Activity? activity = Start(activityName);
        activity?.SetTag(DraftIdAttribute, id.Value.ToString());
        return activity;
    }

    internal static Result<TValue> Complete<TValue>(Activity? activity, Result<TValue> result)
    {
        if (result.IsSuccess)
        {
            activity?.SetTag(ResultCategoryAttribute, "success");
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }

        Error error = result.Error
            ?? throw new InvalidOperationException("A failed result must include an error.");
        activity?.SetTag(ResultCategoryAttribute, error.Category.ToString().ToLowerInvariant());
        activity?.SetTag(ErrorTypeAttribute, error.Code);
        if (error.Category == ErrorCategory.Infrastructure)
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.Code);
        }

        return result;
    }

    internal static void SetDraftId(Activity? activity, ArticleDraftId id)
    {
        activity?.SetTag(DraftIdAttribute, id.Value.ToString());
    }

    internal static void RecordException(Exception exception)
    {
        Activity? activity = Activity.Current;
        activity?.AddEvent(new ActivityEvent(
            "exception",
            tags: new ActivityTagsCollection
            {
                ["exception.type"] = exception.GetType().FullName
            }));
        activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
    }
}
