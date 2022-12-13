using Paraverse.Mob;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
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
        public TextMeshProUGUI goldText;
    }

    public enum RoundCompletionType
    {
        Completed,
        Failed,
        BossDefeated,
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
    public Animator roundFailedWindow;
    public Animator bossDefeatedWindow;
    public GameObject loadingScreen;
    public GameObject roundResultsWindow;
    public RoundTimer roundTimer;
    public PauseMenuViewController pauseMenu;

    [Header("End Portal")]
    public GameObject EndPortal;

    [Header("Predicate")]
    public CompletionPredicateType CompletionPredicate;
    private Predicate<bool> _predicate;

    [Header("References")]
    public ResultsScreen Results;

    [Space(20)]
    [Header("Runtime Data")]
    [Min(1)]
    public int nextRoundNumber;
    public RoundCompletionType roundCompletionType;
    private GameObject player;
    MobStats playerStats;
    PlayerController playerController;
    public int damageTaken;
    private int totalEnemiesSpawned;
    private int lastHealthSaved = -1;
    private int playerMaxHealth;
    private int goldRewarded = 0;

    private bool _roundReady = false;
    private bool _isBossRound = false;
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
        player = GlobalSettings.Instance.player;
        playerStats = player.GetComponentInChildren<MobStats>();
        playerController = player.GetComponentInChildren<PlayerController>();

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

        if (Input.GetKeyDown(KeyCode.U)) EndPortal.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Y)) playerStats.SetFullHealth();

        if (player.transform.position.y <= -25f)
        {
            playerStats.UpdateCurrentHealth((int)(-playerStats.MaxHealth * GlobalValues.FallDamage / 100.0f));
            UtilityFunctions.TeleportObject(player, MapGeneration.Instance.GetClosestBlock(player.transform).transform.position + new Vector3(0, 0.5f, 0));
        }
    }


    #region MAIN_MENU

    public void InitiateRound()
    {
        GameLoopEvents.OnInitiateRound?.Invoke();

        ResetStates();
    }

    public void StartRound()
    {
        GameLoopEvents.OnStartRound?.Invoke();

        totalEnemiesSpawned = EnemiesManager.Instance.EnemiesCount;
        playerMaxHealth = playerStats.MaxHealth;

        playerStats.OnHealthChange.AddListener(AccrueDamageTaken);
        playerController.OnDeathEvent += EndRoundPremature;

        _roundReady = true;
    }

    public void ResetStates()
    {
        damageTaken = 0;
        totalEnemiesSpawned = 0;
        lastHealthSaved = -1;
        playerMaxHealth = 0;
        goldRewarded = 0;
        _isBossRound = false;
        _roundReady = false;
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

    private void EndRoundPremature(Transform t)
    {
        roundCompletionType = RoundCompletionType.Failed;
        EndRound(successfulRound: false);
    }

    public void EndRound(bool successfulRound) => StartCoroutine(IEndRound(successfulRound));

    public IEnumerator IEndRound(bool successfulRound)
    {
        _roundReady = false;
        roundTimer.PauseTimer();

        // DETERMINE HOW ROUND ENDED (SUCCESSFUL OR FAILED OR BOSS DEFEATED?)
        if (successfulRound)
        {
            if (!_isBossRound) roundCompletionType = RoundCompletionType.Completed;
            else roundCompletionType = RoundCompletionType.BossDefeated;
        }
        else
        {
            roundCompletionType = RoundCompletionType.Failed;
        }

        // here ...


        Animator roundEndWindow = new Animator();
        switch (roundCompletionType)
        {
            case RoundCompletionType.Completed:
                roundEndWindow = roundCompleteWindow;
                break;
            case RoundCompletionType.Failed:
                roundEndWindow = roundFailedWindow;
                break;
            case RoundCompletionType.BossDefeated:
                roundEndWindow = bossDefeatedWindow;
                break;
        }

        GameLoopEvents.OnEndRound?.Invoke();
        roundEndWindow.gameObject.SetActive(true);
        roundEndWindow.SetTrigger("Entry");
        Time.timeScale = 0.4f;
        yield return new WaitForSecondsRealtime(3f);
        roundEndWindow.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(1.5f);
        roundEndWindow.gameObject.SetActive(false);
        Time.timeScale = 1f;
        CompleteRound();
    }

    public void CompleteRound()
    {
        playerStats.OnHealthChange.RemoveListener(AccrueDamageTaken);
        playerController.OnDeathEvent -= EndRoundPremature;
        EnemiesManager.Instance.ResetEnemiesList();
        Destroy(EndPortal);

        float score = ScoreFormula.CalculateScore(totalEnemiesSpawned * 10f, roundTimer.GetTime(), playerMaxHealth, damageTaken);
        goldRewarded = (int)(score * 1);

        playerStats.UpdateGold(goldRewarded); // save it to db

        Results.scoreText.text = "Score: " + (int)score + "%";
        Results.rankText.text = ScoreFormula.GetScoreRank((int)score);

        nextRoundNumber++;

        //Results.goldText.text = "+" + goldRewarded;

        // save it in database here, we need to save stats in db asap so players
        // who might d/c right after ending get their stuff saved

        ResetStates();
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
        int enemiesLeft = EnemiesManager.Instance.EnemiesCount;
        //Debug.Log($"CURRENT PREDICATE: [KILL ALL ENEMIES] STATUS:  mapReady: {mapReady}  -  enemies left: {enemiesLeft}");
        return mapReady && enemiesLeft <= 0;
    }

    // Implement Get All Gems

    // Implement survive X minutes


    /* * * * * * *  P R E D I C A T E S  * * * * * * * * */


}
