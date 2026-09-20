using AcademyERP.Application.DTOs.Communication;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Communication;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.Constants;



namespace AcademyERP.Infrastructure.Services;

public class CommunicationService : ICommunicationService
{
    private readonly ApplicationDbContext _context;


    public CommunicationService(
        ApplicationDbContext context)
    {
        _context = context;
    }



   public async Task<List<ConversationDto>> GetMyConversationsAsync(
    Guid userId)
{
    return await _context.Conversations
        .Where(x =>
            x.Messages.Any(m =>
                m.SenderUserId == userId ||
                m.ReceiverUserId == userId))
        .OrderByDescending(x => x.LastMessageAt)
        .Select(x => new ConversationDto
        {
            Id = x.Id,

            StudentId = x.StudentId,

            StudentName =
                x.StudentId.HasValue
                    ? _context.Students
                        .Where(s =>
                            s.Id == x.StudentId.Value)
                        .Select(s =>
                            s.FullName)
                        .FirstOrDefault()
                        ?? ""
                    : "",

            LastMessage =
                x.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.MessageText)
                    .FirstOrDefault()
                    ?? "",

            LastMessageAt =
                x.LastMessageAt,

            OtherUserId =
                x.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m =>
                        m.SenderUserId == userId
                            ? m.ReceiverUserId
                            : m.SenderUserId)
                    .FirstOrDefault(),

            OtherUserName =
                x.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m =>
                        m.SenderUserId == userId
                            ? m.Receiver.FullName
                            : m.Sender.FullName)
                    .FirstOrDefault()
                    ?? "",

            UnreadCount =
                x.Messages.Count(m =>
                    m.ReceiverUserId == userId &&
                    !m.IsRead)
        })
        .ToListAsync();
}




    public async Task<List<MessageDto>> GetMessagesAsync(
        Guid conversationId,
        Guid userId)
    {
        var hasAccess =
    await _context.Messages
    .AnyAsync(x =>
        x.ConversationId == conversationId &&
        (
            x.SenderUserId == userId ||
            x.ReceiverUserId == userId
        ));


if(!hasAccess)
{
    throw new UnauthorizedAccessException(
        "You do not have access to this conversation.");
}

var unreadMessages = await _context.Messages
    .Where(x =>
        x.ConversationId == conversationId &&
        x.ReceiverUserId == userId &&
        !x.IsRead)
    .ToListAsync();

foreach (var message in unreadMessages)
{
    message.IsRead = true;
}

if (unreadMessages.Count > 0)
{
    await _context.SaveChangesAsync();
}
        return await _context.Messages

            .Where(x =>
                x.ConversationId == conversationId)

            .OrderBy(x =>
                x.SentAt)

            .Select(x => new MessageDto
            {
                Id = x.Id,

                SenderName =
                    x.Sender.FullName,

                MessageText =
                    x.MessageText,

                SentAt =
                    x.SentAt,

                IsMine =
                    x.SenderUserId == userId
            })

            .ToListAsync();
    }


public async Task<List<MessageDto>> GetAdminMessagesAsync(
    Guid conversationId,
    Guid adminUserId)
{
    var conversationExists =
        await _context.Conversations
            .AnyAsync(c =>
                c.Id == conversationId &&
                c.Messages.Any());

    if (!conversationExists)
    {
        throw new KeyNotFoundException(
            "Conversation was not found.");
    }

    // Mark messages received by this admin as read
    var unreadMessages =
        await _context.Messages
            .Where(m =>
                m.ConversationId == conversationId &&
                m.ReceiverUserId == adminUserId &&
                !m.IsRead)
            .ToListAsync();

    foreach (var message in unreadMessages)
    {
        message.IsRead = true;
    }

    if (unreadMessages.Count > 0)
    {
        await _context.SaveChangesAsync();
    }

    // Return the conversation messages
    return await _context.Messages
        .Where(m =>
            m.ConversationId == conversationId)
        .OrderBy(m => m.SentAt)
        .Select(m => new MessageDto
        {
            Id = m.Id,

            SenderName =
                m.Sender.FullName,

            MessageText =
                m.MessageText,

            SentAt =
                m.SentAt,

            IsMine =
                m.SenderUserId == adminUserId
        })
        .ToListAsync();
}


    public async Task SendMessageAsync(
        Guid senderUserId,
        SendMessageRequest request)
    {

        var message =
            new Message
            {
                ConversationId =
                    request.ConversationId,

                SenderUserId =
                    senderUserId,

                ReceiverUserId =
                    request.ReceiverUserId,

                MessageText =
                    request.MessageText,

                SentAt =
                    DateTime.UtcNow
            };

        var hasAccess =
    await _context.Messages
    .AnyAsync(x =>
        x.ConversationId == request.ConversationId &&
        (
            x.SenderUserId == senderUserId ||
            x.ReceiverUserId == senderUserId
        ));


if(!hasAccess)
{
    throw new UnauthorizedAccessException(
        "You do not have access to this conversation.");
}
        _context.Messages.Add(message);


        var conversation =
            await _context.Conversations
            .FirstAsync(x =>
                x.Id == request.ConversationId);


        conversation.LastMessageAt =
            DateTime.UtcNow;


        await _context.SaveChangesAsync();
    }

