using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPI.Services;

namespace WhatsAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly WhatsAppService _whatsAppService;
        public WhatsAppController(WhatsAppService whatsAppService) 
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost("SendStaticTemplate")]
        public async Task<IActionResult> SendMessage([FromQuery] string phoneNumber, [FromQuery] string templateName)
        {
            var result = await _whatsAppService.StaticTemplateMessageService(phoneNumber, templateName);

            if (result)
            {
                return Ok("Message sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send message");
            }
        }


        [HttpPost("SendFeeTemplate")]
        public async Task<IActionResult> SendFeeTemplate([FromQuery] string phoneNumber, [FromQuery] string templateName, [FromQuery] string name, [FromQuery] string from)
        {
            var result = await _whatsAppService.SendFeeMessage(phoneNumber, templateName, name,from);

            if (result)
            {
                return Ok("Message sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send message");
            }
        }
    }
}
