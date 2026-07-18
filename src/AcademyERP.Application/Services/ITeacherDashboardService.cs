using AcademyERP.Application.DTOs.TeacherDashboard;

namespace AcademyERP.Application.Services;

public interface ITeacherDashboardService
{
    Task<TeacherDashboardResponse> GetDashboardAsync(Guid teacherId);
}