public async Task<List<TeacherMessageRecipientDto>> GetTeacherMessageRecipientsAsync(
    Guid teacherUserId)
{
    var teacher = await _context.Teachers
        .FirstOrDefaultAsync(x =>
            !x.IsDeleted &&
            x.ApplicationUserId == teacherUserId);

    if (teacher == null)
        throw new UnauthorizedAccessException("Teacher not found.");

    var recipients = new List<TeacherMessageRecipientDto>();

    // Administrators and SuperAdmins
    var adminRecipients = await (
        from user in _context.Users
        join userRole in _context.UserRoles
            on user.Id equals userRole.UserId
        join role in _context.Roles
            on userRole.RoleId equals role.Id
        where user.IsActive
              && (role.Name == Roles.Administrator ||
                  role.Name == Roles.SuperAdmin)
        select new TeacherMessageRecipientDto
        {
            UserId = user.Id,
            Name = user.FullName,
            Email = user.Email ?? "",
            RecipientType = "Administrator"
        })
        .Distinct()
        .OrderBy(x => x.Name)
        .ToListAsync();

    recipients.AddRange(adminRecipients);

    // Parents of students currently assigned to this teacher
    var parentRecipients = await _context.TeachingSchedules
        .Where(x =>
            !x.IsDeleted &&
            x.IsActive &&
            x.TeacherId == teacher.Id &&
            !x.Enrollment.IsDeleted &&
            x.Enrollment.Status == EnrollmentStatus.Active)
        .SelectMany(x => x.Enrollment.Student.StudentParents)
        .Where(x =>
            !x.IsDeleted &&
            !x.Parent.IsDeleted &&
            x.Parent.Status == UserStatus.Active &&
            x.Parent.ApplicationUser.IsActive)
        .Select(x => new TeacherMessageRecipientDto
        {
            UserId = x.Parent.ApplicationUserId,
            Name = x.Parent.FullName,
            Email = x.Parent.Email,
            StudentId = x.StudentId,
            StudentName = x.Student.FullName,
            RecipientType = "Parent"
        })
        .ToListAsync();

    parentRecipients = parentRecipients
        .GroupBy(x => new
        {
            x.UserId,
            x.StudentId
        })
        .Select(x => x.First())
        .OrderBy(x => x.Name)
        .ThenBy(x => x.StudentName)
        .ToList();

    recipients.AddRange(parentRecipients);

    return recipients;
}

public async Task<List<MessageRecipientDto>> GetTeacherRecipientsAsync()
{
    return await _context.Teachers
        .Where(x => !x.IsDeleted)
        .OrderBy(x => x.FullName)
        .Select(x => new MessageRecipientDto
        {
            TeacherId = x.Id,
            UserId = x.ApplicationUserId,
            Name = x.FullName,
            Email = x.Email
        })
        .ToListAsync();
}

public async Task<Guid> StartTeacherConversationAsync(
    Guid teacherUserId,
    Guid receiverUserId,
    string messageText,
    Guid? studentId = null)
{
    if (string.IsNullOrWhiteSpace(messageText))
        throw new ArgumentException("Message cannot be empty.");

    var teacher = await _context.Teachers
        .FirstOrDefaultAsync(x =>
            !x.IsDeleted &&
            x.ApplicationUserId == teacherUserId);

    if (teacher == null)
        throw new UnauthorizedAccessException("Teacher not found.");

    // Teacher -> Administrator / SuperAdmin
    var isAdministrator = await (
        from userRole in _context.UserRoles
        join role in _context.Roles
            on userRole.RoleId equals role.Id
        where userRole.UserId == receiverUserId
              && (role.Name == Roles.Administrator ||
                  role.Name == Roles.SuperAdmin)
        select role.Id)
        .AnyAsync();

    if (isAdministrator)
    {
        if (studentId.HasValue)
            studentId = null;
    }
    else
    {
        // Teacher -> Parent requires a specific student
        if (!studentId.HasValue)
            throw new UnauthorizedAccessException(
                "A student must be selected when messaging a parent.");

        var isAuthorizedParent = await _context.TeachingSchedules
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.IsActive &&
                x.TeacherId == teacher.Id &&
                !x.Enrollment.IsDeleted &&
                x.Enrollment.Status == EnrollmentStatus.Active &&
                x.Enrollment.StudentId == studentId.Value &&
                x.Enrollment.Student.StudentParents.Any(sp =>
                    !sp.IsDeleted &&
                    !sp.Parent.IsDeleted &&
                    sp.Parent.ApplicationUserId == receiverUserId &&
                    sp.Parent.Status == UserStatus.Active));

        if (!isAuthorizedParent)
            throw new UnauthorizedAccessException(
                "This parent is not associated with a student taught by this teacher.");
    }

    // Find an existing teacher-parent conversation for this student.
