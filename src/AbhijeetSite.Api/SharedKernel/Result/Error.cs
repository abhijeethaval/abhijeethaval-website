namespace AbhijeetSite.Api.SharedKernel.Result;

/// <summary>
/// Describes an actionable operation failure.
/// </summary>
/// <param name="Code">Stable error code for clients and tests.</param>
/// <param name="Message">Human-readable failure description.</param>
/// <param name="Category">Failure category used at application boundaries.</param>
public sealed record Error(string Code, string Message, ErrorCategory Category);
