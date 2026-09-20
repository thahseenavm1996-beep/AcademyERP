using AcademyERP.Application.DTOs.ClassReports;

namespace AcademyERP.Application.Services.Reports;

public interface IClassReportImageService
{
    byte[] GenerateClassReportsImage(
        IReadOnlyList<ClassReportResponse> reports,
        ClassReportQueryRequest request,
        bool hideTeacherColumn = false);
}