var conversation = await _context.Conversations
    .Where(c =>
        !c.IsClosed &&
        c.StudentId == studentId.Value &&
        c.Messages.Any(m =>
            (m.SenderUserId == teacherUserId &&
             m.ReceiverUserId == receiverUserId)
            ||
            (m.SenderUserId == receiverUserId &&
             m.ReceiverUserId == teacherUserId)))
    .OrderByDescending(c => c.LastMessageAt)
    .FirstOrDefaultAsync();

// Create a new conversation if one does not exist.
if (conversation == null)
{
    conversation = new Conversation
    {
        StudentId = studentId.Value,
        LastMessageAt = DateTime.UtcNow,
        IsClosed = false
    };

    _context.Conversations.Add(conversation);
}

// Add the teacher's message.
var message = new Message
{
    Conversation = conversation,
    SenderUserId = teacherUserId,
    ReceiverUserId = receiverUserId,
    MessageText = messageText,
    SentAt = DateTime.UtcNow,
    IsRead = false
};

_context.Messages.Add(message);

conversation.LastMessageAt = DateTime.UtcNow;

await _context.SaveChangesAsync();

return conversation.Id;
}

public async Task<List<MessageRecipientDto>> GetParentTeacherRecipientsAsync(
    Guid parentUserId)
{
    var parent = await _context.Parents
        .FirstOrDefaultAsync(x =>
            x.ApplicationUserId == parentUserId &&
            !x.IsDeleted);

    if (parent == null)
    {
        throw new UnauthorizedAccessException(
            "Parent account was not found.");
    }

    return await _context.TeachingSchedules
        .Where(x =>
            x.IsActive &&
            !x.IsDeleted &&

            x.Enrollment.Status == EnrollmentStatus.Active &&

            x.Enrollment.Student.StudentParents
                .Any(sp =>
                    sp.ParentId == parent.Id &&
                    !sp.IsDeleted) &&

            !x.Teacher.IsDeleted)
        .Select(x => new MessageRecipientDto
        {
            UserId = x.Teacher.ApplicationUserId,

            TeacherId = x.TeacherId,

            StudentId = x.Enrollment.StudentId,

            Name = x.Teacher.FullName,

            StudentName = x.Enrollment.Student.FullName,

            Email = x.Teacher.Email
        })
        .Distinct()
        .OrderBy(x => x.StudentName)
        .ThenBy(x => x.Name)
        .ToListAsync();
}

public async Task<Guid> StartConversationAsync(
    Guid senderUserId,
    Guid receiverUserId,
    string messageText,
    Guid? studentId = null)
{
    if (string.IsNullOrWhiteSpace(messageText))
        throw new ArgumentException(
            "Message cannot be empty.");

    // If a student is supplied, this is a
    // parent/teacher conversation and the
    // student context must be preserved.
    if (studentId.HasValue)
    {
        var validParentTeacherRelationship =
            await _context.TeachingSchedules
                .AnyAsync(x =>
                    x.IsActive &&
                    !x.IsDeleted &&

                    x.Enrollment.Status ==
                        EnrollmentStatus.Active &&

                    x.Enrollment.StudentId ==
                        studentId.Value &&

                    x.Teacher.ApplicationUserId ==
                        receiverUserId &&

                    x.Enrollment.Student.StudentParents
                        .Any(sp =>
                            sp.Parent.ApplicationUserId ==
                                senderUserId &&
                            !sp.IsDeleted) &&

                    !x.Teacher.IsDeleted);

        if (!validParentTeacherRelationship)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to message this teacher for this student.");
        }
    }

    // Find an existing open conversation.
    //
    // When StudentId is supplied, the student must
    // also match. This prevents two children of the
    // same parent/teacher from sharing one conversation.
    var conversation =
        await _context.Conversations
            .Where(c =>
                !c.IsClosed &&

                c.StudentId == studentId &&

                c.Messages.Any(m =>
                    (m.SenderUserId == senderUserId &&
                     m.ReceiverUserId == receiverUserId)
                    ||
                    (m.SenderUserId == receiverUserId &&
                     m.ReceiverUserId == senderUserId)
                ))
            .OrderByDescending(c =>
                c.LastMessageAt)
            .FirstOrDefaultAsync();

    // If no conversation exists, create one.
    if (conversation == null)
    {
        conversation = new Conversation
        {
            StudentId = studentId,

            LastMessageAt =
                DateTime.UtcNow,

            IsClosed = false
        };

        _context.Conversations.Add(conversation);
    }

    // Add message.
    var message = new Message
    {
        Conversation = conversation,

        SenderUserId =
            senderUserId,

        ReceiverUserId =
            receiverUserId,

        MessageText =
            messageText,

        SentAt =
            DateTime.UtcNow,

        IsRead = false
    };

    _context.Messages.Add(message);

    conversation.LastMessageAt =
        DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return conversation.Id;
}


    public async Task<List<ConversationDto>> GetAllConversationsAsync(
    Guid userId)
{
    return await _context.Conversations
        .Where(x =>
    x.Messages.Any(m =>
        m.SenderUserId == userId ||
        m.ReceiverUserId == userId))
        .OrderByDescending(x => x.LastMessageAt)
        .Select(x => new ConversationDto
        {
            Id = x.Id,

            StudentId = x.StudentId,

            LastMessage =
                x.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.MessageText)
                    .FirstOrDefault()
                    ?? "",

            LastMessageAt =
                x.LastMessageAt,

            OtherUserId =
                x.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m =>
                        m.SenderUserId == userId
                            ? m.ReceiverUserId
                            : m.SenderUserId)
                    .FirstOrDefault(),

            OtherUserName =
                x.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m =>
                        m.SenderUserId == userId
                            ? m.Receiver.FullName
                            : m.Sender.FullName)
                    .FirstOrDefault()
                    ?? ""
        })
        .ToListAsync();
}

