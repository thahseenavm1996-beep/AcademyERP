using System.ComponentModel.DataAnnotations;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Students;

public class CreateStudentRequest
{

    public string AdmissionNumber { get; set; } = string.Empty;


    public string FullName { get; set; } = string.Empty;


    public DateTime AdmissionDate { get; set; }


    public DateTime? DateOfBirth { get; set; }


    public Gender Gender { get; set; }

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

   [EmailAddress]
public string? Email { get; set; }

    public string Country { get; set; } = string.Empty;


    public string TimeZone { get; set; } = string.Empty;
[MinLength(6)]
public string? Password { get; set; }
    public string? Remarks { get; set; }
}