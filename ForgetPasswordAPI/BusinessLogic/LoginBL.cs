using ForgetPasswordAPI.IBusinessLogic;
using ForgetPasswordAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace ForgetPasswordAPI.BusinessLogic
{
    public class LoginBL : ILoginBL
    {
        private readonly UserContext _dbContext;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        public LoginBL(UserContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _tokenHandler = new JwtSecurityTokenHandler();
            _configuration = configuration;


        }

        public Response SignUp(User user)
        {
            if (_dbContext.Users.Any(u => u.Email == user.Email))
            {
                return new Response { Message = "Email already exists" };
            }

            // Hash the password before saving to the database
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return new Response { Message = "User registered successfully" };
        }



        public Response Login(User user)
        {
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            {
                return new Response { Message = "Invalid credentials" };
            }

            // Generate a secure key for HMAC-SHA256
            var key = GenerateSecureKey();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, existingUser.Id.ToString()) // You can add more claims as needed
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Set expiration time as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = _tokenHandler.WriteToken(token);

            // Return the token along with a success message
            return new Response { Message = "Login successful", Result = existingUser };
        }

        private byte[] GenerateSecureKey()
        {
            using (var hmac = new HMACSHA256())
            {
                return hmac.Key;
            }
        }


        //        {
        //  "id": 0,
        //  "email": "mkamran8727419@gmail.com",
        //  "password": "Kamran8727^^"
        //}

        //public Response ForgetPassword(string email)
        //{
        //    var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

        //    if (user == null)
        //    {
        //        return new Response { Message = "User not found" };
        //    }


        //    string resetToken = Guid.NewGuid().ToString();




        //    string hashedResetToken = BCrypt.Net.BCrypt.HashPassword(resetToken);
        //    user.Password = hashedResetToken;
        //    _dbContext.Users.Update(user);
        //    _dbContext.SaveChanges();

        //    SendPasswordResetEmailAsync(user.Email, resetToken);
        //    return new Response { Message = "Password reset token sent to the email", Result = resetToken };
        //}

        //public async Task SendPasswordResetEmailAsync(string userEmail, string resetToken)
        //{


        //    string smtpServer = _configuration["EmailSettings:SmtpServer"];
        //    int port = int.Parse(_configuration["EmailSettings:Port"]);
        //    string userName = _configuration["EmailSettings:UserName"];
        //    string password = _configuration["EmailSettings:Password"];

        //    using (SmtpClient client = new SmtpClient(smtpServer, port))
        //    {
        //        client.UseDefaultCredentials = false;
        //        client.Credentials = new NetworkCredential(userName, password);
        //        client.EnableSsl = true;
        //        MailMessage mailMessage = new MailMessage();
        //        mailMessage.From = new MailAddress(userName);
        //        mailMessage.To.Add(userEmail);
        //        mailMessage.Subject = "Password Reset Token";
        //        mailMessage.Body = $"Click the following link to reset your password: https://yourapp.com/reset-password?token={resetToken}";
        //        mailMessage.IsBodyHtml = true;
        //        await client.SendMailAsync(mailMessage);




        //    }
        //}




        //public Response ResetPassword(string email, string resetToken, string newPassword)
        //{
        //    var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(resetToken, user.Password))
        //    {
        //        return new Response { Message = "Invalid reset token or user not found" };
        //    }

        //    // Hash the new password before updating
        //    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        //    _dbContext.Users.Update(user);
        //    _dbContext.SaveChanges();

        //    return new Response { Message = "Password reset successfully" };
        //}


        public Response ForgetPassword(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return new Response { Message = "User not found" };
            }

            // Generate a random PIN code
            Random rnd = new Random();
            string pinCode = rnd.Next(1000, 9999).ToString();

            // Store the PIN code in the database (or any other secure storage)
            user.ResetPin = BCrypt.Net.BCrypt.HashPassword(pinCode);
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            SendPinCodeResetAsync(user.Email, pinCode);
            return new Response { Message = "PIN code sent to the email", Result = pinCode };
        }

        public async Task SendPinCodeResetAsync(string userEmail, string pinCode)
        {
            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            int port = int.Parse(_configuration["EmailSettings:Port"]);
            string userName = _configuration["EmailSettings:UserName"];
            string password = _configuration["EmailSettings:Password"];

            using (SmtpClient client = new SmtpClient(smtpServer, port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(userName);
                mailMessage.To.Add(userEmail);
                mailMessage.Subject = "Password Reset PIN Code";
                mailMessage.Body = $"Your PIN code to reset your password is: {pinCode}";
                mailMessage.IsBodyHtml = true;
                await client.SendMailAsync(mailMessage);
            }
        }

        public Response ResetPassword(string email, string enteredPin, string newPassword)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(enteredPin, user.ResetPin))
            {
                return new Response { Message = "Invalid PIN code or user not found" };
            }

            // Hash the new password before updating
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetPin = null; // Clear the PIN code after successful reset
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return new Response { Message = "Password reset successfully" };
        }



    }
}

