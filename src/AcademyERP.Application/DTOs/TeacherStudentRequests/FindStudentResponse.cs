namespace AcademyERP.Application.DTOs.TeacherStudentRequests;

public class FindStudentResponse
{
    public Guid Id { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}
