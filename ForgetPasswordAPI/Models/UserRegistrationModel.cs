﻿namespace ForgetPasswordAPI.Models
{
    public class UserRegistrationModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

}
