using AcademyERP.Application.DTOs.AttendanceReports;
using AcademyERP.Application.Services.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Infrastructure.Services;

public class PdfService : IPdfService
{
    public byte[] GenerateAttendanceReport(
        AttendanceReportDashboardResponse report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x =>
                    x.FontSize(9));

                var logoBytes = GetLogoBytes();

page.Header()
    .Column(column =>
    {
        column.Item()
            .Row(row =>
            {
                row.ConstantItem(90)
                    .Height(65)
                    .Image(logoBytes)
                    .FitArea();

                row.RelativeItem()
                    .PaddingLeft(15)
                    .AlignMiddle()
                    .Column(headerColumn =>
                    {
                        headerColumn.Item()
                            .Text("MS INTERNATIONAL ACADEMY")
                            .FontSize(20)
                            .Bold();

                        headerColumn.Item()
                            .Text("MUEENU SSALIK")
                            .FontSize(10)
                            .SemiBold();

                        headerColumn.Item()
                            .PaddingTop(5)
                            .Text("Attendance Report")
                            .FontSize(14)
                            .SemiBold();
                    });
            });

        column.Item()
            .PaddingTop(8)
            .LineHorizontal(1);
    });

                page.Content()
                    .PaddingTop(15)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item()
                            .Row(row =>
                            {
                                AddSummaryCard(
                                    row,
                                    "Total Classes",
                                    report.TotalClasses.ToString());

                                AddSummaryCard(
                                    row,
                                    "Completed",
                                    report.CompletedClasses.ToString());

                                AddSummaryCard(
                                    row,
                                    "Attendance Records",
                                    report.TotalAttendanceRecords.ToString());

                                AddSummaryCard(
                                    row,
                                    "Average Attendance",
                                    $"{report.AverageAttendance:0.##}%");
                            });

                        AddSectionTitle(
                            column,
                            "Attendance Trend");

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                AddHeader(table, "Month", "Attendance");

                                foreach (var item in report.AttendanceTrend)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.Month);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text($"{item.Percentage:0.##}%");
                                }
                            });

                        AddSectionTitle(
                            column,
                            "Attendance Status Distribution");

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                AddHeader(table, "Status", "Count");

                                foreach (var item in report.StatusDistribution)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.Status);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.Count.ToString());
                                }
                            });

                        AddSectionTitle(
                            column,
                            "Course Attendance");

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                });

                                AddHeader(
                                    table,
                                    "Course",
                                    "Attendance");

                                foreach (var item in report.CourseAttendance)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.CourseName);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(
                                            $"{item.AttendancePercentage:0.##}%");
                                }
                            });

                        AddSectionTitle(
                            column,
                            "Students Needing Attention");

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                });

                                AddHeader(
                                    table,
                                    "Student",
                                    "Course",
                                    "Attendance");

                                foreach (
                                    var item
                                    in report.StudentsNeedingAttention)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.StudentName);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.CourseName);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(
                                            $"{item.AttendancePercentage:0.##}%");
                                }
                            });

                        AddSectionTitle(
                            column,
                            "Teacher Attendance Summary");

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                AddHeader(
                                    table,
                                    "Teacher",
                                    "Classes",
                                    "Attendance");

                                foreach (var item in report.TeacherSummary)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.TeacherName);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(item.TotalClasses.ToString());

                                    table.Cell()
                                        .BorderBottom(1)
                                        .Padding(5)
                                        .Text(
                                            $"{item.AttendancePercentage:0.##}%");
                                }
                            });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generated by MS INTERNATIONAL ACADEMY • ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

