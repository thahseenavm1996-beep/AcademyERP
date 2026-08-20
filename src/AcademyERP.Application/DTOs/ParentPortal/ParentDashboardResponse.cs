namespace AcademyERP.Application.DTOs.ParentPortal;

public class ParentDashboardResponse
{
    public string ParentName { get; set; } = string.Empty;


    public int ChildrenCount { get; set; }


    public decimal PendingFeeAmount { get; set; }


    public List<ParentChildSummaryDto> Children { get; set; }
        = new();


    public List<UpcomingClassDto> UpcomingClasses { get; set; }
        = new();
}