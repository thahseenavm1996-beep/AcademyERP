using AcademyERP.Admin.Models.ClassProgress;
namespace AcademyERP.Admin.Models.Attendance;

public class CompleteClassRequest
{
    public CreateAttendanceRequest Attendance { get; set; } = new();

    public CreateClassProgressRequest Progress { get; set; } = new();
}