using Firebase;
using Firebase.Auth;
using ParaverseWebsite.Models;
using System.Collections;
using System.Collections.Generic;
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
  public TMP_InputField usernameLoginField;
  public TMP_InputField passwordLoginField;

  // Registration variables
  [Space]
  [Header("Registration")]
  public TMP_InputField emailRegisterField;
  public TMP_InputField usernameRegisterField;
  public TMP_InputField passwordRegisterField;
  public TMP_InputField confirmPasswordRegisterField;

  public string Username => _username;
  private string _username;


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

  /// <summary>
  /// Initializes firebase auth connection 
  /// </summary>
  private void InitializeFirebase()
  {
    // Set the default instance object
    auth = FirebaseAuth.DefaultInstance;

    auth.StateChanged += AuthStateChanged;
  }

  private void AuthStateChanged(object sender, System.EventArgs eventArgs)
  {
    Debug.Log($"AuthStateChanged - auth: {auth}, CurrentUser: {auth.CurrentUser}, user: {user}");
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
        Debug.Log($"Signed In: {user.Email}");
        AutoLogin();
      }
    }
  }

  #region Auto Login Methods
  /// <summary>
  /// Checks if user is already logged in
  /// </summary>
  /// <returns></returns>
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

  /// <summary>
  /// Opens the corresponding (HomeLayout or LoginLayout) depending on if user is already logged in or not
  /// </summary>
  private void AutoLogin()
  {
    if (user != null)
    {
      FirebaseDatabaseManager.Instance.GetUsers(
              // SUCCESSFULLY RETREIVED USERS LIST
              (users) =>
              {
                Debug.Log($"Auto logged in as {user}... getting username...");
                foreach (KeyValuePair<string, UserModel> entry in users)
                {
                  if (entry.Value.Email == user.Email)
                  {
                    _username = entry.Value.Username;
                    Debug.Log($"Auto logged in as {user}. Username is {_username}");
                    break;
                  }
                }
                OpenHomeLayout();
              },
              // FAILURE TO RETREIVE USERS LIST
              () =>
              {
                Debug.Log($"Failed to auto login as {user}");
                Debug.Log($"Failed to auto login as {user.Email}");
                LoginFeedback.text = "Auto Login Failed! User does not exist!";
                OpenLoginLayout();
              });
    }
    else
    {
      OpenLoginLayout();
    }
  }
  #endregion

  #region Login Methods
  /// <summary>
  /// User login validation check
  /// </summary>
  /// <param name="model"></param>
  /// <param name="failedMessage"></param>
  /// <param name="outputMessage"></param>
  /// <returns></returns>
  private bool LoginValidationCheck(LoginValidationModel model, string failedMessage, out string outputMessage)
  {
    outputMessage = failedMessage;

    if (model.Username == "")
    {
      outputMessage += "Username field is empty";
      return false;
    }
    else if (model.Password == "")
    {
      outputMessage += "Password field is empty";
      return false;
    }
    return true;
  }

  /// <summary>
  /// OnClick method for Login button
  /// </summary>
  public void Login()
  {
    LoginValidationModel model = new LoginValidationModel(usernameLoginField.text, null, passwordLoginField.text);

    FirebaseDatabaseManager.Instance.GetUser(usernameLoginField.text,
          // SUCCESSFULLY RETRIEVED USER
          (user) => {
            _username = user.Username;
            model.Email = user.Email;
            Debug.Log($"User exists in database! {user.Username}");

            StartCoroutine(LoginAsync(model));
          },
          // FAILED TO RETRIEVE USER
          () =>
          {
            model.Email = usernameLoginField.text;

            FirebaseDatabaseManager.Instance.GetUsers(
              // SUCCESSFULLY RETREIVED USERS LIST
              (users) =>
              {
                Debug.Log("Failed to retrieve user via username. Trying with email...");
                StartCoroutine(LoginAsync(model));
                foreach (KeyValuePair<string, UserModel> entry in users)
                {
                  if (entry.Value.Email  == model.Email)
                  {
                    _username = entry.Value.Username;
                    break;
                  }
                }
              },
              // FAILURE TO RETREIVE USERS LIST
              () =>
              {
                Debug.Log($"Failed to retrieve user via username and not trying with email since users list does not exist!");
                LoginFeedback.text = "Login Failed! User does not exist!";
              });
          }
        );
  }

  /// <summary>
  /// Logs user in by checking Firebase Auth in database
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
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
            failedMessage += "Username does not exists in database";
            break;
          case AuthError.WrongPassword:
            failedMessage += "Password is incorrect";
            break;
          case AuthError.MissingEmail:
            failedMessage += "Username field is empty";
            break;
          case AuthError.MissingPassword:
            failedMessage += "Password field is empty";
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
        LoginFeedback.text = $"You are successfully logged in!";
      }
    }

    LoginFeedback.text = failedMessage;
  }

  /// <summary>
  /// Clears login input fields
  /// </summary>
  private void ClearLoginInputFieldText()
  {
    usernameLoginField.text = "";
    passwordLoginField.text = "";
  }
  #endregion

  #region Registration Methods

  /// <summary>
  /// User registration validation check
  /// </summary>
  /// <param name="model"></param>
  /// <param name="failedMessage"></param>
  /// <param name="outputMessage"></param>
  /// <returns></returns>
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
  /// <summary>
  /// OnClick method for register button
  /// </summary>
  public void Register()
  {
    RegistrationValidationModel model = new RegistrationValidationModel(usernameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text);

    FirebaseDatabaseManager.Instance.GetUser(usernameRegisterField.text,
          // USER EXISTS IN DATABASE, PREVENT REGISTRATION
          (user) => {
            Debug.Log($"An account with Username: {model.Username} already exists in the database");
            RegisterFeedback.text = $"An account with Username: {model.Username} already exists in the database";
          },
          // USER DOES NOT EXIST IN DATABASE, CONTINUE WITH REGISTRATION
          () =>
          {
            Debug.Log($"An account with Username: {model.Username} does not exist in the database. Continue with Registration!");
            Debug.Log($"An account with Email: {model.Email} does not exist in the database. Continue with Registration!");
            _username = model.Username;
            StartCoroutine(RegisterAsync(model));
          }
        );
  }

  /// <summary>
  /// Registers user and updates Firebase Auth in database
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
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
          case AuthError.EmailAlreadyInUse:
            failedMessage += "Email already exists";
            break;
          default:
            failedMessage += authError.ToString();
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

          UserModel userModel = new UserModel(user.UserId ,usernameRegisterField.text, emailRegisterField.text);

          FirebaseDatabaseManager.Instance.PostUser(userModel, (user) => {
            _username = user.Username;
            Debug.Log($"User model added to database! {user.Username}");
            Debug.Log($"User model added to database! {_username}");
            }
          );
        }
      }

      RegisterFeedback.text = failedMessage;
    }

    RegisterFeedback.text = failedMessage;
  }

  /// <summary>
  /// Clears registration input fields
  /// </summary>
  private void ClearRegistrationInputFieldText()
  {
    usernameRegisterField.text = "";
    emailRegisterField.text = "";
    passwordRegisterField.text = "";
    confirmPasswordRegisterField.text = "";
  }
  #endregion

  /// <summary>
  /// Logs user out
  /// </summary>
  public void Logout()
  {
    if (auth != null && user != null)
    {
      auth.SignOut();
    }
  }

  #region Layout Methods
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
  #endregion

  public void OnChangeInputText(string s = null)
  {
    if (string.IsNullOrWhiteSpace(usernameLoginField.text))
    {
      return;
    }

    submitButton.gameObject.SetActive(true);
  }

  public void OnClickWebsiteLink()
  {
    Application.OpenURL("https://paraverse-studio-dev.herokuapp.com/");
  }
}