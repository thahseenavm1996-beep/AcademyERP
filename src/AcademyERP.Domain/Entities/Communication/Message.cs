using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Identity;

namespace AcademyERP.Domain.Entities.Communication;

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }

    public Conversation Conversation { get; set; } = null!;


    public Guid SenderUserId { get; set; }

    public ApplicationUser Sender { get; set; } = null!;


    public Guid ReceiverUserId { get; set; }

    public ApplicationUser Receiver { get; set; } = null!;


    public string MessageText { get; set; } = "";


    public DateTime SentAt { get; set; }
        = DateTime.UtcNow;


    public bool IsRead { get; set; }
}