private static byte[] GetLogoBytes()
{
    var logoPath = Path.Combine(
        AppContext.BaseDirectory,
        "Assets",
        "ms-academy-logo.png");

    return File.ReadAllBytes(logoPath);
}

    private static void AddSummaryCard(
        RowDescriptor row,
        string title,
        string value)
    {
        row.RelativeItem()
            .Border(1)
            .Padding(8)
            .Column(column =>
            {
                column.Item()
                    .Text(title)
                    .FontSize(8);

                column.Item()
                    .PaddingTop(3)
                    .Text(value)
                    .FontSize(14)
                    .Bold();
            });
    }

    private static void AddSectionTitle(
        ColumnDescriptor column,
        string title)
    {
        column.Item()
            .PaddingTop(5)
            .Text(title)
            .FontSize(12)
            .Bold();
    }

    private static void AddHeader(
        TableDescriptor table,
        params string[] headers)
    {
        foreach (var header in headers)
        {
            table.Cell()
                .BorderBottom(1)
                .Padding(5)
                .Text(header)
                .Bold();
        }
    }
    public byte[] GenerateClassReportsReport(
    IReadOnlyList<ClassReportResponse> reports,
    ClassReportQueryRequest request)
{
    var logoBytes = GetLogoBytes();

    // Teacher portal reports are already restricted to the logged-in teacher.
    // Use the teacher name from the returned reports instead of showing "All".
    var teacherNames = reports
    .Select(x => x.TeacherName)
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Distinct()
    .ToList();

var teacherName =
    !string.IsNullOrWhiteSpace(request.TeacherName)
        ? request.TeacherName
        : teacherNames.Count == 1
            ? teacherNames[0]
            : teacherNames.Count > 1
                ? "All Teachers"
                : "Teacher";

    // Highest performance first.
    var orderedReports = reports
    .OrderByDescending(x =>
        string.Equals(
            x.AttendanceStatusText,
            "Absent",
            StringComparison.OrdinalIgnoreCase)
            ? 0
            : (int)x.PerformanceRating)
    .ThenByDescending(x => x.ClassDate)
    .ThenBy(x => x.StudentName)
    .ToList();

    var reportFromDate = request.FromDate.HasValue
        ? DateOnly.FromDateTime(request.FromDate.Value)
        : orderedReports.Count > 0
            ? orderedReports.Min(x => x.ClassDate)
            : (DateOnly?)null;

    var reportToDate = request.ToDate.HasValue
        ? DateOnly.FromDateTime(request.ToDate.Value)
        : orderedReports.Count > 0
            ? orderedReports.Max(x => x.ClassDate)
            : (DateOnly?)null;

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);

            page.DefaultTextStyle(x =>
                x.FontSize(8));

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.ConstantItem(90)
                                .Height(65)
                                .Image(logoBytes)
                                .FitArea();

                            row.RelativeItem()
                                .PaddingLeft(15)
                                .AlignMiddle()
                                .Column(headerColumn =>
                                {
                                    headerColumn.Item()
                                        .Text("MS INTERNATIONAL ACADEMY")
                                        .FontSize(18)
                                        .Bold();

                                    headerColumn.Item()
                                        .Text("MUEENU SSALIK")
                                        .FontSize(10)
                                        .SemiBold();

                                    headerColumn.Item()
                                        .PaddingTop(5)
                                        .Text("Teacher Class Report")
                                        .FontSize(14)
                                        .SemiBold();

                                    headerColumn.Item()
                                        .PaddingTop(4)
                                        .Text($"Teacher: {teacherName}")
                                        .FontSize(10)
                                        .Bold();
                                });
                        });

                    column.Item()
                        .PaddingTop(8)
                        .LineHorizontal(1);
                });

            page.Content()
                .PaddingTop(15)
                .Column(column =>
                {
                    column.Spacing(10);

                    // Report context
                    column.Item()
                        .Border(1)
                        .Padding(8)
                        .Column(context =>
                        {
                            context.Item()
                                .Text("Report Information")
                                .FontSize(11)
                                .Bold();

                            context.Item()
                                .PaddingTop(4)
                                .Text(
                                    $"Teacher: {teacherName}");

                            context.Item()
                                .PaddingTop(2)
                                .Text(
                                    $"Report Period: " +
                                    $"{FormatReportDate(reportFromDate)} - " +
                                    $"{FormatReportDate(reportToDate)}");

                            if (!string.IsNullOrWhiteSpace(request.CourseName))
                            {
                                context.Item()
                                    .PaddingTop(2)
                                    .Text(
                                        $"Course: {request.CourseName}");
                            }

                            context.Item()
                                .PaddingTop(2)
                                .Text(
                                    $"Total Class Reports: {orderedReports.Count}")
                                .Bold();
                        });

                    // Main report table
                    column.Item()
                        .PaddingTop(5)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.2f); // Student
                                columns.RelativeColumn(1.8f); // Course
                                columns.RelativeColumn(1.4f); // Attendance
                                columns.RelativeColumn(1.7f); // Homework
                                columns.RelativeColumn(1.5f); // Performance
                            });

                            AddClassReportHeader(
                                table,
                                "Student",
                                "Course",
                                "Attendance",
                                "Homework",
                                "Performance");

                            foreach (var report in orderedReports)
                            {
                                table.Cell()
                                    .BorderBottom(1)
                                    .Padding(5)
                                    .Text(report.StudentName);

                                table.Cell()
                                    .BorderBottom(1)
                                    .Padding(5)
                                    .Text(report.CourseName);

                                var attendanceStatus = report.AttendanceStatusText ?? string.Empty;

if (string.Equals(
        attendanceStatus,
        "Absent",
        StringComparison.OrdinalIgnoreCase))
{
    table.Cell()
        .BorderBottom(1)
        .Padding(5)
        .Background("#FEE2E2")
        .Text("Absent")
        .FontColor("#DC2626")
        .Bold();

    table.Cell()
        .BorderBottom(1)
        .Padding(5)
        .Text("—")
        .FontColor("#9CA3AF");

    table.Cell()
        .BorderBottom(1)
        .Padding(5)
        .Background("#F3F4F6")
        .Text("Nil")
        .FontColor("#6B7280")
        .Bold();
}
else
{
    var attendanceColor =
        string.Equals(attendanceStatus, "Present",
            StringComparison.OrdinalIgnoreCase)
            ? "#16A34A"
            : string.Equals(attendanceStatus, "Late",
                StringComparison.OrdinalIgnoreCase)
                ? "#F59E0B"
                : "#374151";

    table.Cell()
        .BorderBottom(1)
        .Padding(5)
        .Text(attendanceStatus)
        .FontColor(attendanceColor)
        .Bold();

    AddHomeworkBadge(
        table,
        report.HomeworkStatus);

    AddPerformanceBadge(
        table,
        report.PerformanceRating);
}
                            }
                        });

                    AddSectionTitle(
                        column,
                        "Class Report Details");

                    foreach (var report in orderedReports)
                    {
                        var isAbsent = string.Equals(
    report.AttendanceStatusText,
    "Absent",
    StringComparison.OrdinalIgnoreCase);

column.Item()
    .Border(1)
    .BorderColor(isAbsent ? "#DC2626" : "#D1D5DB")
    .Padding(8)
                            .Column(detail =>
                            {
                                detail.Item()
                                    .Text(
                                        $"{report.StudentName} — " +
                                        $"{report.ClassDate:dd MMM yyyy}")
                                    .FontSize(10)
                                    .Bold();

                                detail.Item()
                                    .PaddingTop(3)
                                    .Text(
    $"Course: {report.CourseName} | " +
    $"Attendance: {report.AttendanceStatusText}")
    .FontColor(isAbsent ? "#DC2626" : "#374151")
    .Bold();

                         


detail.Item()
    .PaddingTop(3)
    .Row(row =>
    {
        row.AutoItem()
            .Text("Homework: ")
            .Bold();

        if (isAbsent)
        {
            row.AutoItem()
                .Text("—")
                .FontColor("#9CA3AF");

            row.AutoItem()
                .PaddingLeft(12)
                .Text("Performance: ")
                .Bold();

            row.AutoItem()
                .Text("Nil")
                .FontColor("#6B7280")
                .Bold();
        }
        else
        {
            AddHomeworkInlineBadge(
                row,
                report.HomeworkStatus);

            row.AutoItem()
                .PaddingLeft(12)
                .Text("Performance: ")
                .Bold();

            AddPerformanceInlineBadge(
                row,
                report.PerformanceRating);
        }
    });

                                if (!string.IsNullOrWhiteSpace(report.LessonTaken))
                                {
                                    detail.Item()
                                        .PaddingTop(4)
                                        .Text(
                                            $"Lesson Taken: {report.LessonTaken}");
                                }

                                if (!string.IsNullOrWhiteSpace(report.NextHomework))
                                {
                                    detail.Item()
                                        .PaddingTop(3)
                                        .Text(
                                            $"Next Homework: {report.NextHomework}");
                                }

                                if (!string.IsNullOrWhiteSpace(
                                        report.TeacherRemarks))
                                {
                                    detail.Item()
                                        .PaddingTop(3)
                                        .Text(
                                            $"Teacher Remarks: {report.TeacherRemarks}");
                                }
                            });
                    }
                });

            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span(
                        "Generated by MS INTERNATIONAL ACADEMY • ");

                    text.CurrentPageNumber();

                    text.Span(" / ");

                    text.TotalPages();
                });
        });
    });

    return document.GeneratePdf();
}

