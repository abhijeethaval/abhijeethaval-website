# 0003 - User-Journey Observability Boundary

## Status

Accepted

## Context

The public request path crosses the React application, the Nginx web container, the
internal ASP.NET Core API, and API dependencies such as PostgreSQL and Google OAuth.
Server-only telemetry cannot explain the user action that initiated a request or failures
that occur before the request reaches the API.

## Decision

Use user-journey tracing as the application observability boundary. For instrumented user
interactions, correlation begins in React and continues through Nginx to ASP.NET Core and
its database and HTTP dependencies using W3C Trace Context. Nginx initially remains a
transparent propagation hop rather than creating a dedicated span. Work without a browser
initiator, such as startup and background processing, begins a trace at the API boundary.

Azure Monitor Application Insights is the production telemetry backend. React uses the
Application Insights JavaScript SDK for browser real-user monitoring and creates W3C
trace context for API requests. The ASP.NET Core API uses OpenTelemetry APIs and exports
logs and traces over OTLP to the Azure Container Apps managed OpenTelemetry agent, which
forwards them to Application Insights.

The API must not reference Application Insights telemetry types or export directly with
the Azure Monitor SDK. This keeps server instrumentation and transport vendor-neutral.
A browser-only OpenTelemetry pipeline is rejected for now because it would require a
public, browser-safe telemetry gateway without improving trace-context interoperability.

ASP.NET Core structured logging uses the built-in `ILogger<T>` abstraction and the
OpenTelemetry logging provider. Stable application events use source-generated
`LoggerMessage` methods with explicit event IDs, event names, message templates, and typed
properties. Serilog is not introduced because a second logging pipeline would add
configuration and enrichment overlap without a current sink or formatting requirement
that the native stack cannot satisfy.

Telemetry uses an explicit allowlist. It may contain event identity, module and operation
names, route templates, status, duration, typed error details, trace context, deployment
metadata, and domain IDs when those IDs are required for diagnosis. High-cardinality
domain IDs must not be metric dimensions.

Telemetry must not contain names, email addresses, OAuth claims or tokens, cookies,
authorization headers, connection strings, request or response bodies, article or MDX
content, query strings, or unsanitized URLs. Authentication events may use the internal
`UserId`, but never the user's email address.

Dynamic article slugs and draft identifiers are normalized to route templates before
browser export. Edge access logs omit request paths because Nginx cannot reliably map them
to ASP.NET route templates. Application failure telemetry records the exception type, not
raw exception messages or stack traces that may embed SQL, connection details, or content.

The production export scope is logs and traces. Existing OpenTelemetry metrics remain
available in the local Aspire dashboard, while Azure Container Apps platform metrics cover
the production baseline. Custom application metrics require a separate decision because
the managed agent's Application Insights destination does not currently accept metrics.

User and business traces use parent-based 100 percent sampling across React and ASP.NET
Core. Successful health probes, static asset requests, and telemetry transport traffic are
suppressed as operational noise. Failed requests and exceptions remain observable,
including failures on otherwise suppressed routes. Sampling may be reduced only in
response to measured production ingestion volume, not preemptively.

PostgreSQL telemetry uses Npgsql OpenTelemetry instrumentation but does not export raw SQL
or parameter values in production. Database spans retain operation, database and schema
metadata, duration, status, and exception details. Production logging raises EF Core and
Npgsql command categories to `Warning`; development may enable SQL command text at
`Debug`, but parameter-value logging remains disabled in every environment.

Automatic instrumentation is supplemented by one manual activity for each named
application use case. Activity names describe stable domain operations such as
`articles.publish_draft` rather than implementation methods. Activities may contain
allowlisted domain IDs, the result category, and typed error code. Entities, value
objects, and helper methods are not instrumented. State transitions are emitted as
structured events so a trace explains business intent without mirroring the call stack.

Each deployed environment uses one Application Insights resource for end-to-end queries.
Browser telemetry uses the role name `abhijeetsite-web`; API telemetry uses the OpenTelemetry
resource attribute `service.name=abhijeetsite-api`. Azure Container Apps environment and
revision metadata distinguish deployments. Local development and tests do not export to
the production resource and continue to use the Aspire dashboard. A future Azure staging
environment must use a separate Application Insights resource.

