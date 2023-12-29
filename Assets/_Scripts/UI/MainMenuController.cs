using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
  public static MainMenuController Instance;
  public string Username => _username;
  private string _username; 

  public TMP_InputField usernameInput;
  public TMP_InputField passwordInput;
  public Button submitButton;

  private void Awake()
  {
    if (Instance == null)
      Instance = this;
    else
      Destroy(Instance);
  }


  public void OnChangeInputText(string s = null)
  {
    if (string.IsNullOrWhiteSpace(usernameInput.text) || string.IsNullOrWhiteSpace(passwordInput.text))
    {
      return;
    }

    submitButton.gameObject.SetActive(true);

  }

  public void OnSubmitUsernameAndPassword()
  {
    _username = usernameInput.text;
  }

}
