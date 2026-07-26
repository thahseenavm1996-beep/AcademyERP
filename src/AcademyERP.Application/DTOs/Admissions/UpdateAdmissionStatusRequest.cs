using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Admissions;

public class UpdateAdmissionStatusRequest
{
    public AdmissionStatus Status { get; set; }
}