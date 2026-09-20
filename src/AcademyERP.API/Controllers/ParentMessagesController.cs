using AcademyERP.Application.DTOs.Communication;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AcademyERP.Application.Constants;

namespace AcademyERP.API.Controllers;


[ApiController]
[Authorize(Roles = Roles.Parent)]
[Route("api/parent/messages")]
public class ParentMessagesController : ControllerBase
{

    private readonly ICommunicationService _service;


    public ParentMessagesController(
        ICommunicationService service)
    {
        _service = service;
    }



    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId =
            Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);


        return Ok(
            await _service.GetMyConversationsAsync(userId));
    }


[HttpGet("recipients")]
public async Task<IActionResult> GetRecipients()
{
    var userId =
        Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

    return Ok(
        await _service.GetParentTeacherRecipientsAsync(
            userId));
}

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> GetMessages(
        Guid conversationId)
    {
        var userId =
            Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);


        return Ok(
            await _service.GetMessagesAsync(
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

    if (!request.StudentId.HasValue)
    {
        return BadRequest(
            "StudentId is required when a parent starts a conversation.");
    }

    var conversationId =
        await _service.StartConversationAsync(
            userId,
            request.ReceiverUserId,
            request.MessageText,
            request.StudentId);

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

}