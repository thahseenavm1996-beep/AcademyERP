using AcademyERP.Admin.Models.Admissions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AcademyERP.Admin.Services.Exports;

public static class AdmissionApplicationExportService
{
    public static byte[] GeneratePdf(
        IEnumerable<AdmissionApplicationResponse> applications,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? enquiryOfficer = null)
    {
        var applicationList = applications
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(style => style.FontSize(9));

                page.Header()
                    .Column(header =>
                    {
                        header.Item()
                            .Text("MS INTERNATIONAL ACADEMY")
                            .Bold()
                            .FontSize(16)
                            .FontColor("#123B70");

                        header.Item()
                            .PaddingTop(3)
                            .Text("Admission Applications")
                            .FontSize(12)
                            .SemiBold();

                        header.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1)
                            .LineColor("#B8C2CC");
                    });

                page.Content()
                    .PaddingTop(16)
                    .Column(content =>
                    {
                        content.Item()
                            .PaddingBottom(8)
                            .Text($"Applications: {applicationList.Count}    •    Enquiry officer: {(string.IsNullOrWhiteSpace(enquiryOfficer) ? "Any" : enquiryOfficer.Trim())}    •    Submitted between: {FormatDate(fromDate)} and {FormatDate(toDate)}    •    Generated: {ConvertToIndiaTime(DateTime.UtcNow):dd MMM yyyy, hh:mm tt}")
                            .FontColor("#526173");

                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.2f);
                                columns.RelativeColumn(1.8f);
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn(1.7f);
                                columns.RelativeColumn(4.2f);
                            });

                            table.Header(header =>
                            {
                                HeaderCell(header.Cell()).Text("Student Name");
                                HeaderCell(header.Cell()).Text("Course");
                                HeaderCell(header.Cell()).Text("Current Status");
                                HeaderCell(header.Cell()).Text("Submitted At");
                                HeaderCell(header.Cell()).Text("Enquiry officer");
                            });

                            foreach (var application in applicationList)
                            {
                                BodyCell(table.Cell()).Text(application.StudentName);
                                BodyCell(table.Cell()).Text(application.CourseName ?? "Not assigned");
                                BodyCell(table.Cell()).Text(GetStatusText(application.Status));
                                BodyCell(table.Cell()).Text(
                                    ConvertToIndiaTime(application.CreatedAt).ToString("dd MMM yyyy, hh:mm tt"));
                                BodyCell(table.Cell()).Text(
                                    string.IsNullOrWhiteSpace(application.Message)
                                        ? "—"
                                        : application.Message);
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("MS INTERNATIONAL ACADEMY • ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static string FormatDate(DateTime? date)
    {
        return date?.ToString("dd MMM yyyy") ?? "Any date";
    }

    private static DateTime ConvertToIndiaTime(DateTime dateTime)
    {
        var utcDateTime = dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
        };

        return TimeZoneInfo.ConvertTimeFromUtc(
            utcDateTime,
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata"));
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background("#E9EEF5")
            .BorderBottom(1)
            .BorderColor("#B8C2CC")
            .Padding(7)
            .DefaultTextStyle(style => style.Bold().FontColor("#123B70"));
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .BorderBottom(0.5f)
            .BorderColor("#D9E0E8")
            .PaddingVertical(7)
            .PaddingHorizontal(6)
            .AlignMiddle();
    }

    private static string GetStatusText(int status)
    {
        return status switch
        {
            1 => "New",
            2 => "Contacted",
            3 => "Demo Scheduled",
            4 => "Demo Completed",
            5 => "Approved",
            6 => "Rejected",
            7 => "Cancelled",
            8 => "Demo Started",
            _ => "Unknown"
        };
    }
}
