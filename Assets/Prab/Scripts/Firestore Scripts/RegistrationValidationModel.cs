using UnityEngine;

public class RegistrationValidationModel
{
  public string Username { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
  public string ConfirmPassword { get; set; }

  public RegistrationValidationModel(string username, string email, string password, string confirmPassword)
  {
    Debug.Log($"username {username}");
    Debug.Log($"username {username.ToLower()}");
    Username = username.ToLower();
    Debug.Log($"username {Username}");
    Email = email;
    Password = password;
    ConfirmPassword = confirmPassword;
  }
}