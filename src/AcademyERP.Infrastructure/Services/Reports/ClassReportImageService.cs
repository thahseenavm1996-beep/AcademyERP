using AcademyERP.Application.DTOs.ClassReports;
using SkiaSharp;

namespace AcademyERP.Infrastructure.Services.Reports;

public class ClassReportImageService
    : AcademyERP.Application.Services.Reports.IClassReportImageService
{
    // =========================================================
    // PAGE
    // =========================================================

    private const int PageWidth = 1600;

    private const int MarginLeft = 60;
    private const int MarginRight = 60;

    private const int ContentWidth =
        PageWidth - MarginLeft - MarginRight;

    // =========================================================
    // COLORS
    // =========================================================

    private static readonly SKColor Navy =
        SKColor.Parse("#123F73");

    private static readonly SKColor NavyDark =
        SKColor.Parse("#0D315B");

    private static readonly SKColor Blue =
        SKColor.Parse("#1675C8");

    private static readonly SKColor PageBackground =
        SKColor.Parse("#F5F8FC");

    private static readonly SKColor White =
        SKColors.White;

    private static readonly SKColor Text =
        SKColor.Parse("#243B53");

    private static readonly SKColor TextMuted =
        SKColor.Parse("#718096");

    private static readonly SKColor Border =
        SKColor.Parse("#D9E3EC");

    private static readonly SKColor RowAlternate =
        SKColor.Parse("#F7FAFD");

    private static readonly SKColor Green =
        SKColor.Parse("#138A4B");

    private static readonly SKColor GreenLight =
        SKColor.Parse("#E2F5EA");

    private static readonly SKColor Red =
        SKColor.Parse("#C92A35");

    private static readonly SKColor RedLight =
        SKColor.Parse("#FCE7E9");

    private static readonly SKColor Purple =
        SKColor.Parse("#7146C8");

    private static readonly SKColor PurpleLight =
        SKColor.Parse("#F0EAFE");

    private static readonly SKColor Gray =
        SKColor.Parse("#68798A");

    private static readonly SKColor GrayLight =
        SKColor.Parse("#EEF2F5");

    private static readonly SKColor BlueLight =
        SKColor.Parse("#EAF4FC");

    // =========================================================
    // MAIN
    // =========================================================

   public byte[] GenerateClassReportsImage(
    IReadOnlyList<ClassReportResponse> reports,
    ClassReportQueryRequest request,
    bool hideTeacherColumn = false)
    {
        var items = reports
            .OrderByDescending(IsPresent)
            .ThenByDescending(x => (int)x.PerformanceRating)
            .ThenBy(x => x.StudentName)
            .ThenBy(x => x.ClassDate)
            .ToList();

        // -----------------------------------------------------
        // FILTER TYPE
        // -----------------------------------------------------

        var studentFiltered =
            !string.IsNullOrWhiteSpace(request.StudentName);

        var teacherFiltered =
            !string.IsNullOrWhiteSpace(request.TeacherName);

        string scopeLabel;
        string scopeValue;

        if (studentFiltered)
        {
            scopeLabel = "Student";
            scopeValue = request.StudentName!.Trim();
        }
        else if (teacherFiltered)
        {
            scopeLabel = "Teacher";
            scopeValue = request.TeacherName!.Trim();
        }
        else
        {
            scopeLabel = "Report Scope";
            scopeValue = "All Students";
        }

        // -----------------------------------------------------
        // DATES
        // -----------------------------------------------------

        var fromDate =
            request.FromDate.HasValue
                ? DateOnly.FromDateTime(request.FromDate.Value)
                : items.Count > 0
                    ? items.Min(x => x.ClassDate)
                    : (DateOnly?)null;

        var toDate =
            request.ToDate.HasValue
                ? DateOnly.FromDateTime(request.ToDate.Value)
                : items.Count > 0
                    ? items.Max(x => x.ClassDate)
                    : (DateOnly?)null;

                    var teacherName =
    hideTeacherColumn
        ? items
            .Select(x => x.TeacherName)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
        : null;

        // -----------------------------------------------------
        // SUMMARY
        // -----------------------------------------------------

        var totalStudents = items.Count;

        var totalTeachers =
            items
                .Select(x => x.TeacherName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

        var present =
            items.Count(IsPresent);

        var absent =
            items.Count(IsAbsent);

        var attendanceRate =
            totalStudents == 0
                ? 0
                : (int)Math.Round(
                    present * 100.0 / totalStudents);

        // -----------------------------------------------------
        // PAGE DIMENSIONS
        // -----------------------------------------------------

        const int pageTop = 30;

        const int headerHeight = 158;

        const int summaryTop = 220;
        const int summaryHeight = 112;

        const int tableTop = 365;

        const int tableHeaderHeight = 58;
        const int rowHeight = 68;

        const int verseGap = 28;
        const int verseHeight = 70;

        const int bottomMargin = 30;

        var rowCount =
            Math.Max(items.Count, 1);

        var tableHeight =
            tableHeaderHeight +
            rowCount * rowHeight;

        var tableBottom =
            tableTop + tableHeight;

        var verseTop =
            tableBottom + verseGap;

        var pageHeight =
            verseTop +
            verseHeight +
            bottomMargin;

        // -----------------------------------------------------
        // BITMAP
        // -----------------------------------------------------

        using var bitmap =
            new SKBitmap(
                PageWidth,
                pageHeight,
                SKColorType.Rgba8888,
                SKAlphaType.Premul);

        using var canvas =
            new SKCanvas(bitmap);

          

        canvas.Clear(PageBackground);

        // -----------------------------------------------------
        // PAGE
        // -----------------------------------------------------

        DrawPage(
            canvas,
            pageHeight);

        // -----------------------------------------------------
        // HEADER
        // -----------------------------------------------------

       DrawHeader(
    canvas,
    pageTop,
    headerHeight,
    scopeLabel,
    scopeValue,
    teacherName,
    fromDate,
    toDate);
        // -----------------------------------------------------
        // SUMMARY
        // -----------------------------------------------------

        DrawSummary(
            canvas,
            summaryTop,
            summaryHeight,
            studentFiltered
                ? totalTeachers
                : totalStudents,
            studentFiltered
                ? "TOTAL TEACHERS"
                : "TOTAL STUDENTS",
            present,
            absent,
            attendanceRate);

        // -----------------------------------------------------
        // TABLE
        // -----------------------------------------------------

        var columns =
    BuildColumns(
        showStudent: !studentFiltered,
        showTeacher: !teacherFiltered && !hideTeacherColumn);

        DrawTable(
            canvas,
            items,
            columns,
            tableTop,
            tableHeaderHeight,
            rowHeight);

        // -----------------------------------------------------
        // QURAN VERSE
        // -----------------------------------------------------

        DrawVerse(
            canvas,
            verseTop,
            verseHeight);

        // -----------------------------------------------------
        // ENCODE
        // -----------------------------------------------------

        using var image =
            SKImage.FromBitmap(bitmap);

        using var data =
            image.Encode(
                SKEncodedImageFormat.Png,
                100);

        return data.ToArray();
    }

    // =========================================================
    // PAGE
    // =========================================================

    private static void DrawPage(
        SKCanvas canvas,
        int pageHeight)
    {
        using var paint =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = White,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                24,
                24,
                PageWidth - 24,
                pageHeight - 24),
            12,
            12,
            paint);
    }

    // =========================================================
    // HEADER
    // =========================================================

    private static void DrawHeader(
    SKCanvas canvas,
    int top,
    int height,
    string scopeLabel,
    string scopeValue,
    string? teacherName,
    DateOnly? fromDate,
    DateOnly? toDate)
    {
       var x = MarginLeft;
var width = ContentWidth;

        // -----------------------------------------------------
        // Header
        // -----------------------------------------------------

        using var background =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Navy,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                x,
                top,
                x + width,
                top + height),
            12,
            12,
            background);

        // -----------------------------------------------------
        // Accent line
        // -----------------------------------------------------

        using var accent =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#55B9FF"),
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                x + 18,
                top + 20,
                x + 23,
                top + height - 20),
            2,
            2,
            accent);

        // -----------------------------------------------------
        // Logo panel
        // -----------------------------------------------------

        var logoX = x + 40;
