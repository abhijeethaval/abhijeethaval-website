namespace AbhijeetSite.Api.SharedKernel.Result;

/// <summary>
/// Classifies application errors by the boundary that should handle them.
/// </summary>
public enum ErrorCategory
{
    /// <summary>
    /// The caller supplied invalid input.
    /// </summary>
    Validation,

    /// <summary>
    /// The requested operation violates a business rule.
    /// </summary>
    Business,

    /// <summary>
    /// An infrastructure dependency failed.
    /// </summary>
    Infrastructure
}
