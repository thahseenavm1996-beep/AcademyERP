namespace AcademyERP.Application.DTOs.TeacherStudentRequests;

public class ReviewTeacherStudentRequestDto
{
    public bool Approve { get; set; }

    public decimal StudentFee { get; set; }

    public decimal TeacherSalary { get; set; }

    public string? AdminRemarks { get; set; }
}
