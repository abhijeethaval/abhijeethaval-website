export interface PublishedArticleSummary {
  readonly slug: string;
  readonly title: string;
  readonly summary: string;
  readonly publishedAt: string;
  readonly updatedAt: string;
}

export interface PublishedArticle extends PublishedArticleSummary {
  readonly renderedHtml: string;
  readonly revision: number;
}
