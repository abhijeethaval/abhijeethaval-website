import { apiClient } from '../../shared/api/apiClient';
import { HomeSummary } from './types';

export const homeApi = {
  getSummary: (): Promise<HomeSummary> => {
    return apiClient.get<HomeSummary>('/api/home/summary');
  },
};
