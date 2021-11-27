using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    public bool playOnStart = false;
    public MenuScreen previousMenu;
    public MenuScreen nextMenu;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        // have a Fader script on all menus, and this calls that
        //if (_fader) _fader.FadeIn = true;
        //_cs.alpha = 1f;
        gameObject.SetActive(true);

    }

    public void End()
    {
        //if (nextMenu)
        //{
        //    _fader.FadeOut = true; // use Fader here?
        //    nextMenu.Show();
        //}

        //_cs.alpha = 0f;
        gameObject.SetActive(false);

    }
}
