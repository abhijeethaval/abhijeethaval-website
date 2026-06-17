namespace AbhijeetSite.Api.Features.Articles.CreateArticleDraft;

/// <summary>
/// Input for creating an article draft.
/// </summary>
/// <param name="Title">Draft title.</param>
/// <param name="Slug">URL-safe draft slug.</param>
/// <param name="Summary">Draft summary.</param>
/// <param name="MdxSource">Owner-authored MDX source.</param>
public sealed record CreateArticleDraftCommand(
    string Title,
    string Slug,
    string Summary,
    string MdxSource);
