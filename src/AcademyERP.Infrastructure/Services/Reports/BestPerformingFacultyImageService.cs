using AcademyERP.Application.DTOs.Reports;
using SkiaSharp;

namespace AcademyERP.Infrastructure.Services.Reports;

public class BestPerformingFacultyImageService
{
    private const int Width = 1600;
    private const int Margin = 60;
    private const int ContentWidth = Width - Margin * 2;

    private static readonly SKColor Blue =
        SKColor.Parse("#17477D");

    private static readonly SKColor LightBlue =
        SKColor.Parse("#EAF3FB");

    private static readonly SKColor Border =
        SKColor.Parse("#D7E3EF");

    private static readonly SKColor Dark =
        SKColor.Parse("#18324A");

    private static readonly SKColor Muted =
        SKColor.Parse("#64748B");

    private static readonly SKColor Green =
        SKColor.Parse("#16A34A");

    private static readonly SKColor GreenLight =
        SKColor.Parse("#E1F5E9");

    private static readonly SKColor Purple =
        SKColor.Parse("#7C3AED");

    private static readonly SKColor Orange =
        SKColor.Parse("#F59E0B");

    private static readonly SKColor Red =
        SKColor.Parse("#D72839");

    public byte[] Generate(
        BestPerformingFacultyReportResponse report,
        string? courseName,
        string? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        const int headerHeight = 190;
        const int cardHeight = 112;
        const int tableHeaderHeight = 58;
        const int rowHeight = 66;

        var bestHeight =
            report.BestFaculty is null
                ? 0
                : 145;

        var height =
            headerHeight +
            30 +
            cardHeight +
            25 +
            bestHeight +
            30 +
            tableHeaderHeight +
            Math.Max(1, report.Faculty.Count) * rowHeight +
            140;

        using var bitmap =
            new SKBitmap(Width, height);

        using var canvas =
            new SKCanvas(bitmap);

        canvas.Clear(
            SKColor.Parse("#F4F8FC"));

        using var bold =
            ReportFontHelper.Bold();

        using var normal =
            ReportFontHelper.Regular();

        using var titleFont =
            new SKFont(bold, 34);

        using var academyFont =
            new SKFont(bold, 25);

        using var subtitleFont =
            new SKFont(normal, 18);

        using var smallFont =
            new SKFont(normal, 14);

        using var headerFont =
            new SKFont(bold, 15);

        using var bodyFont =
            new SKFont(normal, 15);

        using var bodyBoldFont =
            new SKFont(bold, 16);

        using var sectionFont =
            new SKFont(bold, 19);

        using var white =
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White
            };

        using var darkPaint =
            new SKPaint
            {
                IsAntialias = true,
                Color = Dark
            };

        var y = 30f;

        // HEADER

        using var headerPaint =
            new SKPaint
            {
                IsAntialias = true,
                Color = Blue
            };

        canvas.DrawRoundRect(
            new SKRect(
                Margin,
                y,
                Width - Margin,
                y + headerHeight),
            14,
            14,
            headerPaint);

