using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button submitButton;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnChangeInputText(string s = null)
    {
        if (string.IsNullOrWhiteSpace(usernameInput.text) || string.IsNullOrWhiteSpace(passwordInput.text))
        {
            return;
        }

        submitButton.interactable = true;
        submitButton.gameObject.SetActive(true);

    }

    public void OnSubmitUsernameAndPassword()
    {
        // TODO
    }

}
