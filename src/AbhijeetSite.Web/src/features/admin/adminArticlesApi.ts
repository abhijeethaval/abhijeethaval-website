import { apiClient } from '../../shared/api/apiClient';
import type {
  ArticleDraft,
  ArticleDraftSummary,
  CreateArticleDraftPayload,
  PublishedArticleMutation,
  PublishArticleDraftPayload,
  UpdateArticleDraftPayload,
} from './types';

const DRAFTS_PATH = '/api/admin/articles/drafts';

export const adminArticlesApi = {
  getDrafts: async (): Promise<ReadonlyArray<ArticleDraftSummary>> => {
    return apiClient.get<ReadonlyArray<ArticleDraftSummary>>(DRAFTS_PATH);
  },
  getDraft: async (id: string): Promise<ArticleDraft> => {
    return apiClient.get<ArticleDraft>(`${DRAFTS_PATH}/${encodeURIComponent(id)}`);
  },
  createDraft: async (payload: CreateArticleDraftPayload): Promise<ArticleDraft> => {
    return apiClient.postJson<CreateArticleDraftPayload, ArticleDraft>(DRAFTS_PATH, payload);
  },
  updateDraft: async (id: string, payload: UpdateArticleDraftPayload): Promise<ArticleDraft> => {
    const path: string = `${DRAFTS_PATH}/${encodeURIComponent(id)}`;
    return apiClient.putJson<UpdateArticleDraftPayload, ArticleDraft>(path, payload);
  },
  publishDraft: async (
    id: string,
    payload: PublishArticleDraftPayload,
  ): Promise<PublishedArticleMutation> => {
    const path: string = `${DRAFTS_PATH}/${encodeURIComponent(id)}/publish`;
    return apiClient.postJson<PublishArticleDraftPayload, PublishedArticleMutation>(path, payload);
  },
};