        using var accent =
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColor.Parse("#5DB8F5")
            };

        canvas.DrawRoundRect(
            new SKRect(
                Margin + 18,
                y + 20,
                Margin + 23,
                y + headerHeight - 20),
            3,
            3,
            accent);

        var logoCard =
            new SKRect(
                Margin + 40,
                y + 18,
                Margin + 205,
                y + headerHeight - 18);

        using var logoBackground =
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White
            };

        canvas.DrawRoundRect(
            logoCard,
            12,
            12,
            logoBackground);

        DrawLogo(canvas, logoCard);

        var textX =
            Margin + 235;

        canvas.DrawText(
            "MS INTERNATIONAL ACADEMY",
            textX,
            y + 58,
            academyFont,
            white);

        canvas.DrawText(
            "MUEENU SSALIK",
            textX,
            y + 87,
            subtitleFont,
            white);

        canvas.DrawText(
            "Best Performing Faculty",
            textX,
            y + 130,
            titleFont,
            white);

        canvas.DrawText(
            "Faculty performance  •  Attendance  •  Student teaching",
            textX,
            y + 158,
            subtitleFont,
            white);

        using var filterLabelFont =
            new SKFont(bold, 12);

        canvas.DrawText(
            "REPORT FILTERS",
            Width - Margin - 330,
            y + 45,
            filterLabelFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColor.Parse("#A9D8F7")
            });

        DrawWrapped(
            canvas,
            BuildFilterText(
                courseName,
                gender,
                fromDate,
                toDate),
            Width - Margin - 330,
            y + 72,
            290,
            smallFont,
            white,
            20);

        y += headerHeight + 30;

        // SUMMARY CARDS

        var gap = 15f;

        var cardWidth =
            (ContentWidth - gap * 3) /
            4f;

        DrawCard(
            canvas,
            Margin,
            y,
            cardWidth,
            "TOTAL FACULTY",
            report.TotalFaculty.ToString(),
            "Faculty matching filters",
            Blue);

        DrawCard(
            canvas,
            Margin + cardWidth + gap,
            y,
            cardWidth,
            "WITH REPORTS",
            report.FacultyWithReports.ToString(),
            "Faculty with reports",
            Green);

        DrawCard(
            canvas,
            Margin + (cardWidth + gap) * 2,
            y,
            cardWidth,
            "AVG PERFORMANCE",
            $"{report.AveragePerformance:0.##}%",
            "Average performance",
            Purple);

        DrawCard(
            canvas,
            Margin + (cardWidth + gap) * 3,
            y,
            cardWidth,
            "AVG ATTENDANCE",
            $"{report.AverageAttendance:0.##}%",
            "Overall attendance",
            Orange);

        y += cardHeight + 25;

        // BEST FACULTY

        if (report.BestFaculty is not null)
        {
            using var bestBackground =
                new SKPaint
                {
                    IsAntialias = true,
                    Color = LightBlue
                };

            canvas.DrawRoundRect(
                new SKRect(
                    Margin,
                    y,
                    Width - Margin,
                    y + 145),
                12,
                12,
                bestBackground);

            using var bestBorder =
                new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColor.Parse("#B9D8F0"),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1
                };

            canvas.DrawRoundRect(
                new SKRect(
                    Margin,
                    y,
                    Width - Margin,
                    y + 145),
                12,
                12,
                bestBorder);

            using var gold =
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Orange
                };

            canvas.DrawRoundRect(
                new SKRect(
                    Margin + 15,
                    y + 15,
                    Margin + 20,
                    y + 125),
                3,
                3,
                gold);

            using var bestTitle =
                new SKFont(bold, 13);

            using var bestName =
                new SKFont(bold, 22);

            using var info =
                new SKFont(normal, 15);

            canvas.DrawText(
                "★  BEST PERFORMING FACULTY",
                Margin + 35,
                y + 31,
                bestTitle,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Blue
                });

            canvas.DrawText(
                report.BestFaculty.TeacherName,
                Margin + 35,
                y + 65,
                bestName,
                darkPaint);

            canvas.DrawText(
                $"{report.BestFaculty.CourseName}  •  " +
                $"{report.BestFaculty.Gender}",
                Margin + 35,
                y + 93,
                info,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Muted
                });

            canvas.DrawText(
                $"Students: {report.BestFaculty.StudentsTaught}",
                Margin + 35,
                y + 119,
                info,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Muted
                });

            canvas.DrawText(
                $"{report.BestFaculty.PerformanceScore:0.##}% Performance",
                Width - Margin - 390,
                y + 58,
                bestName,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Green
                });

            canvas.DrawText(
                $"{report.BestFaculty.AttendancePercentage:0.##}% Attendance",
                Width - Margin - 390,
                y + 87,
                info,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Muted
                });

            y += 175;
        }

        canvas.DrawText(
            "TOP PERFORMING FACULTY",
            Margin,
            y + 22,
            sectionFont,
            darkPaint);

        y += 42;

        // TABLE
        //
        // # | FACULTY | GENDER | COURSE |
        // STUDENTS | PERFORMANCE | ATTENDANCE | CLASSES

        var widths = new[]
        {
            55f,
            270f,
            110f,
            330f,
            150f,
            220f,
            220f,
            135f
        };

        var scale =
            ContentWidth /
            widths.Sum();

        for (var i = 0; i < widths.Length; i++)
            widths[i] *= scale;

        var headers = new[]
        {
            "#",
            "FACULTY",
            "GENDER",
            "COURSE",
            "STUDENTS",
            "PERFORMANCE",
            "ATTENDANCE",
            "CLASSES"
        };

        using var tableHeader =
            new SKPaint
            {
                IsAntialias = true,
                Color = Blue
            };

        canvas.DrawRoundRect(
            new SKRect(
                Margin,
                y,
                Width - Margin,
                y + tableHeaderHeight),
            8,
            8,
            tableHeader);

        var x = (float)Margin;

        for (var i = 0; i < headers.Length; i++)
        {
            canvas.DrawText(
                headers[i],
                x + 10,
                y + 36,
                headerFont,
                white);

            x += widths[i];
        }

        y += tableHeaderHeight;

        for (var index = 0;
             index < report.Faculty.Count;
             index++)
        {
            var faculty =
                report.Faculty[index];

            using var rowPaint =
                new SKPaint
                {
                    IsAntialias = true,
                    Color =
                        index % 2 == 0
                            ? SKColors.White
                            : SKColor.Parse("#F7FAFD")
                };

            canvas.DrawRect(
                new SKRect(
                    Margin,
                    y,
                    Width - Margin,
                    y + rowHeight),
                rowPaint);

            x = Margin;

            DrawCell(
                canvas,
                faculty.Rank.ToString(),
                x,
                y,
                widths[0],
                rowHeight,
                bodyFont,
                Dark);

            x += widths[0];

            DrawCell(
                canvas,
                faculty.TeacherName,
                x,
                y,
                widths[1],
                rowHeight,
                bodyBoldFont,
                Dark);

            x += widths[1];

            DrawCell(
                canvas,
                faculty.Gender,
                x,
                y,
                widths[2],
                rowHeight,
                bodyFont,
                Muted);

            x += widths[2];

            DrawCell(
                canvas,
                faculty.CourseName,
                x,
                y,
                widths[3],
                rowHeight,
                bodyFont,
                Dark);

            x += widths[3];

            DrawCell(
                canvas,
                faculty.StudentsTaught.ToString(),
                x,
                y,
                widths[4],
                rowHeight,
                bodyFont,
                Dark);

            x += widths[4];

            DrawMetric(
                canvas,
                faculty.PerformanceScore,
                x,
                y,
                widths[5],
                rowHeight,
                true);

            x += widths[5];

            DrawMetric(
                canvas,
                faculty.AttendancePercentage,
                x,
                y,
                widths[6],
                rowHeight,
                false);

            x += widths[6];

            DrawCell(
                canvas,
                faculty.ClassesReported.ToString(),
                x,
                y,
                widths[7],
                rowHeight,
                bodyFont,
                Dark);

            y += rowHeight;
        }

        // FOOTER

        y += 35;

        using var verseFont =
            new SKFont(normal, 16);

        using var sourceFont =
            new SKFont(normal, 12);

        var footerPaint =
            new SKPaint
            {
                IsAntialias = true,
                Color = Muted
            };

        CenterText(
            canvas,
            "\"And say, My Lord, increase me in knowledge\"",
            Width / 2f,
            y,
            verseFont,
            footerPaint);

        CenterText(
            canvas,
            "— Quran 20:114",
            Width / 2f,
            y + 24,
            sourceFont,
            footerPaint);

        using var image =
            SKImage.FromBitmap(bitmap);

        using var data =
            image.Encode(
                SKEncodedImageFormat.Png,
                100);

        return data.ToArray();
    }

    private static void DrawLogo(
        SKCanvas canvas,
        SKRect card)
    {
        var path =
            Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "Clogo.png");

        if (!File.Exists(path))
            return;

        using var logo =
            SKBitmap.Decode(path);

        if (logo is null)
            return;

        canvas.DrawBitmap(
            logo,
            new SKRect(
                card.Left + 12,
                card.Top + 12,
                card.Right - 12,
                card.Bottom - 12));
    }

    private static void DrawCard(
        SKCanvas canvas,
        float x,
        float y,
        float width,
        string title,
        string value,
        string description,
        SKColor accent)
    {
        using var background =
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White
            };

        canvas.DrawRoundRect(
            new SKRect(
                x,
                y,
                x + width,
                y + 112),
            10,
            10,
            background);

        using var border =
            new SKPaint
            {
                IsAntialias = true,
                Color = Border,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1
            };

        canvas.DrawRoundRect(
            new SKRect(
                x,
                y,
                x + width,
                y + 112),
            10,
            10,
            border);

        using var accentPaint =
            new SKPaint
            {
                IsAntialias = true,
                Color = accent
            };

        canvas.DrawRoundRect(
            new SKRect(
                x + 12,
                y + 18,
                x + 16,
                y + 94),
            2,
            2,
            accentPaint);

        using var bold =
            ReportFontHelper.Bold();

        using var normal =
            ReportFontHelper.Regular();

        using var titleFont =
            new SKFont(bold, 13);

        using var valueFont =
            new SKFont(bold, 28);

        using var descFont =
            new SKFont(normal, 11);

        canvas.DrawText(
            title,
            x + 28,
            y + 30,
            titleFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = Muted
            });

        canvas.DrawText(
            value,
            x + 28,
            y + 67,
            valueFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = accent
            });

        canvas.DrawText(
            description,
            x + 28,
            y + 94,
            descFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = Muted
            });
    }

    private static void DrawCell(
        SKCanvas canvas,
        string text,
        float x,
        float y,
        float width,
        float height,
        SKFont font,
        SKColor color)
    {
        text =
            FitText(
                text,
                font,
                width - 20);

        canvas.DrawText(
            text,
            x + 10,
            y + height / 2 + 6,
            font,
            new SKPaint
            {
                IsAntialias = true,
                Color = color
            });
    }

    private static void DrawMetric(
        SKCanvas canvas,
        double value,
        float x,
        float y,
        float width,
        float height,
        bool performance)
    {
        SKColor color;
        SKColor light;

        if (value >= 90)
        {
            color = Green;
            light = GreenLight;
        }
        else if (value >= 75)
        {
            color = Blue;
            light = SKColor.Parse("#E6F1FB");
        }
        else if (value >= 60)
        {
            color = SKColor.Parse("#00897B");
            light = SKColor.Parse("#E0F4F1");
        }
        else if (value >= 40)
        {
            color = Orange;
            light = SKColor.Parse("#FFF3D6");
        }
        else
        {
            color = Red;
            light = SKColor.Parse("#FDE7EA");
        }

        using var paint =
            new SKPaint
            {
                IsAntialias = true,
                Color = light
            };

        var rect =
            new SKRect(
                x + 10,
                y + 11,
                x + width - 10,
                y + height - 11);

        canvas.DrawRoundRect(
            rect,
            8,
            8,
            paint);

        using var typeface =
            ReportFontHelper.Bold();

        using var font =
            new SKFont(typeface, 16);

        var text =
            $"{value:0.##}%";

        var textWidth =
            font.MeasureText(text);

        canvas.DrawText(
            text,
            rect.MidX - textWidth / 2,
            rect.MidY + 6,
            font,
            new SKPaint
            {
                IsAntialias = true,
                Color = color
            });
    }

    private static string FitText(
        string text,
        SKFont font,
        float maxWidth)
    {
        if (font.MeasureText(text) <= maxWidth)
            return text;

        while (
            text.Length > 3 &&
            font.MeasureText(text + "...") > maxWidth)
        {
            text = text[..^1];
        }

        return text + "...";
    }

    private static void CenterText(
        SKCanvas canvas,
        string text,
        float center,
        float y,
        SKFont font,
        SKPaint paint)
    {
        var width =
            font.MeasureText(text);

        canvas.DrawText(
            text,
            center - width / 2,
            y,
            font,
            paint);
    }

    private static void DrawWrapped(
        SKCanvas canvas,
        string text,
        float x,
        float y,
        float maxWidth,
        SKFont font,
        SKPaint paint,
        float lineHeight)
    {
        var line = string.Empty;

        foreach (var word in
                 text.Split(
                     ' ',
                     StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate =
                string.IsNullOrEmpty(line)
                    ? word
                    : line + " " + word;

            if (font.MeasureText(candidate) <= maxWidth)
            {
                line = candidate;
            }
            else
            {
                canvas.DrawText(
                    line,
                    x,
                    y,
                    font,
                    paint);

                y += lineHeight;
                line = word;
            }
        }

        if (!string.IsNullOrWhiteSpace(line))
        {
            canvas.DrawText(
                line,
                x,
                y,
                font,
                paint);
        }
    }

    private static string BuildFilterText(
        string? course,
        string? gender,
        DateOnly? from,
        DateOnly? to)
    {
        var list =
            new List<string>();

        if (!string.IsNullOrWhiteSpace(course))
            list.Add($"Course: {course}");

        if (!string.IsNullOrWhiteSpace(gender))
            list.Add($"Gender: {gender}");

        if (from.HasValue)
            list.Add($"From: {from:dd MMM yyyy}");

        if (to.HasValue)
            list.Add($"To: {to:dd MMM yyyy}");

        return list.Count == 0
            ? "All Faculty"
            : string.Join(
                "  |  ",
                list);
    }
}
