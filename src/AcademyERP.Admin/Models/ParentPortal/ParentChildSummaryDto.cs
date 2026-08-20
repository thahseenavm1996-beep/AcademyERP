namespace AcademyERP.Admin.Models.ParentPortal;

public class ParentChildSummaryDto
{
    public Guid StudentId { get; set; }


    public string StudentName { get; set; } = string.Empty;


    public string CourseName { get; set; } = string.Empty;


    public string TeacherName { get; set; } = string.Empty;


    public decimal AttendancePercentage { get; set; }


    public decimal ProgressPercentage { get; set; }

    public bool HasAttendanceRecords { get; set; }

public bool HasProgressRecords { get; set; }

}