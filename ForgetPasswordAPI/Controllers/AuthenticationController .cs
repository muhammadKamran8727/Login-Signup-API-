// AuthenticationController.cs
using ForgetPasswordAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using static ForgetPasswordAPI.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AutoWrapper.Wrappers;
using ForgetPasswordAPI.IBusinessLogic;

namespace ForgetPasswordAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {


        private readonly ILoginBL _iloginBL;
        public AuthenticationController(ILoginBL iloginBL)
        {

            _iloginBL = iloginBL;
        }

        [HttpPost("signup")]
        public ApiResponse SignUp(User user)
        {
            try
            {
                var response = _iloginBL.SignUp(user);
                if (response != null && response.IsError == false)
                    return new ApiResponse { StatusCode = 200, Message = response.Message };

                return new ApiResponse { StatusCode = 500, Message = response.Message, Result = response.Result };
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = 401, Message = ex.Message };
            }

        }

        [HttpPost("login")]
        public ApiResponse Login(User user)
        {
            try
            {
                var response = _iloginBL.Login(user);
                if (response != null && response.IsError == false)
                    return new ApiResponse { StatusCode = 200, Message = response.Message, Result = response.Result };

                return new ApiResponse { StatusCode = 500, Message = response.Message, Result = response.Result };
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = 401, Message = ex.Message };
            }
        }

        [HttpPost("forgetpassword")]
        public ApiResponse ForgetPassword(string email)
        {
            try
            {
                var response = _iloginBL.ForgetPassword(email);
                if (response != null && response.IsError == false)
                    return new ApiResponse { StatusCode = 200, Message = response.Message, Result = response.Result };

                return new ApiResponse { StatusCode = 500, Message = response.Message, Result = response.Result };
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = 401, Message = ex.Message };
            }
        }



        [HttpPost("resetpassword")]
        public ApiResponse ResetPassword(string email, string enteredPin, string newPassword)
        {
            try
            {
                var response = _iloginBL.ResetPassword(email, enteredPin, newPassword);
                if (response != null && response.IsError == false)
                    return new ApiResponse { StatusCode = 200, Message = response.Message };

                return new ApiResponse { StatusCode = 500, Message = response.Message, Result = response.Result };
            }
            catch (Exception ex)
            {
                return new ApiResponse { StatusCode = 401, Message = ex.Message };
            }
        }
    }
}
