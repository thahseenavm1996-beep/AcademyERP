using AcademyERP.Application.DTOs.TeacherStudentRequests;

namespace AcademyERP.Application.Interfaces.TeacherStudentRequests;

public interface ITeacherStudentRequestService
{
    Task<TeacherStudentRequestResponse> CreateAsync(
        Guid teacherId,
        CreateTeacherStudentRequestDto request);

    Task<List<TeacherStudentRequestResponse>> GetForTeacherAsync(
        Guid teacherId);

    Task<List<TeacherStudentRequestResponse>> GetPendingAsync();

    Task<TeacherStudentRequestResponse?> GetByIdAsync(Guid id);

    Task<TeacherStudentRequestResponse> ReviewAsync(
        Guid id,
        Guid reviewerId,
        ReviewTeacherStudentRequestDto request);

    Task<TeacherStudentRequestResponse> LinkStudentAsync(
        Guid id,
        Guid studentId);

    Task<FindStudentResponse?> FindStudentAsync(
        string studentName,
        string studentPhoneNumber);
}
