using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [System.Serializable]
    public struct Events
    {
        public UnityEvent OnQuitToMainMenu;
        public UnityEvent OnPause;
        public UnityEvent OffPause;
        public BoolEvent OnPauseBool;
    }

    public static PauseMenu Instance;
    public float pausingAnimationSpeed;
    public RectTransform rectTransform;

    public Events events;

    private bool _isPaused;
    public bool Paused
    {
        get { return _isPaused; }
    }

    // this boolean is modified externally.
    // For situations where the game is in menu, and no pause needed
    // should only be pausable during round gameplay
    public bool Pausable = false;
    public void TogglePausable(bool o) => Pausable = o;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _isPaused = false;
    }

    private float GetTimeDelta()
    {
        return Time.unscaledDeltaTime * pausingAnimationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // it's here instead of Player Controller for ex. because player controlled might be disabled during UI, pauses, etc.
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

        if (_isPaused)
        {
            //if (Time.timeScale != 0f) Time.timeScale = 0;

            rectTransform.localScale = new Vector3(Mathf.Max(rectTransform.localScale.x - GetTimeDelta(), 1), 1, 1);
            rectTransform.gameObject.SetActive(true);
        }
        else if (!_isPaused)
        {
            //if (Time.timeScale != 1f) Time.timeScale = 1f;

            rectTransform.localScale = new Vector3(Mathf.Min(rectTransform.localScale.x + GetTimeDelta(), 3), 1, 1);
            if (rectTransform.localScale.x >= 3f) rectTransform.gameObject.SetActive(false);
        }
    }       

    public void TogglePause()
    {
        SetPause(!_isPaused);
    }

    public void SetPause(bool o)
    {
        if (!Pausable) return;

        _isPaused = o;
        
        if (o) events.OnPause?.Invoke();
        if (!o) events.OffPause?.Invoke();
        events.OnPauseBool?.Invoke(o);

        Time.timeScale = _isPaused? 0f : 1f;
    }

    public void QuitToMainMenu()
    {
        SetPause(false);
        events.OnQuitToMainMenu?.Invoke();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
