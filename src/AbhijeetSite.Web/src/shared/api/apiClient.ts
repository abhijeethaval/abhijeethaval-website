const getApiBaseUrl = (): string => {
  // If VITE_API_URL is provided via environment (e.g. by Aspire), use it.
  if (import.meta.env.VITE_API_URL) {
    return import.meta.env.VITE_API_URL;
  }
  // Fall back to default API local development port
  return 'http://localhost:5297';
};

export const apiClient = {
  get: async <T>(path: string): Promise<T> => {
    const baseUrl = getApiBaseUrl();
    const cleanPath = path.startsWith('/') ? path : `/${path}`;
    const url = `${baseUrl}${cleanPath}`;

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    return response.json() as Promise<T>;
  },
};
