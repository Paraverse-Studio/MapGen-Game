public class RegistrationValidationModel
{
  public string Username { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
  public string ConfirmPassword { get; set; }

  public RegistrationValidationModel(string username, string email, string password, string confirmPassword)
  {
    Username = username;
    Email = email;
    Password = password;
    ConfirmPassword = confirmPassword;
  }
}