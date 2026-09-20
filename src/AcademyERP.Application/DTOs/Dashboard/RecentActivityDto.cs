namespace AcademyERP.Application.DTOs.Dashboard;

public class RecentActivityDto
{
    public Guid? EntityId { get; set; }

    public string ActivityType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; }
}