using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.DTOs.Communication;
using System.Security.Claims;
using AcademyERP.Application.Constants;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/admin/messages")]
[Authorize(Roles = Roles.Administrator + "," + Roles.SuperAdmin)]
public class AdminMessagesController : ControllerBase
{
    private readonly ICommunicationService _service;

    public AdminMessagesController(
        ICommunicationService service)
    {
        _service = service;
    }

    [HttpGet("all")]
public async Task<IActionResult> GetAllConversations()
{
    var userId = Guid.Parse(
        User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    var result =
        await _service.GetAllConversationsAsync(userId);

    return Ok(result);
}

    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeachers()
    {
        return Ok(
            await _service.GetTeacherRecipientsAsync());
    }

  [HttpGet("{conversationId}")]
public async Task<IActionResult> GetMessages(
    Guid conversationId)
{
    var userId = Guid.Parse(
        User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    return Ok(
        await _service.GetAdminMessagesAsync(
            conversationId,
            userId));
}

    [HttpPost("start")]
    public async Task<IActionResult> StartConversation(
        [FromBody] StartConversationRequest request)
    {
        var userId =
            Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

        var conversationId =
            await _service.StartConversationAsync(
                userId,
                request.ReceiverUserId,
                request.MessageText);

        return Ok(conversationId);
    }

    [HttpPost]
    public async Task<IActionResult> Send(
        SendMessageRequest request)
    {
        var userId =
            Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

        await _service.SendMessageAsync(
            userId,
            request);

        return Ok();
    }

    [HttpGet("monitor")]
public async Task<IActionResult> GetMonitoredConversations()
{
    return Ok(
        await _service.GetAdminMonitoredConversationsAsync());
}

[HttpGet("monitor/{conversationId:guid}")]
public async Task<IActionResult> GetMonitoredMessages(
    Guid conversationId)
{
    return Ok(
        await _service.GetAdminMonitoredMessagesAsync(
            conversationId));
}

[HttpGet("unread-count")]
public async Task<IActionResult> GetUnreadCount()
{
    var userId = Guid.Parse(
        User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    var count =
        await _service.GetAdminUnreadMessageCountAsync(userId);

    return Ok(count);
}

}