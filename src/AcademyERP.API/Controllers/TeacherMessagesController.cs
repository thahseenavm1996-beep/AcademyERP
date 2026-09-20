using AcademyERP.Application.DTOs.Communication;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AcademyERP.Application.Constants;

namespace AcademyERP.API.Controllers;


[ApiController]
[Route("api/teacher/messages")]
[Authorize(Roles = Roles.Teacher)]
public class TeacherMessagesController : ControllerBase
{

    private readonly ICommunicationService _service;


    public TeacherMessagesController(
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

[HttpGet("recipients")]
public async Task<IActionResult> GetRecipients()
{
    var userId = Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    return Ok(
        await _service.GetTeacherMessageRecipientsAsync(userId));
}

[HttpPost("start")]
public async Task<IActionResult> StartConversation(
    StartTeacherConversationRequest request)
{
    var userId = Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    try
    {
        var conversationId =
            await _service.StartTeacherConversationAsync(
                userId,
                request.ReceiverUserId,
                request.MessageText,
                request.StudentId);

        return Ok(conversationId);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Forbid();
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
}


}