private static string GetFilterValue(string? value)
{
    return string.IsNullOrWhiteSpace(value)
        ? "All"
        : value;
}

private static string GetFilterDate(DateTime? value)
{
    return value.HasValue
        ? value.Value.ToString("dd MMM yyyy")
        : "All";
}

public byte[] GenerateFeeInvoiceReport(
    AcademyERP.Application.DTOs.Fees.FeeInvoiceResponse invoice)
{
    var logoBytes = GetLogoBytes();

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(35);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.ConstantItem(90)
                                .Image(logoBytes)
                                .FitArea();

                            row.RelativeItem()
                                .PaddingLeft(15)
                                .Column(text =>
                                {
                                    text.Item()
                                        .Text("MS INTERNATIONAL ACADEMY")
                                        .Bold()
                                        .FontSize(17);

                                    text.Item()
                                        .Text("MUEENU SSALIK")
                                        .FontSize(10);

                                    text.Item()
                                        .PaddingTop(5)
                                        .Text("Fee Invoice")
                                        .Bold()
                                        .FontSize(13);
                                });
                        });

                    column.Item()
                        .PaddingTop(10)
                        .LineHorizontal(1);
                });

            page.Content()
                .PaddingTop(20)
                .Column(column =>
                {
                    // Invoice information
                    column.Item()
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Column(left =>
                                {
                                    left.Item()
                                        .Text("INVOICE TO")
                                        .Bold()
                                        .FontSize(9);

                                    left.Item()
                                        .PaddingTop(5)
                                        .Text(invoice.StudentName)
                                        .Bold()
                                        .FontSize(13);

                                    left.Item()
                                        .PaddingTop(3)
                                        .Text(invoice.CourseName);
                                });

                            row.RelativeItem()
                                .Column(right =>
                                {
                                    right.Item()
                                        .AlignRight()
                                        .Text("INVOICE")
                                        .Bold()
                                        .FontSize(9);

                                    right.Item()
                                        .PaddingTop(5)
                                        .AlignRight()
                                        .Text(invoice.InvoiceNumber)
                                        .Bold()
                                        .FontSize(12);

                                    right.Item()
                                        .PaddingTop(3)
                                        .AlignRight()
                                        .Text(
                                            $"Invoice Date: {invoice.InvoiceDate:dd MMM yyyy}");

                                    right.Item()
                                        .AlignRight()
                                        .Text(
                                            $"Due Date: {invoice.DueDate:dd MMM yyyy}");
                                            right.Item()
    .AlignRight()
    .Text(
        $"Paid Date: {(invoice.PaidDate.HasValue ? invoice.PaidDate.Value.ToString("dd MMM yyyy") : "—")}");
                                });
                        });

                    // Invoice table
                    column.Item()
                        .PaddingTop(25)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Element(HeaderCell)
                                    .Text("Description");

                                header.Cell()
                                    .Element(HeaderCell)
                                    .AlignRight()
                                    .Text("Amount");

                                header.Cell()
                                    .Element(HeaderCell)
                                    .AlignRight()
                                    .Text("Paid");

                                header.Cell()
                                    .Element(HeaderCell)
                                    .AlignRight()
                                    .Text("Balance");
                            });

                            table.Cell()
                                .Element(BodyCell)
                                .Text($"Course Fee - {invoice.CourseName}");

                            table.Cell()
                                .Element(BodyCell)
                                .AlignRight()
                                .Text(invoice.Amount.ToString("0.00"));

                            table.Cell()
                                .Element(BodyCell)
                                .AlignRight()
                                .Text(invoice.PaidAmount.ToString("0.00"));

                            table.Cell()
                                .Element(BodyCell)
                                .AlignRight()
                                .Text(invoice.BalanceAmount.ToString("0.00"));
                        });

                    // Summary
                    column.Item()
                        .PaddingTop(25)
                        .AlignRight()
                        .Column(summary =>
                        {
                            summary.Item()
                                .Width(250)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Invoice Amount");

                                    row.ConstantItem(90)
                                        .AlignRight()
                                        .Text(invoice.Amount.ToString("0.00"));
                                });

                           

                            summary.Item()
                                .PaddingTop(6)
                                .Width(250)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Paid");

                                    row.ConstantItem(90)
                                        .AlignRight()
                                        .Text(
                                            invoice.PaidAmount
                                                .ToString("0.00"));
                                });

                            summary.Item()
                                .PaddingTop(10)
                                .Width(250)
                                .LineHorizontal(1);

                            summary.Item()
                                .PaddingTop(8)
                                .Width(250)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Balance")
                                        .Bold();

                                    row.ConstantItem(90)
                                        .AlignRight()
                                        .Text(
                                            invoice.BalanceAmount
                                                .ToString("0.00"))
                                        .Bold();
                                });
                        });

                    // Status and remarks
                    column.Item()
                        .PaddingTop(30)
                        .Column(notes =>
                        {
                            notes.Item()
                                .Text("Status")
                                .Bold();

                            notes.Item()
                                .PaddingTop(4)
                                .Text(GetFeeStatusText(invoice.Status));

                            if (!string.IsNullOrWhiteSpace(invoice.Remarks))
                            {
                                notes.Item()
                                    .PaddingTop(15)
                                    .Text("Remarks")
                                    .Bold();

                                notes.Item()
                                    .PaddingTop(4)
                                    .Text(invoice.Remarks);
                            }
                        });
                });

            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span(
                        "Generated by MS INTERNATIONAL ACADEMY • ");

                    text.CurrentPageNumber();

                    text.Span(" / ");

                    text.TotalPages();
                });
        });
    });

    return document.GeneratePdf();
}
private static string GetFeeStatusText(FeeStatus status)
{
    return status.ToString();
}

