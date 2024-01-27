using UnityEngine;

public class LoginValidationModel
{
  public string Username { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }

  public LoginValidationModel(string username, string email, string password)
  {
    Username = username.ToLower();
    Email = email;
    Password = password;
  }
}
