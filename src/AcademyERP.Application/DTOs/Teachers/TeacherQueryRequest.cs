namespace AcademyERP.Application.DTOs.Teachers;

public class TeacherQueryRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }
}