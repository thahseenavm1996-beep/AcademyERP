using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.ClassReports;

public class ClassReportQueryRequest
{
    public Guid? TeacherId { get; set; }

    public Guid? StudentId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public AttendanceStatus? AttendanceStatus { get; set; }

    public PerformanceRating? PerformanceRating { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}