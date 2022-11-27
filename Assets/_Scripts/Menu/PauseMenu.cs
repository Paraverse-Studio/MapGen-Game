using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public float pausingSpeed;
    public ContextMessageHandler contextMenu;

    public UnityEvent OnQuitToMainMenu = new UnityEvent();
    public UnityEvent OnPause = new UnityEvent();
    public UnityEvent OffPause = new UnityEvent();
    public BoolEvent OnPauseBool = new BoolEvent();

    private RectTransform _rectTransform;
    private bool _isPaused;

    public bool Paused
    {
        get { return _isPaused; }
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _isPaused = false;
    }

    private float GetTimeDelta()
    {
        return Time.unscaledDeltaTime * pausingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPaused)
        {
            if (Time.timeScale != 0f) Time.timeScale = 0;

            _rectTransform.offsetMin = new Vector2(Mathf.Max(_rectTransform.offsetMin.x - GetTimeDelta()*1000, 0), 0);
        }
        else if (!_isPaused)
        {
            if (Time.timeScale != 1f) Time.timeScale = 1f;

            _rectTransform.offsetMin = new Vector2(Mathf.Min(_rectTransform.offsetMin.x + GetTimeDelta()*2000, 3500), 0);

        }
    }

    public void TogglePause()
    {
        SetPause(!_isPaused);
    }

    public void SetPause(bool o)
    {
        _isPaused = o;
        
        if (o) OnPause?.Invoke();
        if (!o) OffPause?.Invoke();
        OnPauseBool?.Invoke(o);        
    }

    public void QuitToMainMenu()
    {
        SetPause(false);
        OnQuitToMainMenu?.Invoke();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
