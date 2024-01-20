using Firebase;
using Firebase.Auth;
using ParaverseWebsite.Models;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
  public static MainMenuController Instance;

  public GameObject HomeLayout;
  public GameObject LoginLayout;
  public TextMeshProUGUI LoginFeedback;
  public GameObject RegistrationLayout;
  public TextMeshProUGUI RegisterFeedback;
  public GameObject BloodLinesMenu;

  // Firebase variables
  [Header("Firebase")]
  public DependencyStatus dependencyStatus;
  public FirebaseAuth auth;
  public FirebaseUser user;

  // Login variables
  [Space]
  [Header("Login")]
  public TMP_InputField emailLoginField;
  public TMP_InputField passwordLoginField;

  // Registration variables
  [Space]
  [Header("Registration")]
  public TMP_InputField emailRegisterField;
  public TMP_InputField usernameRegisterField;
  public TMP_InputField passwordRegisterField;
  public TMP_InputField confirmPasswordRegisterField;

  public string Username { get; set; }
  public string Password { get; set; }

  public Button submitButton;


  private void Awake()
  {
    if (Instance == null)
      Instance = this;
    else
      Destroy(Instance);

    passwordLoginField.contentType = TMP_InputField.ContentType.Password;
    passwordRegisterField.contentType = TMP_InputField.ContentType.Password;
    confirmPasswordRegisterField.contentType = TMP_InputField.ContentType.Password;

    // Check that all the necessary dependencies for firebase are present on the system 
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    {
      dependencyStatus = task.Result;

      if (dependencyStatus.Equals(DependencyStatus.Available))
      {
        InitializeFirebase();
      }
      else
      {
        Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
      }
    });
  }

  private void Start()
  {
    OpenLoginLayout();
    CheckAndFixDependenciesAsync();
  }

  private IEnumerator CheckAndFixDependenciesAsync()
  {
    var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

    yield return new WaitUntil(() => dependencyTask.IsCompleted);

    dependencyStatus = dependencyTask.Result;

    if (dependencyStatus == DependencyStatus.Available)
    {
      InitializeFirebase();
      yield return new WaitForEndOfFrame();
      StartCoroutine(CheckForAutoLogin());
    }
    else
    {
      Debug.LogError("Could not resolve all firebase dependecies: " + dependencyStatus);
    }
  }

  private void InitializeFirebase()
  {
    // Set the default instance object
    auth = FirebaseAuth.DefaultInstance;

    auth.StateChanged += AuthStateChanged;
  }

  private void AuthStateChanged(object sender, System.EventArgs eventArgs)
  {
    if (auth.CurrentUser != user)
    {
      bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

      if (!signedIn && user != null)
      {
        OpenLoginLayout();
      }

      user = auth.CurrentUser;

      if (signedIn)
      {
        OpenHomeLayout();
      }
    }
  }

  private IEnumerator CheckForAutoLogin()
  {
    if (user != null)
    {
      var reloadUserTask = user.ReloadAsync();

      yield return new WaitUntil(() => reloadUserTask.IsCompleted);

      AutoLogin();
    }
    else
    {
      OpenLoginLayout();
    }
  }

  private void AutoLogin()
  {
    if (user != null)
    {
      OpenHomeLayout();
    }
    else
    {
      OpenLoginLayout();
    }
  }

  private void ClearLoginInputFieldText()
  {
    emailLoginField.text = "";
    passwordLoginField.text = "";
  }

  private void ClearRegistrationInputFieldText()
  {
    usernameRegisterField.text = "";
    emailRegisterField.text = "";
    passwordRegisterField.text = "";
    confirmPasswordRegisterField.text = "";
  }

  public void Login()
  {
    LoginValidationModel model = new LoginValidationModel(null, emailLoginField.text, passwordLoginField.text);

    StartCoroutine(LoginAsync(model));
  }

  public void Register()
  {
    RegistrationValidationModel model = new RegistrationValidationModel(usernameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text);

    StartCoroutine(RegisterAsync(model));
  }

  public void Logout()
  {
    if (auth != null && user != null)
    {
      auth.SignOut();
    }
  }

  private bool RegistrationValidationCheck(RegistrationValidationModel model, string failedMessage, out string outputMessage)
  {
    outputMessage = failedMessage;
    var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
    var regex = new Regex(pattern);

    if (model.Username == "")
    {
      outputMessage += "Username field is empty";
      return false;
    }
    else if (model.Username.Length < 3 || model.Username.Length > 14)
    {
      outputMessage += "Username must be between 3 to 14 characters long";
      return false;
    }
    else if (model.Username.Any(c => !char.IsLetterOrDigit(c)))
    {
      outputMessage += "Username cannot contain special characters";
      return false;
    }
    else if (model.Email == "")
    {
      outputMessage += "Email field is empty";
      return false;
    }
    else if (false == regex.IsMatch(model.Email))
    {
      outputMessage += "Email is invalid";
      return false;
    }
    else if (model.Password == "")
    {
      outputMessage += "Password field is empty";
      return false;
    }
    else if (model.ConfirmPassword == "")
    {
      outputMessage += "Confirm Password field is empty";
      return false;
    }
    else if (model.Password != model.ConfirmPassword)
    {
      outputMessage += "Password does not match";
      return false;
    }
    else if (model.Password.Length < 6)
    {
      outputMessage += "Password must be at least 6 characters long";
      return false;
    }
    return true;
  }

  private IEnumerator RegisterAsync(RegistrationValidationModel model)
  {
    string failedMessage = "";

    if (RegistrationValidationCheck(model, failedMessage, out failedMessage))
    {
      if (auth == null)
        InitializeFirebase();

      var registerTask = auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);

      yield return new WaitUntil(() => registerTask.IsCompleted);

      if (registerTask.Exception != null)
      {
        Debug.LogError(registerTask.Exception);

        FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
        AuthError authError = (AuthError)firebaseException.ErrorCode;

        failedMessage = "Registration Failed! ";
        switch (authError)
        {
          case AuthError.InvalidEmail:
            failedMessage += "Email is invalid";
            break;
          case AuthError.WrongPassword:
            failedMessage += "Wrong Password";
            break;
          case AuthError.MissingEmail:
            failedMessage += "Email is missing";
            break;
          case AuthError.MissingPassword:
            failedMessage += "Password is missing";
            break;
          default:
            failedMessage += "Registration Failed";
            break;
        }

        RegisterFeedback.text = failedMessage;
      }
      else
      {
        // Get the user after registration success
        user = registerTask.Result.User;

        UserProfile userProfile = new UserProfile { DisplayName = user.DisplayName };

        var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

        yield return new WaitUntil(() => updateProfileTask.IsCompleted);

        if (updateProfileTask.Exception != null)
        {
          // Delete the user if user update failed
          user.DeleteAsync();

          Debug.LogError(updateProfileTask.Exception);

          FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
          AuthError authError = (AuthError)firebaseException.ErrorCode;

          failedMessage = "Profile update Failed! ";
          switch (authError)
          {
            case AuthError.InvalidEmail:
              failedMessage += "Email is invalid";
              break;
            case AuthError.WrongPassword:
              failedMessage += "Wrong Password";
              break;
            case AuthError.MissingEmail:
              failedMessage += "Email is missing";
              break;
            case AuthError.MissingPassword:
              failedMessage += "Password is missing";
              break;
            default:
              failedMessage += "Registration Failed";
              break;
          }
        }
        else
        {
          Debug.Log("Registration Successful Welcome " + user.DisplayName);
          RegisterFeedback.text = $"Registration Successful Welcome {user.DisplayName}";
          OpenLoginLayout();

          UserModel userModel = new UserModel(usernameRegisterField.text, emailRegisterField.text, passwordRegisterField.text);

          FirebaseDatabaseManager.Instance.PostUser(userModel, (response) => Debug.Log($"User model added to database! {response.Username}"));
        }
      }

      RegisterFeedback.text = failedMessage;
    }

    RegisterFeedback.text = failedMessage;
  }

  private bool LoginValidationCheck(LoginValidationModel model, string failedMessage, out string outputMessage)
  {
    outputMessage = failedMessage;
    var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
    var regex = new Regex(pattern);

    if (model.Email == "")
    {
      outputMessage += "Email field is empty";
      return false;
    }
    else if (false == regex.IsMatch(model.Email))
    {
      outputMessage += "Email is invalid";
      return false;
    }
    else if (model.Password == "")
    {
      outputMessage += "Password field is empty";
      return false;
    }
    else if (model.Password.Length < 6)
    {
      outputMessage += "Password must be at least 6 characters long";
      return false;
    }
    return true;
  }

  private IEnumerator LoginAsync(LoginValidationModel model)
  {
    string failedMessage = "";
    Task<AuthResult> loginTask;

    if (LoginValidationCheck(model, failedMessage, out failedMessage))
    {
      if (auth == null)
        InitializeFirebase();

      loginTask = auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

      yield return new WaitUntil(() => loginTask.IsCompleted);

      if (loginTask.Exception != null)
      {
        FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
        AuthError authError = (AuthError)firebaseException.ErrorCode;

        failedMessage = "Login Failed! ";

        switch (authError)
        {
          case AuthError.InvalidEmail:
            failedMessage += "Email is invalid";
            break;
          case AuthError.WrongPassword:
            failedMessage += "Password is incorrect";
            break;
          case AuthError.MissingEmail:
            failedMessage += "Email is missing";
            break;
          case AuthError.MissingPassword:
            failedMessage += "Password is missing";
            break;
          case AuthError.UserNotFound:
            failedMessage += "User not found";
            break;
          case AuthError.AccountExistsWithDifferentCredentials:
            failedMessage += "Account exists with different credentials";
            break;
          case AuthError.TooManyRequests:
            failedMessage += "Too many requests. Please try again later";
            break;
          default:
            failedMessage = authError.ToString();
            break;
        }
      }
      else
      {
        user = loginTask.Result.User;

        OpenHomeLayout();
        LoginFeedback.text = $"You are successfully logged in";
      }

      LoginFeedback.text = failedMessage;
    }

    LoginFeedback.text = failedMessage;
  }

  public void OnChangeInputText(string s = null)
  {
    if (string.IsNullOrWhiteSpace(emailLoginField.text))
    {
      return;
    }

    submitButton.gameObject.SetActive(true);
  }

  public void OnClickWebsiteLink()
  {
    Application.OpenURL("https://paraverse-studio-dev.herokuapp.com/");
  }

  public void OpenLoginLayout()
  {
    CloseAll();
    ClearLoginInputFieldText();
    LoginLayout.SetActive(true);
  }

  public void OpenRegistrationLayout()
  {
    CloseAll();
    ClearRegistrationInputFieldText();
    RegistrationLayout.SetActive(true);
  }

  public void OpenHomeLayout()
  {
    CloseAll();
    HomeLayout.SetActive(true);
  }

  public void CloseAll()
  {
    LoginLayout.SetActive(false);
    RegistrationLayout.SetActive(false);
    HomeLayout.SetActive(false);
    LoginFeedback.text = "";
    RegisterFeedback.text = "";
    BloodLinesMenu.SetActive(false);
  }
}