private static IContainer HeaderCell(IContainer container)
{
    return container
        .Background("#E9EEF5")
        .BorderBottom(1)
        .BorderColor("#B8C2CC")
        .Padding(7);
}

private static IContainer BodyCell(IContainer container)
{
    return container
        .BorderBottom(1)
        .BorderColor("#E1E5E9")
        .Padding(7);
}

public byte[] GenerateTeacherReportMonitoringReport(
    IReadOnlyList<
        AcademyERP.Application.DTOs.ClassReports.TeacherReportMonitoringResponse> reports,
    DateTime? fromDate,
    DateTime? toDate,
    Guid? teacherId,
    string? courseName,
    AcademyERP.Domain.Enums.Gender? gender,
    bool? reportSubmitted)
{
    var logoBytes = GetLogoBytes();

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);

            page.DefaultTextStyle(x => x.FontSize(8));

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.ConstantItem(90)
                                .Height(65)
                                .Image(logoBytes)
                                .FitArea();

                            row.RelativeItem()
                                .PaddingLeft(15)
                                .AlignMiddle()
                                .Column(headerColumn =>
                                {
                                    headerColumn.Item()
                                        .Text("MS INTERNATIONAL ACADEMY")
                                        .FontSize(18)
                                        .Bold();

                                    headerColumn.Item()
                                        .Text("MUEENU SSALIK")
                                        .FontSize(10)
                                        .SemiBold();

                                    headerColumn.Item()
                                        .PaddingTop(5)
                                        .Text("Teacher Report Monitoring")
                                        .FontSize(14)
                                        .SemiBold();
                                });
                        });

                    column.Item()
                        .PaddingTop(8)
                        .LineHorizontal(1);
                });

            page.Content()
                .PaddingTop(15)
                .Column(column =>
                {
                    column.Spacing(8);

                    column.Item()
                        .Text("Report Filters")
                        .FontSize(12)
                        .Bold();

                    column.Item()
                        .Text(
                            $"From: {GetFilterDate(fromDate)}    " +
                            $"To: {GetFilterDate(toDate)}    " +
                            $"Course: {GetFilterValue(courseName)}");

                    var scheduledCount = reports.Count;
                    var requiredReports = reports.Where(x =>
    !string.Equals(x.AttendanceStatus, "Absent", StringComparison.OrdinalIgnoreCase) &&
    !string.Equals(x.AttendanceStatus, "Leave", StringComparison.OrdinalIgnoreCase))
    .ToList();

var sentCount = requiredReports.Count(x => x.ReportSubmitted);
var missingCount = requiredReports.Count(x => !x.ReportSubmitted);
var noReportNeededCount = reports.Count - requiredReports.Count;

                    var submissionRate = scheduledCount == 0
                        ? 0
                        : sentCount * 100.0 / scheduledCount;

                    column.Item()
                        .Text(
    $"Scheduled Classes: {reports.Count}    " +
    $"Reports Sent: {sentCount}    " +
    $"Reports Missing: {missingCount}    " +
    $"No Report Needed: {noReportNeededCount}    " +
    $"Submission Rate: {(requiredReports.Count == 0 ? 0 : sentCount * 100.0 / requiredReports.Count):0.##}%")
.Bold();

                    column.Item()
                        .PaddingTop(5)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                               columns.RelativeColumn(1.1f);
columns.RelativeColumn(1.5f);
columns.RelativeColumn(1.4f);
columns.RelativeColumn(1.4f);
columns.RelativeColumn(0.9f);
columns.RelativeColumn(1.4f);
columns.RelativeColumn(1.0f);
                            });

                            table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Date")
    .Bold();

