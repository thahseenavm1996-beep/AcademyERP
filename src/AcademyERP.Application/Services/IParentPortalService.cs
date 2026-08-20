using AcademyERP.Application.DTOs.ParentPortal;

namespace AcademyERP.Application.Services;

public interface IParentPortalService
{
   Task<ParentDashboardResponse?> GetDashboardAsync(
    Guid applicationUserId,
    string parentName);
}