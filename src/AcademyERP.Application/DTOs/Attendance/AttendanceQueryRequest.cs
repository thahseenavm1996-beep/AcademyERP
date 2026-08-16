using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Attendance;

public class AttendanceQueryRequest
{
    public Guid? StudentId { get; set; }
    public Guid? TeacherId { get; set; }
    public Guid? ProgramId { get; set; }
    public AttendanceStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
