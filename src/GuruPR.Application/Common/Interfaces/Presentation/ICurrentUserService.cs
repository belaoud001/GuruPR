namespace GuruPR.Application.Common.Interfaces.Presentation;

/// <summary>
/// Provides access to properties of the current authenticated user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Unique user identifier (usually JWT sub claim). Null if unauthenticated.
    /// </summary>
    string? UserId { get; }
}
