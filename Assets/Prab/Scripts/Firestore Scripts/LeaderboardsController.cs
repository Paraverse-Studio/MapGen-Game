using ParaverseWebsite.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardsController : MonoBehaviour
{
    public Transform leaderboardsParentGO;
    public GameObject leaderboardsStatContainerPrefab;

    private List<GameObject> leaderboardStats = new List<GameObject>();

    private void OnEnable()
    {
        ClearAll();
        FirebaseDatabaseManager.Instance.GetLeaderboards(GetLeaderboardsStatsFromDatabase);
    }

    private void GetLeaderboardsStatsFromDatabase(Dictionary<string, LeaderboardsModel> model)
    {
        int idx = 1;
        foreach (KeyValuePair<string, LeaderboardsModel> stat in model.OrderByDescending(key => key.Value.CumulativeTotalScore))
        {
            CreateLeaderboardStatContainer(idx, stat.Value);
            idx++;
        }
    }

    private void CreateLeaderboardStatContainer(int idx, LeaderboardsModel model)
    {
        GameObject obj = Instantiate(leaderboardsStatContainerPrefab, leaderboardsParentGO);
        obj.GetComponent<LeaderboardsStatsContainer>().Init(idx, model, DataMapper.BloodlineSpriteMapper, DataMapper.SkillSpriteMapper, DataMapper.EffectSpriteMapper);
        leaderboardStats.Add(obj);
    }

    private void ClearAll()
    {
        foreach (GameObject obj in leaderboardStats)
        {
            Destroy(obj);
        }
        leaderboardStats.Clear();
    }
}
