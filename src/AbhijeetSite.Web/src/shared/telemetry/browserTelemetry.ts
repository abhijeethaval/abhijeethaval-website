import type {
  ApplicationInsights,
  DistributedTracingModes,
  IConfig,
  IConfiguration,
  ITelemetryItem,
  Snippet,
} from '@microsoft/applicationinsights-web';

const CLOUD_ROLE_TAG = 'ai.cloud.role';
const MAX_DEPENDENCIES_PER_VIEW = 50;
const WEB_ROLE_NAME = 'abhijeetsite-web';
const W3C_TRACE_MODE: DistributedTracingModes = 18;
const EXCLUDED_DEPENDENCIES: ReadonlyArray<RegExp> = [
  /\/(?:health|alive)(?:[/?#]|$)/i,
  /\/telemetry-config\.js(?:[?#]|$)/i,
];
const DYNAMIC_PATHS: ReadonlyArray<readonly [RegExp, string]> = [
  [/(\/api\/admin\/articles\/drafts\/)[^/?#]+/gi, '$1{id}'],
  [/(\/api\/articles\/)[^/?#]+/gi, '$1{slug}'],
  [/(^|https?:\/\/[^/]+)(\/articles\/)[^/?#]+/gi, '$1$2{slug}'],
];

export type BrowserTelemetryConfig = IConfiguration & IConfig;

export const createBrowserTelemetryConfig = (
  connectionString: string,
  browserHost: string,
): BrowserTelemetryConfig => ({
  connectionString,
  correlationHeaderDomains: [browserHost],
  disableAjaxTracking: false,
  disableCookiesUsage: true,
  disableExceptionTracking: false,
  disableFetchTracking: false,
  distributedTracingMode: W3C_TRACE_MODE,
  enableAjaxErrorStatusText: false,
  enableAjaxPerfTracking: true,
  enableAutoRouteTracking: true,
  enableCorsCorrelation: false,
  enableRequestHeaderTracking: false,
  enableResponseHeaderTracking: false,
  enableSessionStorageBuffer: true,
  enableUnhandledPromiseRejectionTracking: true,
  excludeRequestFromAutoTrackingPatterns: [...EXCLUDED_DEPENDENCIES],
  maxAjaxCallsPerView: MAX_DEPENDENCIES_PER_VIEW,
});

export const initializeBrowserTelemetry = async (): Promise<void> => {
  const connectionString: string | undefined = getConnectionString();
  if (connectionString === undefined) {
    return Promise.resolve();
  }

  const { ApplicationInsights } = await import('@microsoft/applicationinsights-web');
  const snippet: Snippet = {
    config: createBrowserTelemetryConfig(connectionString, window.location.host),
  };
  const telemetry: ApplicationInsights = new ApplicationInsights(snippet);
  telemetry.addTelemetryInitializer(enrichTelemetry);
  telemetry.loadAppInsights();
  telemetry.trackPageView({ uri: sanitizeUrl(window.location.href) });
};

const getConnectionString = (): string | undefined => {
  const configured: string | undefined = window.__ABHIJEET_SITE_TELEMETRY__?.connectionString;
  const trimmed: string = configured?.trim() ?? '';
  return trimmed.length === 0 ? undefined : trimmed;
};

const enrichTelemetry = (item: ITelemetryItem): void => {
  item.tags ??= {};
  item.tags[CLOUD_ROLE_TAG] = WEB_ROLE_NAME;
  sanitizeBaseData(item);
};

const sanitizeBaseData = (item: ITelemetryItem): void => {
  if (item.baseData === undefined) {
    return;
  }

  const uri: unknown = item.baseData.uri;
  if (typeof uri === 'string') {
    item.baseData.uri = sanitizeUrl(uri);
  }

  const dependencyUrl: unknown = item.baseData.data;
  if (typeof dependencyUrl === 'string') {
    item.baseData.data = sanitizeUrl(dependencyUrl);
  }

  const telemetryName: unknown = item.baseData.name;
  if (typeof telemetryName === 'string') {
    item.baseData.name = sanitizeUrl(telemetryName);
  }
};

export const sanitizeUrl = (url: string): string => {
  const queryIndex: number = url.indexOf('?');
  const fragmentIndex: number = url.indexOf('#');
  const candidates: ReadonlyArray<number> = [queryIndex, fragmentIndex].filter(index => index >= 0);
  const boundary: number = candidates.length === 0 ? url.length : Math.min(...candidates);
  return DYNAMIC_PATHS.reduce(
    (sanitizedUrl, [pattern, replacement]) => sanitizedUrl.replace(pattern, replacement),
    url.slice(0, boundary),
  );
};
