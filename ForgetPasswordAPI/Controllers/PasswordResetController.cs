using ForgetPasswordAPI.BusinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForgetPasswordAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {

        private readonly EmailService _emailService;

        public PasswordResetController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-pin")]
        public IActionResult SendPin([FromBody] string email)
        {
            // Generate a random PIN code (you can implement your logic here)
            Random rnd = new Random();
            string pinCode = rnd.Next(1000, 9999).ToString();

            // Send the PIN code via email
            bool emailSent = _emailService.SendEmailAsync(email, pinCode).Result;

            if (emailSent)
            {
                return Ok("PIN code sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send PIN code");
            }
        }
    }
}