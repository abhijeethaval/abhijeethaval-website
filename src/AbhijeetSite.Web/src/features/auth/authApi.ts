import { apiClient } from '../../shared/api/apiClient';
import type { CurrentUserResponse } from './types';

const CURRENT_USER_PATH = '/api/auth/me';
const GOOGLE_LOGIN_PATH = '/api/auth/login/google';
const LOGOUT_PATH = '/api/auth/logout';

export const authApi = {
  getCurrentUser: async (): Promise<CurrentUserResponse> => {
    return apiClient.get<CurrentUserResponse>(CURRENT_USER_PATH);
  },
  getGoogleLoginUrl: (): string => {
    const returnUrl: string = `${window.location.pathname}${window.location.search}`;
    return `${GOOGLE_LOGIN_PATH}?returnUrl=${encodeURIComponent(returnUrl)}`;
  },
  logout: async (): Promise<void> => {
    await apiClient.post(LOGOUT_PATH);
  },
};
