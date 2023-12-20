using Paraverse;
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

        public UnityEvent OnInitiateSession;

        public UnityEvent OnInitiateRound;
        public UnityEvent OnStartRound;

        public UnityEvent OnEndRound;

        public UnityEvent OnEndSession;

        public UnityEvent OnUI; //whenever game enters any UI (excluding pause)
    }

    [System.Serializable]
    public struct ResultsScreen
    {
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI timeTakenText;
        public TextMeshProUGUI damageTakenText;
        public TextMeshProUGUI totalScoreText;
        public TextMeshProUGUI goldEarnedText;
        public GameObject shopButton;
        public GameObject mainMenuButton;
    }

    [System.Serializable]
    public struct ShopScreen
    {
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI messageText;
    }

    public enum RoundCompletionType
    {
        Completed,
        Failed,
        BossDefeated,
        Reward,
        Quit
    }

    public enum CompletionPredicateType
    {
        EnjoyReward,
        KillAllEnemies,
        GetAllGems,
        SurviveXMinutes,
    }

    public static GameLoopManager Instance;
    [Header("Combat Map")]
    public bool developerMode = false;

    [Space(20)]
    [Header("Screens/Windows/Views")]
    public Animator roundStartWindow;
    public Animator roundCompleteWindow;
    public Animator roundFailedWindow;
    public Animator bossDefeatedWindow;
    public Animator startingHostileRoundWindow;
    public GameObject loadingScreen;
    public GameObject roundResultsWindow;
    public RoundTimer roundTimer;
    public PauseMenuViewController pauseMenu;

    [Header("End Portal")]
    public EndPointTrigger EndPortal;

    [Header("Predicate")]
    public CompletionPredicateType CompletionPredicate;
    private Predicate<bool> _predicate;

    [Header("References")]
    public ResultsScreen resultScreen;
    public ShopScreen shopScreen;

    [Space(20)]
    [Header("Runtime Data")]
    [Min(1)]
    public int nextRoundNumber;
    public int roundNumberInBiome;
    public RoundCompletionType roundCompletionType;
    private GameObject player;
    MobStats playerStats;
    PlayerController playerController;
    public int damageTaken;
    private int totalEnemiesSpawned;
    private int lastHealthSaved = -1;
    private int playerMaxHealth;
    private int goldToReward = 0;

    private bool _roundIsActive = false;
    
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
            if (null != _predicate && _predicate(_roundIsActive) && !EndPortal.IsActivated)
            {
                if (MapCreator.Instance.mapType != MapType.reward)
                    AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(1).AddText("Gate is open!"));
                EndPortal.Activate(true);
            }
        }        

        if (player.transform.position.y <= -20f && _roundIsActive)
        {
            UtilityFunctions.TeleportObject(player, MapGeneration.Instance.GetClosestBlock(player.transform).transform.position + new Vector3(0, 0.5f, 0));
            Invoke(nameof(PlayerFallDamage), 0.15f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) QualityManager.Instance.SetQualityLevel(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) QualityManager.Instance.SetQualityLevel(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) QualityManager.Instance.SetQualityLevel(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) QualityManager.Instance.SetQualityLevel(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) QualityManager.Instance.SetQualityLevel(5);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.U))
        {
            //if (EndPortal) EndPortal.Activate(true);                        
            //AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(0).AddTitle("hii").AddText("portal is open now LMAO :p"));
            //AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(1).AddTitle("hii").AddText("portal is open now LMAO :p"));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            List<MobController> m = EnemiesManager.Instance.Enemies;
            foreach (MobController _m in m)
            {
                _m.GetComponentInChildren<MobStats>().UpdateCurrentHealth(-_m.GetComponentInChildren<MobStats>().CurHealth);
            }
            EndRound(true);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerStats.UpdateGold(500); 
            ShopManager.Instance.ShopWindow.SetActive(!ShopManager.Instance.ShopWindow.activeSelf);
            CalculateShop();
        }
