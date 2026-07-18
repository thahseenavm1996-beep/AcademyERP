namespace AcademyERP.Application.DTOs.Programs;

public class UpdateProgramRequest
{
    public string ProgramCode { get; set; } = string.Empty;

    public string ProgramName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DefaultDurationMinutes { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }
}