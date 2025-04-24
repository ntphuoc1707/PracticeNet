using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DB.DTO.Chat;

[Authorize]
[Route("~/chat/[action]")]
[ApiController]
public class ChatController : Controller
{
    [HttpPost(Name = "StartConversation")]
    public ActionResult StartConversation(StartConversationRequestDTO startConversationRequest)
    {
        return Ok("Started conversation");
    }
}