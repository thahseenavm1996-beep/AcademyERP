using AcademyERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Attendance;

public class UpdateAttendanceRequest
{
    [Required]
    public AttendanceStatus Status { get; set; }

    [MaxLength(2000)]
    public string? Remarks { get; set; }
}
