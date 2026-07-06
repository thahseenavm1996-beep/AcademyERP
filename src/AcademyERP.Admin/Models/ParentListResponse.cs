namespace AcademyERP.Admin.Models;

public class ParentListResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public ParentData Data { get; set; } = new();
}

public class ParentData
{
    public List<ParentResponse> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}