table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Time")
    .Bold();

table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Student")
    .Bold();
table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Employee ID")
    .Bold();
table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Teacher")
    .Bold();

table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Course")
    .Bold();

table.Cell()
    .BorderBottom(1)
    .Padding(5)
    .Text("Report")
    .Bold();

                            foreach (var report in reports)
                            {
                                table.Cell()
                                    .BorderBottom(0.5f)
                                    .Padding(4)
                                    .Text(report.ClassDate.ToString("dd MMM yyyy"));

                                table.Cell()
                                    .BorderBottom(0.5f)
                                    .Padding(4)
                                    .Text(
                                        $"{report.StartTime:hh\\:mm tt} - " +
                                        $"{report.EndTime:hh\\:mm tt}");

                                table.Cell()
                                    .BorderBottom(0.5f)
                                    .Padding(4)
                                    .Text(report.StudentName);
                                table.Cell()
    .BorderBottom(0.5f)
    .Padding(4)
    .Text(report.EmployeeCode);

                                table.Cell()
                                    .BorderBottom(0.5f)
                                    .Padding(4)
                                    .Text(report.TeacherName);

                                table.Cell()
                                    .BorderBottom(0.5f)
                                    .Padding(4)
                                    .Text(report.CourseName);

                                table.Cell()
                                    .BorderBottom(0.5f)
                                    .Padding(4)
                                   .Text(
    string.Equals(report.AttendanceStatus, "Absent", StringComparison.OrdinalIgnoreCase) ||
    string.Equals(report.AttendanceStatus, "Leave", StringComparison.OrdinalIgnoreCase)
        ? "NO REPORT NEEDED"
        : report.ReportSubmitted
            ? "SENT"
            : "NOT SENT");
                            }
                        });
                });

            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("Teacher Report Monitoring • ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        });
    });

    return document.GeneratePdf();
}
private static void AddClassReportHeader(
    TableDescriptor table,
    params string[] headers)
{
    foreach (var header in headers)
    {
        table.Cell()
            .Background("#1268CF")
            .Padding(6)
            .Text(header)
            .FontSize(8)
            .Bold()
            .FontColor("#FFFFFF");
    }
}

