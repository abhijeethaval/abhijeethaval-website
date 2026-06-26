import { apiClient } from '../../shared/api/apiClient';
import { PublishedArticle, PublishedArticleSummary } from './types';

export const articlesApi = {
  getPublishedArticles: async (): Promise<ReadonlyArray<PublishedArticleSummary>> => {
    return apiClient.get<ReadonlyArray<PublishedArticleSummary>>('/api/articles');
  },

  getPublishedArticle: async (slug: string): Promise<PublishedArticle> => {
    return apiClient.get<PublishedArticle>(`/api/articles/${encodeURIComponent(slug)}`);
  },
};
