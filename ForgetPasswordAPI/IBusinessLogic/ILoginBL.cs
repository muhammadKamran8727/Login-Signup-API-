using ForgetPasswordAPI.Models;

namespace ForgetPasswordAPI.IBusinessLogic
{
    public interface ILoginBL
    {
        Response SignUp(User user);
        Response Login(User user);
        Response ForgetPassword(string email);
        Response ResetPassword(string email, string enteredPin, string newPassword);
    }
}