private static void AddPerformanceBadge(
    TableDescriptor table,
    PerformanceRating rating)
{
    var (background, textColor) = GetPerformanceColors(rating);

    table.Cell()
        .BorderBottom(1)
        .Padding(4)
        .AlignCenter()
        .Background(background)
        .Text(rating.ToString())
        .Bold()
        .FontColor(textColor);
}

private static void AddHomeworkBadge(
    TableDescriptor table,
    HomeworkStatus status)
{
    var (background, textColor) = GetHomeworkColors(status);

    table.Cell()
        .BorderBottom(1)
        .Padding(4)
        .AlignCenter()
        .Background(background)
        .Text(GetHomeworkText(status))
        .Bold()
        .FontColor(textColor);
}

private static void AddPerformanceInlineBadge(
    RowDescriptor row,
    PerformanceRating rating)
{
    var (background, textColor) = GetPerformanceColors(rating);

    row.AutoItem()
        .Background(background)
        .PaddingHorizontal(5)
        .PaddingVertical(2)
        .Text(rating.ToString())
        .Bold()
        .FontColor(textColor);
}

private static void AddHomeworkInlineBadge(
    RowDescriptor row,
    HomeworkStatus status)
{
    var (background, textColor) = GetHomeworkColors(status);

    row.AutoItem()
        .Background(background)
        .PaddingHorizontal(5)
        .PaddingVertical(2)
        .Text(GetHomeworkText(status))
        .Bold()
        .FontColor(textColor);
}

private static (string Background, string TextColor)
    GetPerformanceColors(PerformanceRating rating)
{
    return rating switch
    {
        PerformanceRating.Excellent =>
            ("#DCFCE7", "#166534"),

        PerformanceRating.VeryGood =>
            ("#DBEAFE", "#1E40AF"),

        PerformanceRating.Good =>
            ("#CCFBF1", "#115E59"),

        PerformanceRating.Average =>
            ("#FEF3C7", "#92400E"),

        PerformanceRating.Poor =>
            ("#FEE2E2", "#991B1B"),

        _ =>
            ("#F3F4F6", "#374151")
    };
}

