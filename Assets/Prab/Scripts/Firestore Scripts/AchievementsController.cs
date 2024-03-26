using ParaverseWebsite.Models;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsController : MonoBehaviour
{
    public Transform achievementsParentGO;
    public GameObject achievementsContainerPrefab;
    public AchievementData[] Data;

    private List<GameObject> achievements = new List<GameObject>();

    //[SerializeField]
    //private GameObject InformationPage;


    private void OnEnable()
    {
        ClearAll();
        FirebaseDatabaseManager.Instance.GetAchievement(MainMenuController.Instance.Username,
          //  IF USER IS FOUND!!
          (achievementsModel) => UpdateAchievementContainers(achievementsModel),
          //  IF USER IS NOT FOUND!!
          () => OpenInformationPage()
        );
    }

    private void OnDisable()
    {
        CloseInformationPage();
    }

    private void UpdateAchievementContainers(AchievementsModel model)
    {
        CreateAchievementsContainer(Data[0], model.RoundsReachedAchievement.CompletedCount, model.RoundsReachedAchievement.TargetCounts[model.RoundsReachedAchievement.Index]);
        CreateAchievementsContainer(Data[1], model.MobsKilledAchievement.CompletedCount, model.MobsKilledAchievement.TargetCounts[model.MobsKilledAchievement.Index]);
        CreateAchievementsContainer(Data[2], model.BossesKilledAchievement.CompletedCount, model.BossesKilledAchievement.TargetCounts[model.BossesKilledAchievement.Index]);
        CreateAchievementsContainer(Data[3], model.VagabondRoundsCompletedAchievement.CompletedCount, model.VagabondRoundsCompletedAchievement.TargetCounts[model.VagabondRoundsCompletedAchievement.Index]);
        CreateAchievementsContainer(Data[4], model.HarrierRoundsCompletedAchievement.CompletedCount, model.HarrierRoundsCompletedAchievement.TargetCounts[model.HarrierRoundsCompletedAchievement.Index]);
        CreateAchievementsContainer(Data[5], model.PioneerRoundsCompletedAchievement.CompletedCount, model.PioneerRoundsCompletedAchievement.TargetCounts[model.PioneerRoundsCompletedAchievement.Index]);
        CreateAchievementsContainer(Data[6], model.ScholarRoundsCompletedAchievement.CompletedCount, model.ScholarRoundsCompletedAchievement.TargetCounts[model.ScholarRoundsCompletedAchievement.Index]);
    }

    private void CreateAchievementsContainer(AchievementData data, int curCount, int nextCount)
    {
        GameObject obj = Instantiate(achievementsContainerPrefab, achievementsParentGO);
        obj.GetComponent<AchievementsContainer>().Init(data, curCount, nextCount);
        achievements.Add(obj);
    }

    private void ClearAll()
    {
        foreach (GameObject obj in achievements)
        {
            Destroy(obj);
        }
        achievements.Clear();
    }

    private void OpenInformationPage()
    {
        //InformationPage.SetActive(true);
    }

    private void CloseInformationPage()
    {
        //InformationPage.SetActive(false);
    }
}
