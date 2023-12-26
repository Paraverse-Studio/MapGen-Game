using NaughtyAttributes;
using Paraverse.Mob.Controller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public enum MapType
{
    normal, boss, reward
}

public class MapCreator : MonoBehaviour
{
    [System.Serializable]
    public struct MapGenDataPair
    {
        public string mapName;
        public SO_MapGenData map;
        public SO_MapGenData bossMap;
        public SO_MapGenData rewardMap;
    }    

    [System.Serializable]
    public struct MysticMapData
    {
        public Material mapSkybox;
        public VolumeProfile ppProfile;
        public Color environmentLightingColor;
    }

    public static MapCreator Instance;

    [Space(10)]
    [Header("Biomes")]
    public List<MapGenDataPair> maps;

    [Header("Mystic Map Properties")]
    public MysticMapData mysticMapData;
    public Color defaultEnvironmentColor;

    [Min(1)]
    public int switchMapAfterNumOfRounds;

    [Header("Progression")]
    [MinMaxSlider(0f, 10f)]
    public Vector2 bossMapGapLimit;
    [MinMaxSlider(0f, 10f)]
    public Vector2 rewardMapGapLimit;
    [MinMaxSlider(0f, 10f)]
    public Vector2 mysticMapGapLimit;

    [Header("Enemies Scaling")]
    public float enemyDamageScalingPerRound;
    public float enemyHealthScalingPerRound;
    public float bossDamageScalingPerRound;
    public float bossHealthScalingPerRound;

    [Header("UI References")]
    public TextMeshProUGUI objectiveTitle;
    public TextMeshProUGUI objectiveSubtitle;

    [Header("Prefabs")]
    public ChestObject chestPrefab;
    public GameObject blackSmithPrefab;
    public GameObject merchantPrefab;
    public GameObject skillGiverPrefab;

    [Header("Rare Chest Chance Growth"), Tooltip("Game rounds * rare chest growth = chance of rare chest")]
    public float rareChestBaseChance;

    [Header("[Run-time data]")]
    public MapType mapType;
    public bool isMysticMap;
    public bool offerMysticDungeon;
    public int EnemiesSpawned;

    private int roundsSinceLastBossMap;
    private int roundsSinceLastRewardMap;
    private int roundsSinceLastOFferMysticMap;
    private int biomeIndex;
    private bool biomeChangePending;

    public void ResetMapCreator()
    {
        EnemiesSpawned = 0;
        roundsSinceLastBossMap = 0;
        roundsSinceLastRewardMap = 0;
        roundsSinceLastOFferMysticMap = 0;
        biomeIndex = 0;
        biomeChangePending = false;
    }

    private void ResetRuntimeVariables() // between rounds
    {
        EnemiesSpawned = 0;
        mapType = MapType.normal;
        isMysticMap = false;
        offerMysticDungeon = false;
    }

    private void Awake()
    {
        Instance = this;
    }

    public bool NextMapCreatable()
    {
        if (biomeChangePending && (biomeIndex + 1) >= maps.Count) return false;
        return true;
    }

    public void CreateMap()
    {
        ResetRuntimeVariables();

        // Determining biome, removed: we don't change biome after a predetermined # of rounds anymore
        //biomeIndex = adjustedRoundNumber / switchMapAfterNumOfRounds;
        if (biomeChangePending)
        {
            biomeIndex++;
            biomeChangePending = false;
            GameLoopManager.Instance.roundNumberInBiome = 1;
        }

        // Determining type of map (normal, boss, reward)
        if (roundsSinceLastBossMap >= bossMapGapLimit.x)
        {
            float chance = Mathf.Max(0, 1.0f / (bossMapGapLimit.y - bossMapGapLimit.x + 1));
            if (Random.value <= chance || roundsSinceLastBossMap >= bossMapGapLimit.y)
            {
                mapType = MapType.boss;
            }
        }

        // Determining type of map (normal, boss, reward)
        if (roundsSinceLastRewardMap >= rewardMapGapLimit.x)
        {
            float chance = Mathf.Max(0, 1.0f / (rewardMapGapLimit.y - rewardMapGapLimit.x + 1));
            if (Random.value <= chance || roundsSinceLastRewardMap >= rewardMapGapLimit.y)
            {
                mapType = MapType.reward;

                // NEW* Mystic Dungeon map every few reward maps
                if (roundsSinceLastOFferMysticMap >= mysticMapGapLimit.x)
                {
                    float mysticChance = Mathf.Max(0, 1.0f / (mysticMapGapLimit.y - mysticMapGapLimit.x + 1));
                    if (Random.value <= mysticChance || roundsSinceLastOFferMysticMap >= mysticMapGapLimit.y)
                    {
                        offerMysticDungeon = true;
                    }
                }
            }
        }

        if (GameLoopManager.Instance.roundNumber == 2) // forcing round 2 to be a reward map
        {
            mapType = MapType.reward;
        }

        SetMapType();

        // Finally, start the map building
        MapGeneration.Instance.RegenerateMap();

        // Establish the objective to complete
        if (mapType == MapType.reward) GameLoopManager.Instance.CompletionPredicate = GameLoopManager.CompletionPredicateType.EnjoyReward;
        else GameLoopManager.Instance.CompletionPredicate = GameLoopManager.CompletionPredicateType.KillAllEnemies;

        // Post map generation steps
        UpdateObjectiveText();
    }

    private void SetMapType()
    {
        roundsSinceLastBossMap++;
        roundsSinceLastRewardMap++;
        roundsSinceLastOFferMysticMap++;

        switch (mapType)
        {
            case MapType.normal:
                MapGeneration.Instance.M = maps[biomeIndex].map;
                break;

            case MapType.boss:
                MapGeneration.Instance.M = maps[biomeIndex].bossMap;

                roundsSinceLastBossMap = 0;                
                biomeChangePending = true;
                break;

            case MapType.reward:
                MapGeneration.Instance.M = maps[biomeIndex].rewardMap;
                roundsSinceLastRewardMap = 0;
                if (offerMysticDungeon) roundsSinceLastOFferMysticMap = 0;
                break;
        }
    }

    public void UpdateObjectiveText()
    {
        if (GameLoopManager.Instance.CompletionPredicate == GameLoopManager.CompletionPredicateType.KillAllEnemies)
        {
            UpdateObjectiveText(EnemiesManager.Instance.Enemies);
        }
        else if (GameLoopManager.Instance.CompletionPredicate == GameLoopManager.CompletionPredicateType.EnjoyReward)
        {
            UpdateObjectiveText(maps[biomeIndex].mapName);                       
        }
        // Create update objective texts for other types of completion predicates
    }

    public void UpdateObjectiveText(string mapName)
    {
        objectiveTitle.text = mapName;
        objectiveSubtitle.text = $"Savor the tranquility!";
    }

    public void UpdateObjectiveText(List<MobController> enemies)
    {
        int enemiesCount = (null == enemies ? EnemiesManager.Instance.EnemiesCount : enemies.Count);
        if (EnemiesSpawned == 0) EnemiesSpawned = enemiesCount;
        objectiveTitle.text = EnemiesSpawned == 1? "Defeat the colossus!" : "Defeat the enemies!";
        objectiveSubtitle.text = $"Remaining: {enemiesCount} / {EnemiesSpawned}";
    }


}
