using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid CourseId { get; set; }

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

    public string? Remarks { get; set; }
}