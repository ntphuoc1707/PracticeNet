using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DB.DTO.Chat;
using ChatService.Interfaces;

[Authorize]
[Route("~/chat/[action]")]
[ApiController]
public class ChatController : Controller
{
    private IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [AllowAnonymous]
    [HttpPost(Name = "StartConversation")]
    public async Task<ActionResult> StartConversation(StartConversationRequestDTO startConversationRequest)
    {
        var result = await _chatService.StartConversation(startConversationRequest);
        return Ok(new {GroupId= result.ToString()});
    }
}