const getApiBaseUrl = (): string => {
  // Rely on relative URLs. Vite dev server proxy is used in development, and Nginx in production.
  return '';
};

const JSON_HEADERS: Readonly<Record<string, string>> = {
  'Accept': 'application/json',
  'Content-Type': 'application/json',
};

const ACCEPT_JSON_HEADERS: Readonly<Record<string, string>> = {
  'Accept': 'application/json',
};

export const apiClient = {
  get: async <T>(path: string): Promise<T> => {
    return requestJson<T>(path, 'GET');
  },
  post: async (path: string): Promise<void> => {
    await requestNoContent(path, 'POST');
  },
  postJson: async <TRequest, TResponse>(path: string, body: TRequest): Promise<TResponse> => {
    return requestJson<TResponse>(path, 'POST', body);
  },
  putJson: async <TRequest, TResponse>(path: string, body: TRequest): Promise<TResponse> => {
    return requestJson<TResponse>(path, 'PUT', body);
  },
};

const requestJson = async <T>(path: string, method: string, body?: unknown): Promise<T> => {
  const response: Response = await fetch(buildUrl(path), {
    method,
    credentials: 'same-origin',
    headers: body === undefined ? ACCEPT_JSON_HEADERS : JSON_HEADERS,
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (!response.ok) {
    throw new Error(`API Error: ${response.status} ${response.statusText}`);
  }

  return response.json() as Promise<T>;
};

const requestNoContent = async (path: string, method: string): Promise<void> => {
  const response: Response = await fetch(buildUrl(path), {
    method,
    credentials: 'same-origin',
    headers: ACCEPT_JSON_HEADERS,
  });

  if (!response.ok) {
    throw new Error(`API Error: ${response.status} ${response.statusText}`);
  }
};

const buildUrl = (path: string): string => {
  const baseUrl: string = getApiBaseUrl();
  const cleanPath: string = path.startsWith('/') ? path : `/${path}`;
  return `${baseUrl}${cleanPath}`;
};
