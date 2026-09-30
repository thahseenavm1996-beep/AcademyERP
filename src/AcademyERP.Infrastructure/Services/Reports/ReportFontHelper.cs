using SkiaSharp;

namespace AcademyERP.Infrastructure.Services.Reports;

internal static class ReportFontHelper
{
    private static string FontPath(string fileName)
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Fonts",
            fileName);
    }

    public static SKTypeface Regular()
    {
        return SKTypeface.FromFile(
            FontPath("Lato-Regular.ttf"));
    }

    public static SKTypeface Bold()
    {
        return SKTypeface.FromFile(
            FontPath("Lato-Bold.ttf"));
    }
}
