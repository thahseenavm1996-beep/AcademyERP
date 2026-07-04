namespace AcademyERP.Admin.Models;

public class StudentListResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public StudentData Data { get; set; } = new();
}

public class StudentData
{
    public List<StudentResponse> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}