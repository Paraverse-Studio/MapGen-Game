using Firebase;
using Firebase.Auth;
using ParaverseWebsite.Models;
using System;
using System.Collections;
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

  // Firebase variables
  [Header("Firebase")]
  public DependencyStatus dependencyStatus;
  public FirebaseAuth auth;
  public FirebaseUser user;

  // Login variables
  [Space]
  [Header("Login")]
  public TMP_InputField usernameInput;
  public TMP_InputField passwordInput;

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
    //AuthStateChanged(this, null);
  }

  private void AuthStateChanged(object sender, System.EventArgs eventArgs)
  {
    if (auth.CurrentUser != user)
    {
      bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

      if (!signedIn && user != null)
      {
        Debug.Log("Signed out " + user.UserId);
        OpenLoginLayout();
      }

      user = auth.CurrentUser;

      if (signedIn)
      {
        Debug.Log("Signed in 0" + user.UserId);
        OpenHomeLayout();
        Debug.Log("Signed in 1" + user.UserId);
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
    usernameInput.text = "";
    passwordInput.text = "";
  }

  public void Login()
  {
    StartCoroutine(LoginAsync(usernameInput.text, passwordInput.text));
  }

  public void Register()
  {
    StartCoroutine(RegisterAsync(usernameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
  }

  public void Logout()
  {
    if (auth  != null && user != null)
    {
      auth.SignOut();
    }
  }

  private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
  {
    if (name == "")
    {
      Debug.LogError("User Name is empty");
    }
    else if (email == "")
    {
      Debug.LogError("Email field is empty");
    }
    else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
    {
      Debug.LogError("Password does not match");
    }
    else
    {
      var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

      yield return new WaitUntil(() => registerTask.IsCompleted);

      if (registerTask.Exception != null)
      {
        Debug.LogError(registerTask.Exception);

        FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
        AuthError authError = (AuthError)firebaseException.ErrorCode;

        string failedMessage = "Registration Failed! Because ";
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

        Debug.Log(failedMessage);
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

          string failedMessage = "Profile update Failed! Because ";
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

          Debug.Log(failedMessage);
          RegisterFeedback.text = failedMessage;
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
    }
  }

  private IEnumerator LoginAsync(string username, string password)
  {
    var loginTask = auth.SignInWithEmailAndPasswordAsync(username, password);

    yield return new WaitUntil(() => loginTask.IsCompleted);

    if (loginTask.Exception != null)
    {
      Debug.LogError(loginTask.Exception);

      FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
      AuthError authError = (AuthError)firebaseException.ErrorCode;

      string failedMessage = "Login Failed! Because ";

      switch (authError)
      {
        case AuthError.InvalidEmail:
          failedMessage += "Email is Invalid";
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
          failedMessage = "Login Failed";
          break;
      }

      Debug.Log(failedMessage);
      LoginFeedback.text = failedMessage;
    }
    else
    {
      user = loginTask.Result.User;

      Debug.LogFormat("{0} You are successfully logged in", user.DisplayName);
      OpenHomeLayout();
      LoginFeedback.text = $"{user.DisplayName} You are successfully logged in";
    }
  }

  public void OnChangeInputText(string s = null)
  {
    if (string.IsNullOrWhiteSpace(usernameInput.text))
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
    LoginLayout.SetActive(true);
    ClearLoginInputFieldText();
  }

  public void OpenRegistrationLayout()
  {
    CloseAll();
    RegistrationLayout.SetActive(true);
    Debug.Log("Open Registration");
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
  }
}