public async Task<List<AdminConversationDto>>
    GetAdminMonitoredConversationsAsync()
{
    return await _context.Conversations
        .Where(c =>
            c.StudentId.HasValue &&
            c.Messages.Any())
        .OrderByDescending(c => c.LastMessageAt)
        .Select(c => new AdminConversationDto
        {
            ConversationId = c.Id,

            StudentId = c.StudentId,

            StudentName =
                _context.Students
                    .Where(s =>
                        s.Id == c.StudentId.Value)
                    .Select(s => s.FullName)
                    .FirstOrDefault()
                    ?? "",

            ParentId =
                _context.StudentParents
                    .Where(sp =>
                        sp.StudentId == c.StudentId.Value &&
                        !sp.IsDeleted)
                    .Select(sp => (Guid?)sp.ParentId)
                    .FirstOrDefault(),

            ParentName =
                _context.StudentParents
                    .Where(sp =>
                        sp.StudentId == c.StudentId.Value &&
                        !sp.IsDeleted)
                    .Select(sp => sp.Parent.FullName)
                    .FirstOrDefault()
                    ?? "",

            TeacherId =
                _context.TeachingSchedules
                    .Where(ts =>
                        ts.Enrollment.StudentId ==
                            c.StudentId.Value &&
                        ts.IsActive &&
                        !ts.IsDeleted &&
                        !ts.Teacher.IsDeleted)
                    .Select(ts => (Guid?)ts.TeacherId)
                    .FirstOrDefault(),

            TeacherName =
                _context.TeachingSchedules
                    .Where(ts =>
                        ts.Enrollment.StudentId ==
                            c.StudentId.Value &&
                        ts.IsActive &&
                        !ts.IsDeleted &&
                        !ts.Teacher.IsDeleted)
                    .Select(ts => ts.Teacher.FullName)
                    .FirstOrDefault()
                    ?? "",

            LastMessage =
                c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.MessageText)
                    .FirstOrDefault()
                    ?? "",

            LastMessageAt = c.LastMessageAt,

            HasUnreadMessages =
                c.Messages.Any(m =>
                    !m.IsRead)
        })
        .ToListAsync();
}

public async Task<List<MessageDto>>
    GetAdminMonitoredMessagesAsync(Guid conversationId)
{
    var conversationExists =
        await _context.Conversations
            .AnyAsync(c =>
                c.Id == conversationId &&
                c.StudentId.HasValue &&
                c.Messages.Any());

    if (!conversationExists)
    {
        throw new KeyNotFoundException(
            "Monitored conversation was not found.");
    }

    return await _context.Messages
        .Where(m =>
            m.ConversationId == conversationId)
        .OrderBy(m => m.SentAt)
        .Select(m => new MessageDto
        {
            Id = m.Id,

            SenderName =
                m.Sender.FullName,

            MessageText =
                m.MessageText,

            SentAt =
                m.SentAt,

            IsMine = false
        })
        .ToListAsync();
}

public async Task<int> GetAdminUnreadMessageCountAsync(Guid userId)
{
    return await _context.Messages
        .CountAsync(m =>
            m.ReceiverUserId == userId &&
            !m.IsRead);
}

}