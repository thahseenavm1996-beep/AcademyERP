using AcademyERP.Application.DTOs.Reports;
using SkiaSharp;

namespace AcademyERP.Infrastructure.Services.Reports;

public class BestPerformingStudentsImageService
{
    private const int Width = 1600;
    private const int Margin = 60;
    private const int ContentWidth = Width - (Margin * 2);

    private static readonly SKColor AcademyBlue = SKColor.Parse("#17477D");
    private static readonly SKColor AcademyLightBlue = SKColor.Parse("#EAF3FB");
    private static readonly SKColor BorderColor = SKColor.Parse("#D7E3EF");
    private static readonly SKColor TextDark = SKColor.Parse("#18324A");
    private static readonly SKColor TextMuted = SKColor.Parse("#64748B");

    private static readonly SKColor Green = SKColor.Parse("#16A34A");
    private static readonly SKColor GreenLight = SKColor.Parse("#E1F5E9");
    private static readonly SKColor Blue = SKColor.Parse("#1976D2");
    private static readonly SKColor BlueLight = SKColor.Parse("#E6F1FB");
    private static readonly SKColor Teal = SKColor.Parse("#00897B");
    private static readonly SKColor TealLight = SKColor.Parse("#E0F4F1");
    private static readonly SKColor Orange = SKColor.Parse("#F59E0B");
    private static readonly SKColor OrangeLight = SKColor.Parse("#FFF3D6");
    private static readonly SKColor Red = SKColor.Parse("#D72839");
    private static readonly SKColor RedLight = SKColor.Parse("#FDE7EA");
    private static readonly SKColor Purple = SKColor.Parse("#7C3AED");
    private static readonly SKColor PurpleLight = SKColor.Parse("#F0EAFE");

    public byte[] Generate(
        BestPerformingStudentsReportResponse report,
        string? courseName,
        string? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        const int headerHeight = 190;
        const int summaryHeight = 112;
        const int tableHeaderHeight = 58;
        const int rowHeight = 66;

        var bestHeight = report.BestStudent is null ? 0 : 115;

        var height =
            headerHeight +
            30 +
            summaryHeight +
            25 +
            bestHeight +
            30 +
            tableHeaderHeight +
            Math.Max(1, report.Students.Count) * rowHeight +
            140;

        using var bitmap = new SKBitmap(Width, height);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColor.Parse("#F4F8FC"));

