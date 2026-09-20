using AcademyERP.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AcademyERP.Infrastructure.Services.Reports;

public class BestPerformingFacultyPdfService
{
    public byte[] Generate(
        BestPerformingFacultyReportResponse report,
        string? courseName,
        string? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);

                page.DefaultTextStyle(x =>
                    x.FontSize(9));

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text(
                                "MS INTERNATIONAL ACADEMY")
                            .FontSize(20)
                            .Bold();

                        column.Item()
                            .Text("MUEENU SSALIK")
                            .FontSize(10)
                            .SemiBold();

                        column.Item()
                            .PaddingTop(5)
                            .Text(
                                "Best Performing Faculty")
                            .FontSize(15)
                            .Bold();

                        column.Item()
                            .PaddingTop(5)
                            .LineHorizontal(1);
                    });

                page.Content()
                    .PaddingTop(15)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item()
                            .Text(
                                BuildFilterText(
                                    courseName,
                                    gender,
                                    fromDate,
                                    toDate));

                        column.Item()
                            .Row(row =>
                            {
                                AddCard(
                                    row,
                                    "Total Faculty",
                                    report.TotalFaculty.ToString());

                                AddCard(
                                    row,
                                    "With Reports",
                                    report.FacultyWithReports.ToString());

                                AddCard(
                                    row,
                                    "Avg Performance",
                                    $"{report.AveragePerformance:0.##}%");

                                AddCard(
                                    row,
                                    "Avg Attendance",
                                    $"{report.AverageAttendance:0.##}%");
                            });

                        if (report.BestFaculty is not null)
                        {
                            column.Item()
                                .Border(1)
                                .Padding(10)
                                .Column(best =>
                                {
                                    best.Item()
                                        .Text(
                                            "BEST PERFORMING FACULTY")
                                        .Bold()
                                        .FontSize(11);

                                    best.Item()
                                        .PaddingTop(5)
                                        .Text(
                                            $"{report.BestFaculty.TeacherName}  •  " +
                                            $"{report.BestFaculty.PerformanceScore:0.##}% Performance  •  " +
                                            $"{report.BestFaculty.AttendancePercentage:0.##}% Attendance")
                                        .FontSize(12)
                                        .Bold();

                                    best.Item()
                                        .PaddingTop(3)
                                        .Text(
                                            $"Students: {report.BestFaculty.StudentsTaught}  •  " +
                                            $"Classes: {report.BestFaculty.ClassesReported}")
                                        .FontSize(9);
                                });
                        }

                        column.Item()
                            .Text("TOP PERFORMING FACULTY")
                            .FontSize(12)
                            .Bold();

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(35);
                                    columns.RelativeColumn(2);
                                    columns.ConstantColumn(65);
                                    columns.RelativeColumn(2);
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(85);
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(75);
                                });

                                table.Header(header =>
                                {
                                    HeaderCell(header.Cell(), "#");
                                    HeaderCell(header.Cell(), "FACULTY");
                                    HeaderCell(header.Cell(), "GENDER");
                                    HeaderCell(header.Cell(), "COURSE");
                                    HeaderCell(header.Cell(), "STUDENTS");
                                    HeaderCell(header.Cell(), "PERFORMANCE");
                                    HeaderCell(header.Cell(), "ATTENDANCE");
                                    HeaderCell(header.Cell(), "CLASSES");
                                });

                                foreach (var faculty in report.Faculty)
                                {
                                    BodyCell(
                                        table.Cell(),
                                        faculty.Rank.ToString());

                                    BodyCell(
                                        table.Cell(),
                                        faculty.TeacherName);

                                    BodyCell(
                                        table.Cell(),
                                        faculty.Gender);

                                    BodyCell(
                                        table.Cell(),
                                        faculty.CourseName);

                                    BodyCell(
                                        table.Cell(),
                                        faculty.StudentsTaught.ToString());

                                    BodyCell(
                                        table.Cell(),
                                        $"{faculty.PerformanceScore:0.##}%");

                                    BodyCell(
                                        table.Cell(),
                                        $"{faculty.AttendancePercentage:0.##}%");

                                    BodyCell(
                                        table.Cell(),
                                        faculty.ClassesReported.ToString());
                                }
                            });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(
                        "MS INTERNATIONAL ACADEMY")
                    .FontSize(8);
            });
        }).GeneratePdf();
    }

    private static void AddCard(
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
                    .FontSize(8)
                    .SemiBold();

                column.Item()
                    .PaddingTop(3)
                    .Text(value)
                    .FontSize(14)
                    .Bold();
            });
    }

    private static void HeaderCell(
        IContainer container,
        string text)
    {
        container
            .Background(Colors.Grey.Lighten2)
            .Border(1)
            .Padding(5)
            .Text(text)
            .Bold();
    }

    private static void BodyCell(
        IContainer container,
        string text)
    {
        container
            .Border(1)
            .Padding(5)
            .Text(text);
    }

    private static string BuildFilterText(
        string? courseName,
        string? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(courseName))
            filters.Add($"Course: {courseName}");

        if (!string.IsNullOrWhiteSpace(gender))
            filters.Add($"Gender: {gender}");

        if (fromDate.HasValue)
            filters.Add($"From: {fromDate:dd MMM yyyy}");

        if (toDate.HasValue)
            filters.Add($"To: {toDate:dd MMM yyyy}");

        return filters.Count == 0
            ? "Filters: All"
            : $"Filters: {string.Join("  |  ", filters)}";
    }
}
