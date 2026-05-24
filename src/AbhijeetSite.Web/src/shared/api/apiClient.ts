const getApiBaseUrl = (): string => {
  // Rely on relative URLs. Vite dev server proxy is used in development, and Nginx in production.
  return '';
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
