using Paraverse.Mob.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        public UnityEvent OnInitiateRound;
        public UnityEvent OnStartRound;

        public UnityEvent OnEndRound;

        public UnityEvent OnUI; //whenever game enters any UI (excluding pause)
    }

    [System.Serializable]
    public struct ResultsScreen
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI rankText;
    }

    public enum RoundCompletionType
    {
        Completed,
        Failed,
        Quit
    }

    public enum CompletionPredicateType
    {
        KillAllEnemies,
        GetAllGems,
        SurviveXMinutes,
    }
    
    public static GameLoopManager Instance;
    [Header("Combat Map")]
    public bool developerMode = false;

    [Space(20)]
    [Header("Screens/Windows/Views")]
    public Animator roundCompleteWindow;
    public GameObject loadingScreen;
    public GameObject roundResultsWindow;
    public RoundTimer roundTimer;

    [Header("End Portal")]
    public GameObject EndPortal;

    [Header("Predicate")]
    public CompletionPredicateType CompletionPredicate;
    private Predicate<bool> _predicate;

    [Header("References")]
    public ResultsScreen Results;

    [Space(20)]
    [Header("Runtime Data")]
    public RoundCompletionType roundCompletionType;
    public int damageTaken;
    private int totalEnemiesSpawned;
    private int lastHealthSaved = -1;
    private int playerMaxHealth;

    private bool _roundReady = false;
    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    [Space(20)]
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
        if (Time.frameCount % 60 == 0)
        {
            if (null == _predicate) MakeCompletionPredicate(CompletionPredicate);
            if (_predicate(_roundReady)) EndPortal.SetActive(true);
        }
    }


    #region MAIN_MENU

    public void InitiateRound()
    {
        GameLoopEvents.OnInitiateRound?.Invoke();
    }

    public void StartRound()
    {
        _roundReady = true;

        GameLoopEvents.OnStartRound?.Invoke();
        totalEnemiesSpawned = EnemiesManager.Instance.EnemiesCount;
        GlobalSettings.Instance.player.GetComponentInChildren<MobStats>().OnHealthChange.AddListener(AccrueDamageTaken);
    }

    public void MakeCompletionPredicate(CompletionPredicateType predicate)
    {
        switch (CompletionPredicate)
        {
            case CompletionPredicateType.KillAllEnemies:
                _predicate = KillAllEnemies;
                break;

            case CompletionPredicateType.GetAllGems:
                // implement
                break;

            case CompletionPredicateType.SurviveXMinutes:
                //implement
                break;
        }
    }

    public void EndRound() => StartCoroutine(IEndRound());   

    public IEnumerator IEndRound()
    {
        _roundReady = false;

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
        GlobalSettings.Instance.player.GetComponentInChildren<MobStats>().OnHealthChange.RemoveListener(AccrueDamageTaken);
        
        float score = ScoreFormula.CalculateScore(totalEnemiesSpawned * 12f, roundTimer.GetTime(), playerMaxHealth, damageTaken);



        Results.scoreText.text = "Rank: " + (int)score;
        Results.rankText.text = GetScoreRank((int)score);



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

        roundResultsWindow.SetActive(true); // or play an animation
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

    public void AccrueDamageTaken(int playerCurrentHealth, int _playerMaxHealth)
    {
        playerMaxHealth = _playerMaxHealth;

        if (lastHealthSaved != -1)
        {
            damageTaken += Mathf.Max(lastHealthSaved - playerCurrentHealth, 0);
        }
        else
        {
            damageTaken += Mathf.Max(_playerMaxHealth - playerCurrentHealth, 0);
        }

        lastHealthSaved = playerCurrentHealth;
    }


    #endregion

    private IEnumerator DelayedAction(float s, Action a)
    {
        yield return new WaitForSeconds(s);
        a?.Invoke();
    }


    /* * * * * * *  P R E D I C A T E S  * * * * * * * * */
    public bool KillAllEnemies(bool mapReady)
    {

        return mapReady && EnemiesManager.Instance.EnemiesCount <= 0;
    }

    // Implement Get All Gems

    // Implement survive X minutes


    /* * * * * * *  P R E D I C A T E S  * * * * * * * * */

    public string GetScoreRank(int score)
    {
        if (score > 100) return "S+";
        else if (score >= 90) return "S";
        else if (score >= 80) return "A";
        else if (score >= 70) return "B";
        else if (score >= 60) return "C";
        else if (score >= 50) return "D";
        else return "F";
    }


}
