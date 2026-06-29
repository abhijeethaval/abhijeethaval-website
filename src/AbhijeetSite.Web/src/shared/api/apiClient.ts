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
      credentials: 'same-origin',
      headers: {
        'Accept': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    return response.json() as Promise<T>;
  },
  post: async (path: string): Promise<void> => {
    const baseUrl = getApiBaseUrl();
    const cleanPath = path.startsWith('/') ? path : `/${path}`;
    const url = `${baseUrl}${cleanPath}`;

    const response = await fetch(url, {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        'Accept': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }
  },
};
