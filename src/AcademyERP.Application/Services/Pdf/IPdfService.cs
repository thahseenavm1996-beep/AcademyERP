namespace AcademyERP.Application.Services.Pdf;

public interface IPdfService
{
    byte[] GenerateAttendanceReport(
        AcademyERP.Application.DTOs.AttendanceReports.AttendanceReportDashboardResponse report);

    byte[] GenerateClassReportsReport(
        IReadOnlyList<
            AcademyERP.Application.DTOs.ClassReports.ClassReportResponse> reports,
        AcademyERP.Application.DTOs.ClassReports.ClassReportQueryRequest request);

    byte[] GenerateFeeInvoiceReport(
        AcademyERP.Application.DTOs.Fees.FeeInvoiceResponse invoice);

    byte[] GenerateTeacherReportMonitoringReport(
        IReadOnlyList<
            AcademyERP.Application.DTOs.ClassReports.TeacherReportMonitoringResponse> reports,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? teacherId,
        string? courseName,
        AcademyERP.Domain.Enums.Gender? gender,
        bool? reportSubmitted);
}