Browser telemetry is limited to page views, SPA route changes, fetch and XHR dependencies,
failed dependencies, unhandled JavaScript errors, and explicit named events for important
UI states that do not produce an API request. Generic click analytics, DOM or element-text
collection, session replay, and form-field capture are disabled. API-backed actions do not
emit duplicate click events because the dependency and server spans already represent
those operations.

Each failure has one telemetry owner at the boundary where its category becomes known.
Validation failures add a typed result code to the use-case activity without an error log.
Expected business conflicts may emit one `Information` or `Warning` event and do not emit
exception telemetry. An infrastructure boundary that converts an exception to `Result`
emits one `Error` event, records the exception on the activity, and marks the activity as
failed. A global exception boundary owns unhandled exceptions. Fatal startup failures emit
one `Critical` event before startup terminates. Intermediate and HTTP transport layers do
not log the same failure again.

Google OAuth is an explicit trace and trust boundary. Login initiation is traced until the
redirect to Google; the callback starts a new trace covering callback validation, token
exchange, local-user upsert, and the redirect response. Trace context is not carried in
OAuth state, cookies, or provider parameters. Operators correlate the two traces only by
time, deployment, anonymous browser context, and privacy-safe outcome events.

Browser monitoring is anonymous and cookieless. The Application Insights JavaScript SDK
has cookie usage disabled and does not receive authenticated-user context, email, or the
internal `UserId`. Session storage may buffer unsent telemetry but must not persist a user
identifier. Request-level correlation relies on W3C trace context rather than returning-
user identity.

Distributed propagation uses W3C `traceparent` and `tracestate` only. OpenTelemetry
`baggage`, legacy Application Insights request identifiers, domain identifiers, user
identity, and authentication context are not propagated. Nginx forwards W3C headers
unchanged, and ASP.NET Core uses a trace-context-only propagator. Diagnostic attributes
are attached to the span or structured event that owns them.

Telemetry transport is configured centrally by `AbhijeetSite.ServiceDefaults`, including
source registration, OTLP export, sampling, and filtering. Business telemetry definitions
remain module-owned: Articles and Identity each own their stable `ActivitySource` name and
source-generated log events, while persistence owns database and startup events. Domain
entities and value objects have no telemetry dependencies. The application does not add a
generic logging wrapper or central business-event catalog.

The production Application Insights resource permits local-auth ingestion because the
browser SDK and the Azure Container Apps managed OpenTelemetry agent require it. The
connection string is deployment configuration and is not committed to source control,
although its browser-facing identifier cannot be treated as secret. A daily ingestion cap,
cost alert, and unexpected-volume monitoring limit abuse and accidental telemetry growth.
An Entra-only ingestion requirement would force a future exporter-topology decision.

Application Insights data uses 90-day retention without long-term archive. The initial
daily ingestion cap is 0.1 GB, with alerts at 80 percent and when collection stops. After
14 production days, the cap is recalibrated to the greater of 0.1 GB or five times normal
P95 daily ingestion. The cap is an emergency cost safeguard, not routine sampling or
filtering.

Telemetry delivery fails open. OTLP export is asynchronous and bounded; application
readiness and liveness never depend on the managed collector or Application Insights.
Exporter failures remain visible in container console output, but the application keeps
serving and does not accumulate an unbounded retry queue. Health endpoints validate
application dependencies only, not the observability backend.

Telemetry behavior is verified without contacting Azure. API tests use in-memory
collection to assert activity names, typed attributes, status, structured event identity,
W3C parentage, and the absence of prohibited fields. PostgreSQL integration tests verify
dependency spans without SQL or parameter values. Frontend tests verify cookieless,
anonymous configuration and the approved collection scope. Local smoke testing uses the
Aspire dashboard; a controlled production journey must correlate browser, API, use-case,
and PostgreSQL telemetry in Application Insights. Tests never export telemetry to Azure.

Production alerting starts with an Application Insights Standard availability test against
public `GET /api/articles`, every five minutes from five locations, with a five-second
timeout and an alert threshold of three failed locations. Separate alerts fire for three
server `5xx` responses or three PostgreSQL dependency failures within five minutes. One
owner action group receives fired and resolved notifications. Latency alerts wait for the
initial 14-day baseline. Deprecated URL ping tests are not used.
