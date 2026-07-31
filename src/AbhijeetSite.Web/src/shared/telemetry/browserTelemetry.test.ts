import { DistributedTracingModes } from '@microsoft/applicationinsights-web';
import { describe, expect, it } from 'vitest';
import { createBrowserTelemetryConfig, sanitizeUrl } from './browserTelemetry';

const BROWSER_HOST = 'abhijeethaval.com';
const CONNECTION_STRING = 'InstrumentationKey=test-key';

describe('createBrowserTelemetryConfig', () => {
  it('creates a cookieless W3C configuration', () => {
    const config = createBrowserTelemetryConfig(CONNECTION_STRING, BROWSER_HOST);

    expect(config.disableCookiesUsage).toBe(true);
    expect(config.distributedTracingMode).toBe(DistributedTracingModes.W3C_TRACE);
    expect(config.correlationHeaderDomains).toEqual([BROWSER_HOST]);
    expect(config.enableSessionStorageBuffer).toBe(true);
  });

  it('excludes health probes without excluding application APIs', () => {
    const config = createBrowserTelemetryConfig(CONNECTION_STRING, BROWSER_HOST);
    const patterns: ReadonlyArray<string | RegExp> = config.excludeRequestFromAutoTrackingPatterns ?? [];

    expect(matchesAny(patterns, 'https://abhijeethaval.com/health')).toBe(true);
    expect(matchesAny(patterns, 'https://abhijeethaval.com/api/articles')).toBe(false);
  });

  it('removes query data and dynamic article identifiers', () => {
    const url = 'https://abhijeethaval.com/api/articles/private-draft?preview=true#content';

    expect(sanitizeUrl(url)).toBe('https://abhijeethaval.com/api/articles/{slug}');
    expect(sanitizeUrl('/api/admin/articles/drafts/22f07091-33a9-4427-8cf7-7d8649e253be/publish'))
      .toBe('/api/admin/articles/drafts/{id}/publish');
  });
});

const matchesAny = (patterns: ReadonlyArray<string | RegExp>, value: string): boolean => {
  return patterns.some(pattern =>
    typeof pattern === 'string' ? value.includes(pattern) : pattern.test(value));
};
