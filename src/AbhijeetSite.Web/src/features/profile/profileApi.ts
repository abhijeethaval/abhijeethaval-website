import { apiClient } from '../../shared/api/apiClient';
import { Profile } from './types';

export const profileApi = {
  getProfile: (): Promise<Profile> => {
    return apiClient.get<Profile>('/api/profile');
  },
};