var logoY = top + 18;
const int logoWidth = 165;
const int logoHeight = 122;

        using var logoPanel =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = White,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                logoX,
                logoY,
                logoX + logoWidth,
                logoY + logoHeight),
            10,
            10,
            logoPanel);

        DrawLogo(
            canvas,
            logoX + 12,
            logoY + 12,
            logoWidth - 24,
            logoHeight - 24);

        // -----------------------------------------------------
        // Title
        // -----------------------------------------------------

        var titleX =
            logoX + logoWidth + 28;

        DrawText(
            canvas,
            "Teacher Class Report",
            titleX,
            top + 57,
            40,
            White,
            true);

        DrawText(
            canvas,
            "Student progress  •  Attendance  •  Learning status",
            titleX,
            top + 88,
            17,
            SKColor.Parse("#E4F0FA"),
            false);

        // -----------------------------------------------------
        // Scope
        // -----------------------------------------------------

        DrawText(
            canvas,
            scopeLabel,
            titleX,
            top + 119,
            11,
            SKColor.Parse("#BBD5EC"),
            true);

        DrawText(
            canvas,
            scopeValue,
            titleX + 72,
            top + 119,
            15,
            White,
            true);
            if (!string.IsNullOrWhiteSpace(teacherName))
{
    DrawText(
        canvas,
        "Teacher",
        titleX,
        top + 145,
        11,
        SKColor.Parse("#BBD5EC"),
        true);

    DrawText(
        canvas,
        teacherName,
        titleX + 72,
        top + 145,
        15,
        White,
        true);
}

        // -----------------------------------------------------
        // Metadata
        // -----------------------------------------------------

        var metaX =
            x + width - 330;

        DrawText(
            canvas,
            "REPORT PERIOD",
            metaX,
            top + 38,
            10,
            SKColor.Parse("#BBD5EC"),
            true);

        var period =
            fromDate.HasValue &&
            toDate.HasValue
                ? $"{fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}"
                : "All Dates";

        DrawText(
            canvas,
            period,
            metaX,
            top + 61,
            14,
            White,
            true);

        DrawText(
            canvas,
            "GENERATED",
            metaX,
            top + 93,
            10,
            SKColor.Parse("#BBD5EC"),
            true);

        DrawText(
            canvas,
            DateTime.Now.ToString("dd MMM yyyy"),
            metaX,
            top + 116,
            14,
            White,
            true);
    }

    // =========================================================
    // LOGO
    // =========================================================

    private static void DrawLogo(
        SKCanvas canvas,
        float x,
        float y,
        float width,
        float height)
    {
        var path =
            Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "Clogo.png");

        if (!File.Exists(path))
            return;

        using var stream =
            File.OpenRead(path);

        using var logo =
            SKBitmap.Decode(stream);

        if (logo == null)
            return;

        var scale =
            Math.Min(
                width / logo.Width,
                height / logo.Height);

        var drawWidth =
            logo.Width * scale;

        var drawHeight =
            logo.Height * scale;

        var drawX =
            x + (width - drawWidth) / 2;

        var drawY =
            y + (height - drawHeight) / 2;

        using var paint =
            new SKPaint
            {
                IsAntialias = true
            };

        canvas.DrawBitmap(
            logo,
            new SKRect(
                drawX,
                drawY,
                drawX + drawWidth,
                drawY + drawHeight),
            paint);
    }

    // =========================================================
    // SUMMARY
    // =========================================================

    private static void DrawSummary(
        SKCanvas canvas,
        int top,
        int height,
        int firstValue,
        string firstTitle,
        int present,
        int absent,
        int attendanceRate)
    {
        const int gap = 18;

        var cardWidth =
            (ContentWidth - gap * 3) / 4;

        DrawMetricCard(
            canvas,
            MarginLeft,
            top,
            cardWidth,
            height,
            firstTitle,
            firstValue.ToString(),
            firstTitle == "TOTAL TEACHERS"
                ? "Teachers in this report"
                : "Students in this report",
            Blue);

        DrawMetricCard(
            canvas,
            MarginLeft + cardWidth + gap,
            top,
            cardWidth,
            height,
            "PRESENT",
            present.ToString(),
            "Students attended",
            Green);

        DrawMetricCard(
            canvas,
            MarginLeft +
            (cardWidth + gap) * 2,
            top,
            cardWidth,
            height,
            "ABSENT",
            absent.ToString(),
            "Students absent",
            Red);

        DrawMetricCard(
            canvas,
            MarginLeft +
            (cardWidth + gap) * 3,
            top,
            cardWidth,
            height,
            "ATTENDANCE RATE",
            $"{attendanceRate}%",
            "Overall attendance",
            Blue);
    }

    private static void DrawMetricCard(
        SKCanvas canvas,
        int x,
        int y,
        int width,
        int height,
        string title,
        string value,
        string subtitle,
        SKColor accent)
    {
        // -----------------------------------------------------
        // Card
        // -----------------------------------------------------

        using var card =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = White,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                x,
                y,
                x + width,
                y + height),
            7,
            7,
            card);

        // -----------------------------------------------------
        // Border
        // -----------------------------------------------------

        using var border =
            new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Border,
                StrokeWidth = 1,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                x,
                y,
                x + width,
                y + height),
            7,
            7,
            border);

        // -----------------------------------------------------
        // Accent
        // -----------------------------------------------------

        using var accentPaint =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = accent,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                x + 12,
                y + 16,
                x + 16,
                y + height - 16),
            2,
            2,
            accentPaint);

        // -----------------------------------------------------
        // Content
        // -----------------------------------------------------

        DrawText(
            canvas,
            title,
            x + 28,
            y + 30,
            11,
            TextMuted,
            true);

        DrawText(
            canvas,
            value,
            x + 28,
            y + 70,
            32,
            accent,
            true);

        DrawText(
            canvas,
            subtitle,
            x + 28,
            y + 96,
            11,
            TextMuted,
            false);
    }

    // =========================================================
    // TABLE COLUMNS
    // =========================================================

    private static List<TableColumn> BuildColumns(
        bool showStudent,
        bool showTeacher)
    {
        var columns =
            new List<TableColumn>();

        columns.Add(
            new("#", 55));

        if (showStudent)
        {
            columns.Add(
                new("STUDENT", 245));
        }

        if (showTeacher)
        {
            columns.Add(
                new("TEACHER", 185));
        }

        columns.Add(
            new("COURSE", 310));

        columns.Add(
            new("ATTENDANCE", 225));

        columns.Add(
            new("HOMEWORK", 225));

        columns.Add(
            new("PERFORMANCE", 225));

        return columns;
    }

    // =========================================================
    // TABLE
    // =========================================================

    private static void DrawTable(
        SKCanvas canvas,
        List<ClassReportResponse> reports,
        List<TableColumn> columns,
        int top,
        int headerHeight,
        int rowHeight)
    {
        const float left = MarginLeft;
        const float right = PageWidth - MarginRight;

        var tableWidth =
            right - left;

        var totalUnits =
            columns.Sum(x => x.Width);

        var scale =
            tableWidth / totalUnits;

        var positions =
            new float[columns.Count];

        var widths =
            new float[columns.Count];

        var currentX =
            left;

        for (var i = 0;
             i < columns.Count;
             i++)
        {
            positions[i] =
                currentX;

            widths[i] =
                columns[i].Width * scale;

            currentX +=
                widths[i];
        }

        // -----------------------------------------------------
        // Make absolutely sure the final column ends exactly
        // at the table boundary.
        // -----------------------------------------------------

        if (columns.Count > 0)
        {
            widths[^1] =
                right - positions[^1];
        }

        var rowCount =
            Math.Max(reports.Count, 1);

        var bottom =
            top +
            headerHeight +
            rowCount * rowHeight;

        // -----------------------------------------------------
        // Clip drawing to table.
        // -----------------------------------------------------

        canvas.Save();

        canvas.ClipRect(
            new SKRect(
                left,
                top,
                right,
                bottom));

        // -----------------------------------------------------
        // White table background
        // -----------------------------------------------------

        using var white =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = White,
                IsAntialias = true
            };

        canvas.DrawRect(
            left,
            top,
            right,
            bottom,
            white);

        // -----------------------------------------------------
        // Header
        // -----------------------------------------------------

        using var header =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Navy,
                IsAntialias = true
            };

        canvas.DrawRect(
            left,
            top,
            right,
            top + headerHeight,
            header);

        // -----------------------------------------------------
        // Header text
        // -----------------------------------------------------

        for (var i = 0;
             i < columns.Count;
             i++)
        {
            DrawText(
    canvas,
    columns[i].Name,
    positions[i] + 14,
    top + 37,
    14,
    White,
    true);
        }

        // -----------------------------------------------------
        // Empty
        // -----------------------------------------------------

        if (reports.Count == 0)
        {
            DrawCenteredText(
                canvas,
                "No class reports found",
                new SKRect(
                    left,
                    top + headerHeight,
                    right,
                    bottom),
                17,
                TextMuted,
                false);

            canvas.Restore();

            DrawTableOutline(
                canvas,
                left,
                top,
                right,
                bottom);

            return;
        }

        // -----------------------------------------------------
        // Rows
        // -----------------------------------------------------

        for (var index = 0;
             index < reports.Count;
             index++)
        {
            var report =
                reports[index];

            var rowTop =
                top +
                headerHeight +
                index * rowHeight;

            var rowBottom =
                rowTop +
                rowHeight;

            var rowColor =
                IsAbsent(report)
                    ? RedLight
                    : index % 2 == 0
                        ? White
                        : RowAlternate;

            using var rowPaint =
                new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = rowColor,
                    IsAntialias = true
                };

            canvas.DrawRect(
                left,
                rowTop,
                right,
                rowBottom,
                rowPaint);

            var columnIndex = 0;

            // -------------------------------------------------
            // NUMBER
            // -------------------------------------------------

            DrawText(
    canvas,
    (index + 1).ToString(),
    positions[columnIndex] + 14,
    rowTop + 43,
    16,
    TextMuted,
    true);

            columnIndex++;

            // -------------------------------------------------
            // STUDENT
            // -------------------------------------------------

            if (columns.Any(
                    x => x.Name == "STUDENT"))
            {
                DrawText(
    canvas,
    TrimText(
        report.StudentName,
        28),
    positions[columnIndex] + 14,
    rowTop + 43,
    18,
    NavyDark,
    true);

                columnIndex++;
            }

            // -------------------------------------------------
            // TEACHER
            // -------------------------------------------------

            if (columns.Any(
                    x => x.Name == "TEACHER"))
            {
                DrawText(
    canvas,
    TrimText(
        report.TeacherName,
        22),
    positions[columnIndex] + 14,
    rowTop + 43,
    17,
    Text,
    true);

                columnIndex++;
            }

            // -------------------------------------------------
            // COURSE
            // -------------------------------------------------

            DrawText(
    canvas,
    TrimText(
        report.CourseName,
        34),
    positions[columnIndex] + 14,
    rowTop + 43,
    17,
    Text,
    true);

            columnIndex++;

            // -------------------------------------------------
            // ATTENDANCE
            // -------------------------------------------------

            DrawStatus(
                canvas,
                GetAttendanceText(report),
                positions[columnIndex],
                widths[columnIndex],
                rowTop,
                rowHeight);

            columnIndex++;

            // -------------------------------------------------
            // HOMEWORK
            // -------------------------------------------------

            DrawStatus(
                canvas,
                GetHomeworkText(report),
                positions[columnIndex],
                widths[columnIndex],
                rowTop,
                rowHeight);

            columnIndex++;

            // -------------------------------------------------
            // PERFORMANCE
            // -------------------------------------------------

            DrawStatus(
                canvas,
                GetPerformanceText(report),
                positions[columnIndex],
                widths[columnIndex],
                rowTop,
                rowHeight);
        }

        // -----------------------------------------------------
        // Stop all table drawing here.
        // -----------------------------------------------------

        canvas.Restore();

        // -----------------------------------------------------
        // Outline
        // -----------------------------------------------------

        DrawTableOutline(
            canvas,
            left,
            top,
            right,
            bottom);
    }

    // =========================================================
    // TABLE OUTLINE
    // =========================================================

    private static void DrawTableOutline(
        SKCanvas canvas,
        float left,
        float top,
        float right,
        float bottom)
    {
        using var border =
            new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Border,
                StrokeWidth = 1,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                left,
                top,
                right,
                bottom),
            10,
            10,
            border);
    }

    // =========================================================
    // STATUS BADGES
    // =========================================================

    private static void DrawStatus(
        SKCanvas canvas,
        string text,
        float x,
        float width,
        float rowTop,
        float rowHeight)
    {
        var normalized =
            text.Trim()
                .ToLowerInvariant();

        var background =
            BlueLight;

        var foreground =
            Blue;

        if (normalized == "present")
        {
            background = GreenLight;
            foreground = Green;
        }
        else if (normalized == "absent")
        {
            background = RedLight;
            foreground = Red;
        }
        else if (normalized == "late")
        {
            background = BlueLight;
            foreground = Blue;
        }
        else if (normalized.Contains("partial"))
        {
            background = PurpleLight;
            foreground = Purple;
        }
        else if (normalized == "excellent")
        {
            // No yellow.
            background = GreenLight;
            foreground = Green;
        }
        else if (normalized == "very good")
        {
            background = GreenLight;
            foreground = Green;
        }
        else if (normalized == "good")
        {
            background = BlueLight;
            foreground = Blue;
        }
        else if (
            normalized == "nil" ||
            normalized == "—" ||
            normalized == "-")
        {
            background = GrayLight;
            foreground = Gray;
        }

        const float horizontalPadding = 12;
        const float verticalPadding = 11;

        var badgeX =
            x + horizontalPadding;

        var badgeY =
            rowTop + verticalPadding;

        var badgeWidth =
            width -
            horizontalPadding * 2;

        var badgeHeight =
            rowHeight -
            verticalPadding * 2;

        using var paint =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = background,
                IsAntialias = true
            };

        canvas.DrawRoundRect(
            new SKRect(
                badgeX,
                badgeY,
                badgeX + badgeWidth,
                badgeY + badgeHeight),
            8,
            8,
            paint);

        DrawCenteredText(
    canvas,
    text,
    new SKRect(
        badgeX,
        badgeY,
        badgeX + badgeWidth,
        badgeY + badgeHeight),
    16,
    foreground,
    true);
    }

    // =========================================================
    // QURAN VERSE
    // =========================================================

    private static void DrawVerse(
        SKCanvas canvas,
        int top,
        int height)
    {
        var centerX =
            PageWidth / 2f;

        DrawCenteredText(
            canvas,
            "\"And say, My Lord, increase me in knowledge\"",
            new SKRect(
                200,
                top + 2,
                PageWidth - 200,
                top + 30),
            16,
            Navy,
            true);

        DrawCenteredText(
            canvas,
            "— Quran 20:114",
            new SKRect(
                200,
                top + 32,
                PageWidth - 200,
                top + 55),
            12,
            TextMuted,
            false);
    }

    // =========================================================
    // TEXT
    // =========================================================

    private static void DrawText(
        SKCanvas canvas,
        string text,
        float x,
        float y,
        float size,
        SKColor color,
        bool bold)
    {
        using var typeface =
            SKTypeface.FromFamilyName(
                "DejaVu Sans",
                bold
                    ? SKFontStyle.Bold
                    : SKFontStyle.Normal);

        using var font =
            new SKFont(
                typeface,
                size);

        using var paint =
            new SKPaint
            {
                Color = color,
                IsAntialias = true
            };

        canvas.DrawText(
            text,
            x,
            y,
            font,
            paint);
    }

    // =========================================================
    // CENTER TEXT
    // =========================================================

    private static void DrawCenteredText(
        SKCanvas canvas,
        string text,
        SKRect rect,
        float size,
        SKColor color,
        bool bold)
    {
        using var typeface =
            SKTypeface.FromFamilyName(
                "DejaVu Sans",
                bold
                    ? SKFontStyle.Bold
                    : SKFontStyle.Normal);

        using var font =
            new SKFont(
                typeface,
                size);

        using var paint =
            new SKPaint
            {
                Color = color,
                IsAntialias = true
            };

        var textWidth =
            font.MeasureText(text);

        var metrics =
            font.Metrics;

        var x =
            rect.MidX -
            textWidth / 2;

        var y =
            rect.MidY -
            (metrics.Ascent +
             metrics.Descent) / 2;

        canvas.DrawText(
            text,
            x,
            y,
            font,
            paint);
    }

    // =========================================================
    // DATA HELPERS
    // =========================================================

    private static bool IsAbsent(
        ClassReportResponse report)
    {
        return string.Equals(
            report.AttendanceStatusText,
            "Absent",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPresent(
        ClassReportResponse report)
    {
        return
            string.Equals(
                report.AttendanceStatusText,
                "Present",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                report.AttendanceStatusText,
                "Late",
                StringComparison.OrdinalIgnoreCase);
    }

    private static string GetAttendanceText(
        ClassReportResponse report)
    {
        if (IsAbsent(report))
            return "Absent";

        if (string.Equals(
                report.AttendanceStatusText,
                "Late",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Late";
        }

        return "Present";
    }

    private static string GetHomeworkText(
        ClassReportResponse report)
    {
        if (IsAbsent(report))
            return "—";

        var value =
            report.HomeworkStatus.ToString();

        if (string.IsNullOrWhiteSpace(value) ||
            value == "0")
        {
            return "—";
        }

        if (value.Equals(
                "PartiallyCompleted",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Partially Completed";
        }

        if (value.Equals(
                "Completed",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Completed";
        }

        return value;
    }

    private static string GetPerformanceText(
        ClassReportResponse report)
    {
        if (IsAbsent(report))
            return "Nil";

        var value =
            report.PerformanceRating.ToString();

        if (string.IsNullOrWhiteSpace(value) ||
            value == "0")
        {
            return "Nil";
        }

        if (value.Equals(
                "VeryGood",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Very Good";
        }

        if (value.Equals(
                "Excellent",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Excellent";
        }

        return value;
    }

    private static string TrimText(
        string? value,
        int maxCharacters)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "—";

        var clean =
            value.Trim();

        if (clean.Length <= maxCharacters)
            return clean;

        return clean[
            ..Math.Max(
                1,
                maxCharacters - 1)] + "…";
    }

    // =========================================================
    // COLUMN MODEL
    // =========================================================

    private sealed record TableColumn(
        string Name,
        float Width);
}