using System;
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

        public UnityEvent OnStartRound;
        public UnityEvent OnEndRound;

        public UnityEvent OnUI; //whenever game enters any UI (excluding pause)
    }

    public enum RoundCompletionType
    {
        Completed,
        Failed,
        Quit
    }
    
    public static GameLoopManager Instance;
    [Header("Combat Map")]
    [Space (20)]
    public bool developerMode = false;

    [Header("Screens/Windows")]
    public Animator roundCompleteWindow;
    public GameObject loadingScreen;

    [Header("Runtime Data")]
    public RoundCompletionType roundCompletionType;

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
        
    }


    #region MAIN_MENU

    public void StartRound()
    {
        GameLoopEvents.OnStartRound?.Invoke();
    }


    public void EndRound() => StartCoroutine(IEndRound());   

    public IEnumerator IEndRound()
    {
        yield return new WaitForSeconds(0.5f);
        GameLoopEvents.OnEndRound?.Invoke();

        roundCompleteWindow.gameObject.SetActive(true);
        roundCompleteWindow.SetTrigger("Entry");
        yield return new WaitForSeconds(3f);
        roundCompleteWindow.SetTrigger("Exit");
        yield return new WaitForSeconds(1.5f);
        roundCompleteWindow.gameObject.SetActive(false);

        CompleteRound();
    }

    public void CompleteRound()
    {
        GameLoopEvents.OnUI?.Invoke();

        switch (roundCompletionType)
        {
            case RoundCompletionType.Completed:
                // Calculate Stats()
                // show stats + rewards() happily
                // Continue to Shop menu()
                // Show options to continue to next round, or quit to main menu
                break;

            case RoundCompletionType.Failed:
                // Calculate Stats()
                // show stats + rewards() sadly
                // Continue to Shop menu()
                // Show options to continue to next round, or quit to main menu
                break;

            case RoundCompletionType.Quit:
                // Show failed stats (that can be shown so far)
                // Show option to retry or quit to main menu
                break;
        }
        
        RestartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame() => StartCoroutine(IRestartGame());    

    public IEnumerator IRestartGame()
    {
        loadingScreen.SetActive(true);
        yield return null;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameLoopEvents.OnBootupGame?.Invoke();
        yield return new WaitForSeconds(1.5f);
        loadingScreen.SetActive(false);
    }


    #endregion

    private IEnumerator DelayedAction(float s, Action a)
    {
        yield return new WaitForSeconds(s);
        a?.Invoke();
    }

}