private static (string Background, string TextColor)
    GetHomeworkColors(HomeworkStatus status)
{
    return status switch
    {
        HomeworkStatus.Completed =>
            ("#DCFCE7", "#166534"),

        HomeworkStatus.PartiallyCompleted =>
            ("#FEF3C7", "#92400E"),

        HomeworkStatus.NotCompleted =>
            ("#FEE2E2", "#991B1B"),

        _ =>
            ("#F3F4F6", "#374151")
    };
}

private static string GetHomeworkText(HomeworkStatus status)
{
    return status switch
    {
        HomeworkStatus.Completed =>
            "Completed",

        HomeworkStatus.PartiallyCompleted =>
            "Partially Completed",

        HomeworkStatus.NotCompleted =>
            "Not Completed",

        _ =>
            status.ToString()
    };
}

private static string FormatReportDate(DateOnly? date)
{
    return date.HasValue
        ? date.Value.ToString("dd MMM yyyy")
        : "All Dates";
}

public byte[] GenerateFeeInvoicesReport(
    IReadOnlyList<AcademyERP.Application.DTOs.Fees.FeeInvoiceResponse> invoices)
{
    var logoBytes = GetLogoBytes();

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(25);

            page.DefaultTextStyle(x =>
                x.FontSize(8));

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.ConstantItem(70)
                                .Image(logoBytes)
                                .FitArea();

                            row.RelativeItem()
                                .PaddingLeft(12)
                                .Column(text =>
                                {
                                    text.Item()
                                        .Text("MS INTERNATIONAL ACADEMY")
                                        .Bold()
                                        .FontSize(16);

                                    text.Item()
                                        .Text("MUEENU SSALIK")
                                        .FontSize(9);

                                    text.Item()
                                        .PaddingTop(4)
                                        .Text("Fee Invoice Report")
                                        .Bold()
                                        .FontSize(12);
                                });

                            row.ConstantItem(180)
                                .AlignRight()
                                .Column(text =>
                                {
                                    text.Item()
                                        .AlignRight()
                                        .Text(
                                            $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");

                                    text.Item()
                                        .AlignRight()
                                        .PaddingTop(3)
                                        .Text(
                                            $"Total Invoices: {invoices.Count}");
                                });
                        });

                    column.Item()
                        .PaddingTop(8)
                        .LineHorizontal(1);
                });

            page.Content()
                .PaddingTop(15)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(75);  // Invoice
                        columns.RelativeColumn(2.1f); // Student
                        columns.RelativeColumn(1.3f); // Teacher
                        columns.RelativeColumn(1.7f); // Course
                        columns.ConstantColumn(65);   // Invoice date
                        columns.ConstantColumn(65);   // Due date
                        columns.ConstantColumn(65);   // Amount
                        columns.ConstantColumn(65);   // Paid
                        columns.ConstantColumn(65);   // Balance
                        columns.ConstantColumn(70);   // Status
                    });

                    table.Header(header =>
                    {
                        void HeaderCell(
                            IContainer container,
                            string text)
                        {
                            container
                                .Padding(5)
                                .BorderBottom(1)
                                .Text(text)
                                .Bold()
                                .FontSize(8);
                        }

                        HeaderCell(header.Cell(), "Invoice No");
                        HeaderCell(header.Cell(), "Student");
                        HeaderCell(header.Cell(), "Teacher");
                        HeaderCell(header.Cell(), "Course");
                        HeaderCell(header.Cell(), "Invoice Date");
                        HeaderCell(header.Cell(), "Due Date");
                        HeaderCell(header.Cell(), "Amount");
                        HeaderCell(header.Cell(), "Paid");
                        HeaderCell(header.Cell(), "Balance");
                        HeaderCell(header.Cell(), "Status");
                    });

                    foreach (var invoice in invoices)
                    {
                        var status =
                            invoice.BalanceAmount <= 0
                                ? "Paid"
                                : invoice.DueDate.Date < DateTime.Today
                                    ? "Overdue"
                                    : invoice.PaidAmount > 0
                                        ? "Partially Paid"
                                        : "Pending";

                        void Cell(
                            IContainer container,
                            string text)
                        {
                            container
                                .Padding(4)
                                .BorderBottom(0.5f)
                                .Text(text)
                                .FontSize(7.5f);
                        }

                        Cell(
                            table.Cell(),
                            invoice.InvoiceNumber);

                        Cell(
                            table.Cell(),
                            invoice.StudentName);

                        Cell(
                            table.Cell(),
                            invoice.TeacherName ?? "—");

                        Cell(
                            table.Cell(),
                            invoice.CourseName);

                        Cell(
                            table.Cell(),
                            invoice.InvoiceDate.ToString("dd/MM/yyyy"));

                        Cell(
                            table.Cell(),
                            invoice.DueDate.ToString("dd/MM/yyyy"));

                        Cell(
                            table.Cell(),
                            $"₹{invoice.Amount:N2}");

                        Cell(
                            table.Cell(),
                            $"₹{invoice.PaidAmount:N2}");

                        Cell(
                            table.Cell(),
                            $"₹{invoice.BalanceAmount:N2}");

                        Cell(
                            table.Cell(),
                            status);
                    }
                });

            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("MS INTERNATIONAL ACADEMY • Fee Invoice Report • ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        });
    });

    return document.GeneratePdf();
}


