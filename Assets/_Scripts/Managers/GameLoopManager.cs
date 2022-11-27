using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    [System.Serializable]
    public struct GameEvents
    {
        public UnityEvent OnBootupGame;
        public UnityEvent OnDeveloperMode;
        public UnityEvent OnPlay;
        public BoolEvent OnPause;
    }
    
    public static GameLoopManager Instance;

    public bool developerMode = false;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    public GameEvents GameLoopEvents;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!developerMode) 
        {
            GameLoopEvents.OnBootupGame?.Invoke();
        }
        else
        {
            GameLoopEvents.OnDeveloperMode?.Invoke();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PauseGame();
        }
        
    }


    #region MAIN_MENU

    public void StartGame()
    {
        GameLoopEvents.OnPlay?.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused? 1f:0f;

        GameLoopEvents.OnPause?.Invoke(_isPaused);
    }
    #endregion

    #region PLAY_SESSION
    public void CompleteLevel()
    {
        // Calculate Stats()
        // Save/Store Stats in database ()
        // Display stats to player + score ()

        // Start next round:

        MapGeneration.Instance.RegenerateMap();
    }


    #endregion


}
