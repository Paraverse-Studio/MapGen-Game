using NaughtyAttributes;
using Paraverse.Mob.Controller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [System.Serializable]
    public struct MapGenDataPair
    {
        public SO_MapGenData map;
        public SO_MapGenData bossMap;
        public SO_MapGenData rewardMap;
    }

    public static MapCreator Instance;

    [Space(10)]
    [Header("Biomes")]
    public List<MapGenDataPair> maps;
    [Min(1)]
    public int switchMapAfterNumOfRounds;

    [Header("Progression")]
    [MinMaxSlider(0f, 10f)]
    public Vector2 bossMapGapLimit;
    [MinMaxSlider(0f, 10f)]
    public Vector2 rewardMapGapLimit;

    [Header("Enemies")]
    public float enemyScalingPerRound;

    [Header("UI References")]
    public TextMeshProUGUI objectiveText;

    [Header("[Run-time data]")]
    public bool IsBossMap;
    public bool IsRewardMap;
    public int EnemiesSpawned;

    private int roundsSinceLastBossMap = 0;
    private int roundsSincelastRewardMap = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateMap()
    {
        ResetRuntimeVariables();

        int roundNumber = GameLoopManager.Instance.nextRoundNumber;
        int adjustedRoundNumber = roundNumber - 1;

        // Determining biome
        int mapIndex = adjustedRoundNumber / switchMapAfterNumOfRounds;

        // Determining type of map (normal, boss, reward)
        if (roundsSincelastRewardMap >= rewardMapGapLimit.x)
        {
            float chance = Mathf.Max(0, 1.0f / (rewardMapGapLimit.y - rewardMapGapLimit.x));
            if (Random.value <= chance || roundsSincelastRewardMap >= rewardMapGapLimit.y)
            {
                IsRewardMap = true;
                MapGeneration.Instance.M = maps[mapIndex].rewardMap;
                roundsSincelastRewardMap = 0;
            }
        }
        else if (roundsSinceLastBossMap >= bossMapGapLimit.x)
        {
            float chance = Mathf.Max(0, 1.0f / (bossMapGapLimit.y - bossMapGapLimit.x));
            if (Random.value <= chance || roundsSinceLastBossMap >= bossMapGapLimit.y)
            {
                IsBossMap = true;
                MapGeneration.Instance.M = maps[mapIndex].bossMap;
                roundsSinceLastBossMap = 0;
            }
        }
        else
        {
            MapGeneration.Instance.M = maps[mapIndex].map;
        }

        if (!IsBossMap) roundsSinceLastBossMap++;
        if (!IsRewardMap) roundsSincelastRewardMap++;

        // Finally, start the map building
        MapGeneration.Instance.RegenerateMap();

        // Post map generation steps
        UpdateObjectiveText();
    }

    private void ResetRuntimeVariables()
    {
        EnemiesSpawned = 0;
        IsBossMap = false;
        IsRewardMap = false;
    }

    public void UpdateObjectiveText()
    {
        if (GameLoopManager.Instance.CompletionPredicate == GameLoopManager.CompletionPredicateType.KillAllEnemies)
        {
            UpdateObjectiveText(EnemiesManager.Instance.Enemies);
        }
        // Create update objective texts for other types of completion predicates
    }

    public void UpdateObjectiveText(List<MobController> enemies)
    {
        int enemiesCount = (null == enemies ? EnemiesManager.Instance.EnemiesCount : enemies.Count);
        if (EnemiesSpawned == 0) EnemiesSpawned = enemiesCount;
        objectiveText.text = $"Remaining: {enemiesCount} / {EnemiesSpawned}";
    }



}
