namespace Krafter.Shared.Features.AppInfo;

/// <summary>
/// Contains build information for the application.
/// Values are replaced during build process.
/// </summary>
public static class BuildInfo
{
    public static string DateTimeUtc { get; set; } = "#DateTimeUtc";
    public static string Build { get; set; } = "#Build";
}
