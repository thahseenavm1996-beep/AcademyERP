using AcademyERP.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AcademyERP.Infrastructure.Services.Reports;

public class BestPerformingStudentsPdfService
{
    public byte[] Generate(
        BestPerformingStudentsReportResponse report,
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
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("MS INTERNATIONAL ACADEMY")
                            .FontSize(20)
                            .Bold();

                        column.Item()
                            .Text("MUEENU SSALIK")
                            .FontSize(10)
                            .SemiBold();

                        column.Item()
                            .PaddingTop(5)
                            .Text("Best Performing Students")
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

                        column.Item().Text(BuildFilterText(
                            courseName,
                            gender,
                            fromDate,
                            toDate));

                        column.Item()
                            .Row(row =>
                            {
                                AddCard(row, "Total Students",
                                    report.TotalStudents.ToString());

                                AddCard(row, "With Reports",
                                    report.StudentsWithReports.ToString());

                                AddCard(row, "Avg Performance",
                                    $"{report.AveragePerformance:0.##}%");

                                AddCard(row, "Avg Attendance",
                                    $"{report.AverageAttendance:0.##}%");
                            });

                        if (report.BestStudent is not null)
                        {
                            column.Item()
                                .Border(1)
                                .Padding(10)
                                .Column(best =>
                                {
                                    best.Item()
                                        .Text("BEST PERFORMING STUDENT")
                                        .Bold()
                                        .FontSize(11);

                                    best.Item()
                                        .PaddingTop(5)
                                        .Text(
                                            $"{report.BestStudent.StudentName}  •  " +
                                            $"{report.BestStudent.PerformanceScore:0.##}% Performance  •  " +
                                            $"{report.BestStudent.AttendancePercentage:0.##}% Attendance")
                                        .FontSize(12)
                                        .Bold();
                                });
                        }

                        column.Item()
                            .Text("TOP PERFORMING STUDENTS")
                            .FontSize(12)
                            .Bold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(65);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(85);
                                columns.ConstantColumn(85);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                HeaderCell(header.Cell(), "#");
                                HeaderCell(header.Cell(), "STUDENT");
                                HeaderCell(header.Cell(), "GENDER");
                                HeaderCell(header.Cell(), "COURSE");
                                HeaderCell(header.Cell(), "PERFORMANCE");
                                HeaderCell(header.Cell(), "ATTENDANCE");
                                HeaderCell(header.Cell(), "CLASSES");
                            });

                            foreach (var student in report.Students)
                            {
                                BodyCell(table.Cell(), student.Rank.ToString());
                                BodyCell(table.Cell(), student.StudentName);
                                BodyCell(table.Cell(), student.Gender);
                                BodyCell(table.Cell(), student.CourseName);
                                BodyCell(table.Cell(),
                                    $"{student.PerformanceScore:0.##}%");
                                BodyCell(table.Cell(),
                                    $"{student.AttendancePercentage:0.##}%");
                                BodyCell(table.Cell(),
                                    student.ClassesReported.ToString());
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("MS INTERNATIONAL ACADEMY")
                    .FontSize(8);
            });
        }).GeneratePdf();
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
                column.Item().Text(title).FontSize(8).SemiBold();
                column.Item().PaddingTop(3).Text(value).FontSize(14).Bold();
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
}
