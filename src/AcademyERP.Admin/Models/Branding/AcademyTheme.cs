using MudBlazor;

namespace AcademyERP.Admin.Models.Branding;

public static class AcademyTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0756A3",
            PrimaryDarken = "#03427D",
            PrimaryLighten = "#2A78C4",

            Secondary = "#0B72C9",

            AppbarBackground = "#0756A3",
            AppbarText = "#FFFFFF",

            DrawerBackground = "#FFFFFF",
            DrawerText = "#1F2937",
            DrawerIcon = "#0756A3",

            Background = "#F7F9FC",
            Surface = "#FFFFFF",

            TextPrimary = "#1F2937",
            TextSecondary = "#64748B",

            ActionDefault = "#0756A3",
            Divider = "#E2E8F0",
            LinesDefault = "#E2E8F0",

            Error = "#D32F2F",
            Success = "#2E7D32",
            Warning = "#ED6C02",
            Info = "#0288D1"
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px"
        }
    };
}
