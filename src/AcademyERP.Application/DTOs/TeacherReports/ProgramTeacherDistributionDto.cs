namespace AcademyERP.Application.DTOs.TeacherReports;

public class ProgramTeacherDistributionDto
{
    public string ProgramName { get; set; } = string.Empty;

    public int TeachersCount { get; set; }

    public int StudentsCount { get; set; }
}