public byte[] GenerateTeacherRegistrationReport(
    IReadOnlyList<
        AcademyERP.Application.DTOs.TeacherRegistrations.TeacherRegistrationResponse>
        requests)
{
    var logoBytes = GetLogoBytes();

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(25);

            page.DefaultTextStyle(x => x.FontSize(8));

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.ConstantItem(70)
                                .Image(logoBytes)
                                .FitArea();

                            row.RelativeItem()
                                .PaddingLeft(12)
                                .Column(text =>
                                {
                                    text.Item()
                                        .Text("MS INTERNATIONAL ACADEMY")
                                        .Bold()
                                        .FontSize(16);

                                    text.Item()
                                        .Text("MUEENU SSALIK")
                                        .FontSize(9);

                                    text.Item()
                                        .PaddingTop(4)
                                        .Text("Teacher Registration Requests")
                                        .Bold()
                                        .FontSize(12);
                                });

                            row.ConstantItem(180)
                                .AlignRight()
                                .Column(text =>
                                {
                                    text.Item()
                                        .AlignRight()
                                        .Text(
                                            $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");

                                    text.Item()
                                        .AlignRight()
                                        .PaddingTop(3)
                                        .Text(
                                            $"Total Requests: {requests.Count}");
                                });
                        });

                    column.Item()
                        .PaddingTop(8)
                        .LineHorizontal(1);
                });

            page.Content()
                .PaddingTop(15)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2.2f);
                        columns.ConstantColumn(75);
                        columns.ConstantColumn(90);
                        columns.RelativeColumn(2.2f);
                        columns.RelativeColumn(1.8f);
                        columns.ConstantColumn(85);
                        columns.ConstantColumn(75);
                    });

                    table.Header(header =>
                    {
                        void HeaderCell(
                            IContainer container,
                            string text)
                        {
                            container
                                .Padding(5)
                                .BorderBottom(1)
                                .Text(text)
                                .Bold()
                                .FontSize(8);
                        }

                        HeaderCell(header.Cell(), "Name");
                        HeaderCell(header.Cell(), "Gender");
                        HeaderCell(header.Cell(), "Mobile");
                        HeaderCell(header.Cell(), "Email");
                        HeaderCell(header.Cell(), "Department");
                        HeaderCell(header.Cell(), "Submitted");
                        HeaderCell(header.Cell(), "Status");
                    });

                    foreach (var request in requests)
                    {
                        void Cell(
                            IContainer container,
                            string text)
                        {
                            container
                                .Padding(4)
                                .BorderBottom(0.5f)
                                .Text(text)
                                .FontSize(7.5f);
                        }

                        Cell(
                            table.Cell(),
                            request.FullName);

                        Cell(
                            table.Cell(),
                            ((int)request.Gender) switch
                            {
                                1 => "Male",
                                2 => "Female",
                                3 => "Other",
                                _ => "Unknown"
                            });

                        Cell(
                            table.Cell(),
                            request.MobileNumber);

                        Cell(
                            table.Cell(),
                            request.Email);

                        Cell(
                            table.Cell(),
                            request.Department);

                        Cell(
                            table.Cell(),
                            request.SubmittedAt
                                .ToLocalTime()
                                .ToString("dd/MM/yyyy"));

                        Cell(
                            table.Cell(),
                            ((int)request.Status) switch
                            {
                                1 => "Pending",
                                2 => "Approved",
                                3 => "Rejected",
                                _ => "Unknown"
                            });
                    }
                });

            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("MS INTERNATIONAL ACADEMY • Teacher Registration Requests • ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        });
    });

    return document.GeneratePdf();
}
}
