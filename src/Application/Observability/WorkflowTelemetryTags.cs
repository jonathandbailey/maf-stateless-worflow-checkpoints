using System.Diagnostics;

namespace Application.Observability;

public static class WorkflowTelemetryTags
{
    public const string Prefix = "workflow";

    public const string Node = Prefix + ".node";
    public const string ArtifactKey = Prefix + ".artifact_key";

    public const string InputPreview = Prefix + ".input.preview";
    public const string InputLength = Prefix + ".input.length";
    public const string InputTruncated = Prefix + ".input.truncated";

    public const string OutputPreview = Prefix + ".output.preview";
    public const string OutputLength = Prefix + ".output.length";
    public const string OutputTruncated = Prefix + ".output.truncated";

    private const int DefaultPreviewLength = 200;

    public static void SetPreview(Activity? activity, string? value, int maxPreviewLength = DefaultPreviewLength)
    {
        if (activity == null) return;

        if (value == null)
        {
            activity.SetTag(InputPreview, string.Empty);
            activity.SetTag(InputLength, 0);
            activity.SetTag(InputTruncated, false);
            return;
        }

        activity.SetTag(InputLength, value.Length);

        var truncated = value.Length > maxPreviewLength;

        var preview = truncated ? value.Substring(0, maxPreviewLength) : value;

        activity.SetTag(InputPreview, preview);
        activity.SetTag(InputTruncated, truncated);
    }
}