        using var bold = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
        using var normal = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal);

        using var titleFont = new SKFont(bold, 34);
        using var academyFont = new SKFont(bold, 25);
        using var subtitleFont = new SKFont(normal, 18);
        using var smallFont = new SKFont(normal, 14);
        using var headerFont = new SKFont(bold, 15);
        using var bodyFont = new SKFont(normal, 15);
        using var bodyBoldFont = new SKFont(bold, 16);
        using var sectionFont = new SKFont(bold, 19);

        using var whitePaint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.White
        };

        using var textPaint = new SKPaint
        {
            IsAntialias = true,
            Color = TextDark
        };

        var y = 30f;

        // HEADER
        using var headerPaint = new SKPaint
        {
            IsAntialias = true,
            Color = AcademyBlue
        };

        canvas.DrawRoundRect(
            new SKRect(Margin, y, Width - Margin, y + headerHeight),
            14, 14, headerPaint);

        using var accentPaint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColor.Parse("#5DB8F5")
        };

        canvas.DrawRoundRect(
            new SKRect(Margin + 18, y + 20,
                Margin + 23, y + headerHeight - 20),
            3, 3, accentPaint);

        var logoCard = new SKRect(
            Margin + 40,
            y + 18,
            Margin + 205,
            y + headerHeight - 18);

        using var logoCardPaint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.White
        };

        canvas.DrawRoundRect(
            logoCard, 12, 12, logoCardPaint);

        DrawLogo(canvas, logoCard);

        var textX = Margin + 235;

        canvas.DrawText(
            "MS INTERNATIONAL ACADEMY",
            textX, y + 58,
            academyFont, whitePaint);

        canvas.DrawText(
            "MUEENU SSALIK",
            textX, y + 87,
            subtitleFont, whitePaint);

        canvas.DrawText(
            "Best Performing Students",
            textX, y + 130,
            titleFont, whitePaint);

        canvas.DrawText(
            "Student performance  •  Attendance  •  Class progress",
            textX, y + 158,
            subtitleFont, whitePaint);

        using var labelFont = new SKFont(bold, 12);

        canvas.DrawText(
            "REPORT FILTERS",
            Width - Margin - 330,
            y + 45,
            labelFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColor.Parse("#A9D8F7")
            });

        DrawWrappedText(
            canvas,
            BuildFilterText(courseName, gender, fromDate, toDate),
            Width - Margin - 330,
            y + 72,
            290,
            smallFont,
            whitePaint,
            20);

        y += headerHeight + 30;

        // SUMMARY
        var gap = 15f;
        var cardWidth = (ContentWidth - gap * 3) / 4f;

        DrawCard(canvas, Margin, y, cardWidth,
            "TOTAL STUDENTS",
            report.TotalStudents.ToString(),
            "Students matching filters",
            Blue);

        DrawCard(canvas, Margin + cardWidth + gap, y, cardWidth,
            "WITH REPORTS",
            report.StudentsWithReports.ToString(),
            "Students with performance data",
            Green);

        DrawCard(canvas, Margin + (cardWidth + gap) * 2, y, cardWidth,
            "AVG PERFORMANCE",
            $"{report.AveragePerformance:0.##}%",
            "Average performance score",
            Purple);

        DrawCard(canvas, Margin + (cardWidth + gap) * 3, y, cardWidth,
            "AVG ATTENDANCE",
            $"{report.AverageAttendance:0.##}%",
            "Overall attendance",
            Orange);

        y += summaryHeight + 25;

        // BEST STUDENT
        if (report.BestStudent is not null)
        {
            using var bestPaint = new SKPaint
            {
                IsAntialias = true,
                Color = AcademyLightBlue
            };

            canvas.DrawRoundRect(
                new SKRect(Margin, y, Width - Margin, y + 115),
                12, 12, bestPaint);

            using var bestBorder = new SKPaint
            {
                IsAntialias = true,
                Color = SKColor.Parse("#B9D8F0"),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1
            };

            canvas.DrawRoundRect(
                new SKRect(Margin, y, Width - Margin, y + 115),
                12, 12, bestBorder);

            using var gold = new SKPaint
            {
                IsAntialias = true,
                Color = Orange
            };

            canvas.DrawRoundRect(
                new SKRect(Margin + 15, y + 15,
                    Margin + 20, y + 100),
                3, 3, gold);

            using var bestTitleFont = new SKFont(bold, 13);
            using var bestStudentFont = new SKFont(bold, 21);
            using var infoFont = new SKFont(normal, 15);

            canvas.DrawText(
                "★  BEST PERFORMING STUDENT",
                Margin + 35, y + 30,
                bestTitleFont,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = AcademyBlue
                });

            canvas.DrawText(
                report.BestStudent.StudentName,
                Margin + 35, y + 61,
                bestStudentFont,
                textPaint);

            canvas.DrawText(
                $"{report.BestStudent.CourseName}  •  {report.BestStudent.TeacherName}",
                Margin + 35, y + 88,
                infoFont,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = TextMuted
                });

            canvas.DrawText(
                $"{report.BestStudent.PerformanceScore:0.##}% Performance",
                Width - Margin - 390,
                y + 55,
                bestStudentFont,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = Green
                });

            canvas.DrawText(
                $"{report.BestStudent.AttendancePercentage:0.##}% Attendance",
                Width - Margin - 390,
                y + 82,
                infoFont,
                new SKPaint
                {
                    IsAntialias = true,
                    Color = TextMuted
                });

            y += 145;
        }

        // TABLE TITLE
        canvas.DrawText(
            "TOP PERFORMING STUDENTS",
            Margin, y + 22,
            sectionFont, textPaint);

        y += 42;

        // #, STUDENT, TEACHER, GENDER, COURSE, PERFORMANCE, ATTENDANCE, CLASSES
        var widths = new[]
        {
            55f,
            245f,
            260f,
            110f,
            300f,
            210f,
            210f,
            150f
        };

        var scale = ContentWidth / widths.Sum();

        for (var i = 0; i < widths.Length; i++)
            widths[i] *= scale;

        var headers = new[]
        {
            "#",
            "STUDENT",
            "TEACHER",
            "GENDER",
            "COURSE",
            "PERFORMANCE",
            "ATTENDANCE",
            "CLASSES"
        };

        using var tableHeaderPaint = new SKPaint
        {
            IsAntialias = true,
            Color = AcademyBlue
        };

        canvas.DrawRoundRect(
            new SKRect(
                Margin,
                y,
                Width - Margin,
                y + tableHeaderHeight),
            8, 8, tableHeaderPaint);

        var x = (float)Margin;

        for (var i = 0; i < headers.Length; i++)
        {
            canvas.DrawText(
                headers[i],
                x + 10,
                y + 36,
                headerFont,
                whitePaint);

            x += widths[i];
        }

        y += tableHeaderHeight;

        for (var row = 0; row < report.Students.Count; row++)
        {
            var student = report.Students[row];

            using var rowPaint = new SKPaint
            {
                IsAntialias = true,
                Color = row % 2 == 0
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

            DrawCell(canvas, student.Rank.ToString(),
                x, y, widths[0], rowHeight, bodyFont, TextDark);
            x += widths[0];

            DrawCell(canvas, student.StudentName,
                x, y, widths[1], rowHeight, bodyBoldFont, TextDark);
            x += widths[1];

            DrawCell(canvas, student.TeacherName,
                x, y, widths[2], rowHeight, bodyFont, TextDark);
            x += widths[2];

            DrawCell(canvas, student.Gender,
                x, y, widths[3], rowHeight, bodyFont, TextMuted);
            x += widths[3];

            DrawCell(canvas, student.CourseName,
                x, y, widths[4], rowHeight, bodyFont, TextDark);
            x += widths[4];

            DrawMetric(
                canvas,
                student.PerformanceScore,
                x, y, widths[5], rowHeight,
                true);
            x += widths[5];

            DrawMetric(
                canvas,
                student.AttendancePercentage,
                x, y, widths[6], rowHeight,
                false);
            x += widths[6];

            DrawCell(canvas,
                student.ClassesReported.ToString(),
                x, y, widths[7], rowHeight,
                bodyFont, TextDark);

            y += rowHeight;
        }

        y += 35;

        using var verseFont = new SKFont(normal, 16);
        using var sourceFont = new SKFont(normal, 12);

        var footerPaint = new SKPaint
        {
            IsAntialias = true,
            Color = TextMuted
        };

        CenterText(
            canvas,
            "\"And say, My Lord, increase me in knowledge\"",
            Width / 2f, y,
            verseFont, footerPaint);

        CenterText(
            canvas,
            "— Quran 20:114",
            Width / 2f, y + 24,
            sourceFont, footerPaint);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(
            SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }

    private static void DrawLogo(SKCanvas canvas, SKRect card)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Clogo.png");

        if (!File.Exists(path))
            return;

        using var logo = SKBitmap.Decode(path);

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
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.White
        };

        canvas.DrawRoundRect(
            new SKRect(x, y, x + width, y + 112),
            10, 10, paint);

        using var border = new SKPaint
        {
            IsAntialias = true,
            Color = BorderColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };

        canvas.DrawRoundRect(
            new SKRect(x, y, x + width, y + 112),
            10, 10, border);

        using var accentPaint = new SKPaint
        {
            IsAntialias = true,
            Color = accent
        };

        canvas.DrawRoundRect(
            new SKRect(
                x + 12, y + 18,
                x + 16, y + 94),
            2, 2, accentPaint);

        using var bold = SKTypeface.FromFamilyName(
            "Arial", SKFontStyle.Bold);

        using var normal = SKTypeface.FromFamilyName(
            "Arial", SKFontStyle.Normal);

        using var titleFont = new SKFont(bold, 13);
        using var valueFont = new SKFont(bold, 28);
        using var descriptionFont = new SKFont(normal, 11);

        canvas.DrawText(
            title,
            x + 28,
            y + 30,
            titleFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = TextMuted
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
            descriptionFont,
            new SKPaint
            {
                IsAntialias = true,
                Color = TextMuted
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
        text = FitText(text, font, width - 20);

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
        var color = MetricColor(value, performance);
        var light = MetricLight(value, performance);

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = light
        };

        var rect = new SKRect(
            x + 10,
            y + 11,
            x + width - 10,
            y + height - 11);

        canvas.DrawRoundRect(
            rect, 8, 8, paint);

        using var typeface = SKTypeface.FromFamilyName(
            "Arial", SKFontStyle.Bold);

        using var font = new SKFont(typeface, 16);

        var text = $"{value:0.##}%";
        var textWidth = font.MeasureText(text);

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

    private static SKColor MetricColor(
        double value,
        bool performance)
    {
        if (!performance)
        {
            if (value >= 90) return Green;
            if (value >= 75) return Blue;
            if (value >= 50) return Orange;
            return Red;
        }

        if (value >= 90) return Green;
        if (value >= 75) return Blue;
        if (value >= 60) return Teal;
        if (value >= 40) return Orange;
        return Red;
    }

    private static SKColor MetricLight(
        double value,
        bool performance)
    {
        if (!performance)
        {
            if (value >= 90) return GreenLight;
            if (value >= 75) return BlueLight;
            if (value >= 50) return OrangeLight;
            return RedLight;
        }

        if (value >= 90) return GreenLight;
        if (value >= 75) return BlueLight;
        if (value >= 60) return TealLight;
        if (value >= 40) return OrangeLight;
        return RedLight;
    }

    private static string FitText(
        string text,
        SKFont font,
        float maxWidth)
    {
        if (font.MeasureText(text) <= maxWidth)
            return text;

        while (text.Length > 3 &&
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
        var width = font.MeasureText(text);

        canvas.DrawText(
            text,
            center - width / 2,
            y,
            font,
            paint);
    }

    private static void DrawWrappedText(
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

        foreach (var word in text.Split(' ',
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
        var list = new List<string>();

        if (!string.IsNullOrWhiteSpace(course))
            list.Add($"Course: {course}");

        if (!string.IsNullOrWhiteSpace(gender))
            list.Add($"Gender: {gender}");

        if (from.HasValue)
            list.Add($"From: {from:dd MMM yyyy}");

        if (to.HasValue)
            list.Add($"To: {to:dd MMM yyyy}");

        return list.Count == 0
            ? "All Students"
            : string.Join("  |  ", list);
    }
}
