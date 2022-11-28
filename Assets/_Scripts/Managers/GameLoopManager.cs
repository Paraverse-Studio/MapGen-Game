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

    [Header("Runtime Data")]
    public RoundCompletionType roundCompletionType;

    private Queue<Action> queue = new();

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
        yield return new WaitForSeconds(0.25f);
        GameLoopEvents.OnEndRound?.Invoke();

        roundCompleteWindow.gameObject.SetActive(true);
        roundCompleteWindow.SetTrigger("Entry");
        yield return new WaitForSeconds(3f);
        roundCompleteWindow.SetTrigger("Exit");
        yield return new WaitForSeconds(1.5f);
        roundCompleteWindow.gameObject.SetActive(false);

        CompleteRound();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region PLAY_SESSION

    // succeeded = round ended due to completion of round
    // succeeded false = user quit the round, no rewards
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

    #endregion

    private IEnumerator DelayedAction(float s, Action a)
    {
        yield return new WaitForSeconds(s);
        a?.Invoke();
    }

}
