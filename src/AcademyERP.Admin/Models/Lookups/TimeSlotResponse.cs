namespace AcademyERP.Admin.Models.Lookups;

public class TimeSlotResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}