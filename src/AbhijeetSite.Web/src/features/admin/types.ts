export interface ArticleDraftSummary {
  readonly id: string;
  readonly slug: string;
  readonly title: string;
  readonly status: string;
  readonly updatedAt: string;
  readonly version: number;
  readonly isPublished: boolean;
}

export interface ArticleDraft extends ArticleDraftSummary {
  readonly summary: string;
  readonly mdxSource: string;
  readonly createdAt: string;
}

export interface CreateArticleDraftPayload {
  readonly title: string;
  readonly slug: string;
  readonly summary: string;
  readonly mdxSource: string;
}

export interface UpdateArticleDraftPayload extends CreateArticleDraftPayload {
  readonly expectedVersion: number;
}

export interface PublishArticleDraftPayload {
  readonly expectedVersion: number;
}

export interface PublishedArticleMutation {
  readonly slug: string;
  readonly title: string;
  readonly summary: string;
  readonly renderedHtml: string;
  readonly publishedAt: string;
  readonly updatedAt: string;
  readonly revision: number;
}
