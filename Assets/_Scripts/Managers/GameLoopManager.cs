using Paraverse;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI timeTakenText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI goldEarnedText;
    public GameObject shopButton;
  }

  [System.Serializable]
  public struct PlayerSessionData
  {
    public int roundReached;
    public float sessionLength;
    public int damageTaken;
    public int totalScore;
    public int goldEarned;
    public int mobsDefeated;
    public int bossesDefeated;
    public int mysticDungeons;
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
  public SummaryView summaryView;
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
  public int roundNumber;
  public int roundNumberInBiome;
  public RoundCompletionType roundCompletionType;
  private GameObject player;
  MobStats playerStats;
  PlayerCombat playerCombat;
  PlayerController playerController;
  public int damageTaken;
  public float score;
  public PlayerSessionData sessionData;

  private int totalEnemiesSpawned;
  private int lastHealthSaved = -1;
  private int playerMaxHealth;
  private int goldToReward = 0;

  private bool _roundIsActive = false; public bool RoundIsActive => _roundIsActive;

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
    playerCombat = player.GetComponentInChildren<PlayerCombat>();

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
        {
          AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(1).AddText("Gate is open!"));
        }
        EndPortal.Activate(true);
      }
    }

    if (player.transform.position.y <= -20f && _roundIsActive)
    {
      UtilityFunctions.TeleportObject(player, MapGeneration.Instance.GetClosestBlock(player.transform).transform.position + new Vector3(0, 0.5f, 0));

      if (MapCreator.Instance.mapType != MapType.reward)
      {
        Invoke(nameof(PlayerFallDamage), 0.15f);
      }
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
    if (Input.GetKeyDown(KeyCode.I) && RoundIsActive)
    {
      List<MobController> m = EnemiesManager.Instance.Enemies;
      foreach (MobController _m in m)
      {
        _m.GetComponentInChildren<MobStats>().UpdateCurrentHealth(-_m.GetComponentInChildren<MobStats>().CurHealth);
      }
      EndRound(true);
    }
    if (Input.GetKeyDown(KeyCode.T) && RoundIsActive)
    {
      playerStats.UpdateGold(500);
      ShopManager.Instance.ShopWindow.SetActive(!ShopManager.Instance.ShopWindow.activeSelf);
      CalculateShop();
    }
#endif
    if (Input.GetKey(KeyCode.ScrollLock) && Input.GetKeyDown(KeyCode.F9))
    {
      ModsManager.Instance.BaryonModeMod.Activate(GlobalSettings.Instance.player);
      ModsManager.Instance.BaryonModeMod.Consume();
      ModsManager.Instance.PurchasedMods.Add(ModsManager.Instance.BaryonModeMod);
    }

  }


  #region MAIN_MENU
  public void InitiateSession()
  {
    ResetSessionStates();
    GameLoopEvents.OnInitiateSession?.Invoke();
    InitiateRound();
  }

  public void InitiateRound()
  {
    roundNumber++;
    roundNumberInBiome++;
    GameLoopEvents.OnInitiateRound?.Invoke();
  }

  public void StartRound()
  {
    ResetRoundStates();
    GameLoopEvents.OnStartRound?.Invoke();

    totalEnemiesSpawned = EnemiesManager.Instance.EnemiesCount;
    playerMaxHealth = (int)playerStats.MaxHealth.FinalValue;

    GameplayListeners(attachOrRemove: true);

    if (roundNumber == 1 && MapCreator.Instance.mapType != MapType.reward) // only for round 1, since it's tutorial
    {
      AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(1).StartDelay(1.5f).OverrideDuration(3f)
          .AddText("Defeat all enemies & pass through the gate!"));
    }

    MakeCompletionPredicate(CompletionPredicate);
    _roundIsActive = true;
  }

  public void ResetRoundStates()
  {
    damageTaken = 0;
    totalEnemiesSpawned = 0;
    lastHealthSaved = 0;
    playerMaxHealth = 0;
    goldToReward = 0;
    _roundIsActive = false;
  }

  public void ResetSessionStates()
  {
    roundNumber = 0;
    roundNumberInBiome = 0;
  }

  public void FactoryResetSessionStats()
  {
    sessionData.roundReached = 0;
    sessionData.sessionLength = 0;
    sessionData.damageTaken = 0;
    sessionData.totalScore = 0;
    sessionData.goldEarned = 0;
    sessionData.mobsDefeated = 0;
    sessionData.bossesDefeated = 0;
    sessionData.mysticDungeons = 0;
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
    roundTimer.PauseTimer();
    GameplayListeners(attachOrRemove: false);

    // DETERMINE HOW ROUND ENDED (SUCCESSFUL OR FAILED OR BOSS DEFEATED?)
    if (successfulRound)
    {
      if (MapCreator.Instance.mapType == MapType.normal) roundCompletionType = RoundCompletionType.Completed;
      else if (MapCreator.Instance.mapType == MapType.boss) roundCompletionType = RoundCompletionType.BossDefeated;
      else roundCompletionType = RoundCompletionType.Reward;
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

    roundEndWindow.gameObject.SetActive(true);
    Time.timeScale = 0.4f;
    roundEndWindow.SetTrigger("Entry");
    yield return new WaitForSecondsRealtime(3f);
    roundEndWindow.SetTrigger("Exit");
    yield return new WaitForSecondsRealtime(1.5f);
    roundEndWindow.gameObject.SetActive(false);
    Time.timeScale = 1f;

    // boss ones don't auto-complete after above animation, u have to touch portal,
    // because for boss maps, the above animation happens right after killing boss, not touching portal 
    if (roundCompletionType != RoundCompletionType.BossDefeated)
    {
      CompleteRound();
    }
  }

  public void CompleteRound()
  {
    _roundIsActive = false;
    GameLoopEvents.OnEndRound?.Invoke();
    EnemiesManager.Instance.ResetEnemiesList();
    Destroy(EndPortal);

    if (roundCompletionType == RoundCompletionType.Reward)
    {
      InitiateRound();
    }
    else // a normal or boss round was completed, or failed (aka, anything but a reward map)
    {
      score = ScoreFormula.CalculateScore(totalEnemiesSpawned * (MapCreator.Instance.mapType == MapType.boss ? 50f : 10f), roundTimer.GetTime(), playerMaxHealth, damageTaken, out goldToReward);
      UpdatePlayerSessionData();

      if (roundCompletionType == RoundCompletionType.Failed || !MapCreator.Instance.NextMapCreatable())
      {
        summaryView.gameObject.SetActive(true);
        summaryView.Populate(sessionData, playerStats, playerCombat);
        GameLoopEvents.OnEndSession?.Invoke();
      }
      else
      {
        resultScreen.resultTitleText.text = $"Round " + roundNumber + " Results";
        resultScreen.timeTakenText.text = UtilityFunctions.GetFormattedTime(roundTimer.GetTime());
        resultScreen.damageTakenText.text = $"{damageTaken} ({(int)(((float)damageTaken / (float)playerMaxHealth) * 100.0f)}%)";
        resultScreen.totalScoreText.text = $"{Mathf.RoundToInt(score)}%";
        resultScreen.goldEarnedText.text = goldToReward + "";
        resultScreen.rankText.text = ScoreFormula.GetScoreRank((int)score);

        playerStats.UpdateGold(goldToReward); // save it to db
        roundResultsWindow.SetActive(true); // or play an animation
      }

      GameLoopEvents.OnUI?.Invoke();
    }

    // save it in database here, we need to save stats in db asap so players
    // who might d/c right after ending get their stuff saved
  }

  private void UpdatePlayerSessionData()
  {
    sessionData.roundReached = roundNumber;
    sessionData.sessionLength += roundTimer.GetTime();
    sessionData.damageTaken += damageTaken;

    if (roundCompletionType != RoundCompletionType.Failed)
    {
      sessionData.totalScore += Mathf.RoundToInt(score);
    }

    sessionData.goldEarned += goldToReward;

    //if (MapCreator.Instance.mapType == MapType.normal)
    //{
    //    sessionData.mobsDefeated += 14; // TODO , do properly after we add OnKilledUnitEvent (pass in mobController so we can check if boss or not)
    //}
    //if (MapCreator.Instance.mapType == MapType.boss)
    //{
    //    sessionData.bossesDefeated += 1;
    //}

    if (roundCompletionType != RoundCompletionType.Failed && MapCreator.Instance.isMysticMap)
    {
      sessionData.mysticDungeons++;
    }
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
      playerMaxHealth = (int)playerStats.MaxHealth.FinalValue;
      lastHealthSaved = (int)playerStats.CurHealth;
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

    damageTaken += Mathf.Max(lastHealthSaved - playerCurrentHealth, 0);

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