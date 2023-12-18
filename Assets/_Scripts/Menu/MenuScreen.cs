using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public bool playOnStart = false;
    public MenuScreen previousMenu;
    public MenuScreen nextMenu;
    public bool exempt = false; // exempt from force opening/closing

    [Header("Button to invoke when pressing 'ESC'")]
    public Button[] defaultButtons;

    private CanvasGroup _cs;
    public CanvasGroup CS
    {
        get { return _cs; }
        set { _cs = value; }
    }

    private Fader _fader;

    private void Awake()
    {
        _cs = GetComponent<CanvasGroup>();
        _fader = GetComponent<Fader>();
    }

    public void OnEnable()
    {
        UIManager.Instance.SelectMenu(this);
    }

    public void SelectDefaultButton()
    {
        if (defaultButtons.Length > 0)
        {
            for (int i = 0; i < defaultButtons.Length; ++i)
            {
                if (defaultButtons[i].gameObject.activeInHierarchy)
                {
                    Debug.Log("Successful!");
                    defaultButtons[i].onClick.Invoke();
                    break;
                }
            }
        }
    }

}
