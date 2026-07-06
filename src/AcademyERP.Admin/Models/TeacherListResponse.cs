namespace AcademyERP.Admin.Models;

public class TeacherListResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public TeacherData Data { get; set; } = new();
}

public class TeacherData
{
    public List<TeacherResponse> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}