#endif

    }


    #region MAIN_MENU
    public void InitiateSession()
    {
        nextRoundNumber = 1;
        roundNumberInBiome = 1;
        GameLoopEvents.OnInitiateSession?.Invoke();
        InitiateRound();
    }

    public void InitiateRound()
    {      
        GameLoopEvents.OnInitiateRound?.Invoke();
    }

    public void StartRound()
    {
        ResetStates();
        GameLoopEvents.OnStartRound?.Invoke();

        totalEnemiesSpawned = EnemiesManager.Instance.EnemiesCount;
        playerMaxHealth = (int)playerStats.MaxHealth.FinalValue;

        GameplayListeners(attachOrRemove: true);

        if (nextRoundNumber == 1 && MapCreator.Instance.mapType != MapType.reward) // only for round 1, since it's tutorial
        {
            AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(1).StartDelay(1.5f).OverrideDuration(3f)
                .AddText("Defeat all enemies & pass through the gate!"));
        }

        MakeCompletionPredicate(CompletionPredicate);
        _roundIsActive = true;
    }

    public void ResetStates()
    {
        damageTaken = 0;
        totalEnemiesSpawned = 0;
        lastHealthSaved = -1;
        playerMaxHealth = 0;
        goldToReward = 0;
        _roundIsActive = false;
    }

    public void MakeCompletionPredicate(CompletionPredicateType predicate)
    {
        switch (predicate)
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
            case CompletionPredicateType.EnjoyReward:
                _predicate = EnjoyReward;
                break;
        }
    }

    // This means round is ended solely because player was defeated
    private void EndRoundPremature(Transform t)
    {
        roundCompletionType = RoundCompletionType.Failed;
        EndRound(successfulRound: false);
    }

    // Means the round has ended (successful = won/completed, otherwise, failed)
    public void EndRound(bool successfulRound) => StartCoroutine(IEndRound(successfulRound));

    public IEnumerator IEndRound(bool successfulRound)
    {
        _roundIsActive = false;
        roundTimer.PauseTimer();
        GameplayListeners(attachOrRemove: false);

        // DETERMINE HOW ROUND ENDED (SUCCESSFUL OR FAILED OR BOSS DEFEATED?)
        if (successfulRound)
        {
            if (MapCreator.Instance.mapType == MapType.normal) roundCompletionType = RoundCompletionType.Completed;
            else if (MapCreator.Instance.mapType == MapType.boss) roundCompletionType = RoundCompletionType.BossDefeated;
            else roundCompletionType = RoundCompletionType.Reward;
            nextRoundNumber++;
            roundNumberInBiome++;
        }
        else
        {
            roundCompletionType = RoundCompletionType.Failed;
        }

        Animator roundEndWindow = null;
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
            case RoundCompletionType.Reward:
                roundEndWindow = startingHostileRoundWindow;
                break;
        }

        GameLoopEvents.OnEndRound?.Invoke();        
        roundEndWindow.gameObject.SetActive(true);
        Time.timeScale = 0.4f;
        roundEndWindow.SetTrigger("Entry");
        yield return new WaitForSecondsRealtime(3f);
        roundEndWindow.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(1.5f);
        roundEndWindow.gameObject.SetActive(false);
        Time.timeScale = 1f;        
        CompleteRound();
    }

    public void CompleteRound()
    {
        EnemiesManager.Instance.ResetEnemiesList();
        Destroy(EndPortal);

        if (roundCompletionType == RoundCompletionType.Reward)
        {
            ResetStates();
            InitiateRound();
            return;
        }

        float score = ScoreFormula.CalculateScore(totalEnemiesSpawned * (MapCreator.Instance.mapType == MapType.boss ? 50f : 10f), roundTimer.GetTime(), playerMaxHealth, damageTaken, out goldToReward);

        resultScreen.timeTakenText.text = UtilityFunctions.GetFormattedTime(roundTimer.GetTime());
        resultScreen.damageTakenText.text = $"{damageTaken} ({(int)(((float)damageTaken/(float)playerMaxHealth)*100.0f)}%)";
        resultScreen.totalScoreText.text = $"{Mathf.RoundToInt(score)}%";

        if (roundCompletionType == RoundCompletionType.Failed)
        {
            resultScreen.rankText.text = "Reached: " + nextRoundNumber.ToString();
            resultScreen.goldEarnedText.text = "0";
            resultScreen.shopButton.SetActive(false);
            resultScreen.mainMenuButton.SetActive(true);
            GameLoopEvents.OnEndSession?.Invoke();
        }
        else
        {
            resultScreen.shopButton.SetActive(true);
            resultScreen.mainMenuButton.SetActive(false);
            resultScreen.goldEarnedText.text = goldToReward + "";
            playerStats.UpdateGold(goldToReward); // save it to db
            resultScreen.rankText.text = ScoreFormula.GetScoreRank((int)score);
        }

        // Update Results Screen
        //Results.goldText.text = "+" + goldRewarded;

        // Update Shop Screen
        shopScreen.goldText.text = playerStats.Gold.ToString();
        shopScreen.messageText.text = "Spend your gold here to purchase upgrades!";

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
        MapGeneration.Instance.ClearMap();
        yield return null;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameLoopEvents.OnBootupGame?.Invoke();
        loadingScreen.SetActive(false);
    }

    private void GameplayListeners(bool attachOrRemove)
    {
        if (attachOrRemove)
        {
            playerStats.OnHealthChange.AddListener(AccrueDamageTaken);
            playerController.OnDeathEvent += EndRoundPremature;
            EnemiesManager.Instance.OnEnemiesListUpdated.AddListener(MapCreator.Instance.UpdateObjectiveText);
        }
        else
        {
            playerStats.OnHealthChange.RemoveListener(AccrueDamageTaken);
            playerController.OnDeathEvent -= EndRoundPremature;
            EnemiesManager.Instance.OnEnemiesListUpdated.RemoveListener(MapCreator.Instance.UpdateObjectiveText);
        }
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

    public void CalculateShop()
    {
        ShopManager.Instance.CalculateShopItems(playerStats.Gold, new List<SO_Mod>());
    }
    #endregion

    private IEnumerator DelayedAction(float s, Action a)
    {
        yield return new WaitForSeconds(s);
        a?.Invoke();
    }

    private IEnumerator PlayTriggerAnimation(Animator animator, float delay)
    {
        animator.gameObject.SetActive(true);
        animator.SetTrigger("Entry");
        yield return new WaitForSecondsRealtime(delay);
        animator.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(1.5f);
        animator.gameObject.SetActive(false);
    }

    /* * * * * * *  P R E D I C A T E S  * * * * * * * * */


    public bool KillAllEnemies(bool mapReady)
    {
        int enemiesLeft = EnemiesManager.Instance.EnemiesCount;
        //Debug.Log($"CURRENT PREDICATE: [KILL ALL ENEMIES] STATUS:  mapReady: {mapReady}  -  enemies left: {enemiesLeft}");
        return mapReady && enemiesLeft <= 0;
    }

    public bool EnjoyReward(bool mapReady)
    {
        return mapReady;
    }

    // Implement Get All Gems

    // Implement survive X minutes


    /* * * * * * *  P R E D I C A T E S  * * * * * * * * */

    private void PlayerFallDamage()
    {
        playerStats.UpdateCurrentHealth((int)(-playerStats.MaxHealth.FinalValue * GlobalValues.FallDamage / 100.0f));
    }

}
