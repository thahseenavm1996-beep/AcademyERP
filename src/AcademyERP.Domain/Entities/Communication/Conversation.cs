using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Identity;

namespace AcademyERP.Domain.Entities.Communication;

public class Conversation : BaseEntity
{
    public Guid? StudentId { get; set; }


    public string Subject { get; set; } = "";


    public DateTime LastMessageAt { get; set; }


    public bool IsClosed { get; set; }


    public ICollection<Message> Messages { get; set; }
        = new List<Message>();
}