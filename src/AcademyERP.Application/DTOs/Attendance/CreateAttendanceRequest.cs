using AcademyERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Attendance;

public class CreateAttendanceRequest
{
    [Required]
    public Guid ScheduledClassId { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    [MaxLength(2000)]
    public string? Remarks { get; set; }
}