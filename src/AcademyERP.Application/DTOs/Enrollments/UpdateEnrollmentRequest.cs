using AcademyERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Enrollments;

public class UpdateEnrollmentRequest
{
    public Guid? TeacherId { get; set; }

    [Required]
    public Guid TimeSlotId { get; set; }

    [Required]
    public Guid ClassDurationId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MonthlyFee { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ScholarshipAmount { get; set; }

    public string? ScholarshipReason { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    public string? DiscountReason { get; set; }

    public EnrollmentStatus Status { get; set; }

    public string? Remarks { get; set; }
}