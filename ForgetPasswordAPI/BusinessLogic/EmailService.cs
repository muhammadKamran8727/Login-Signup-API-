
using MailKit.Net.Smtp;
using MimeKit;

namespace ForgetPasswordAPI.BusinessLogic
{
    public class EmailService
    {
        public async Task<bool> SendEmailAsync(string toEmail, string pinCode)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Muhammad Kamran", "mkamran8727419@gmail.com"));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "Password Reset PIN Code";

                message.Body = new TextPart("plain")
                {
                    Text = $"Your PIN code to reset your password is: {pinCode}"
                };

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("mkamran8727419@gmail.com", "lobc icjm kijf hzdo");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                // Handle exception (log, return false, etc.)
                Console.WriteLine(ex.Message);
                return false; // Failed to send email
            }
        }
    }
}

