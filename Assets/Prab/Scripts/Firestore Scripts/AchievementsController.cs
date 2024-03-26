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
        for (int i = 0; i < Data.Length - 1; i++)
        {
            CreateAchievementsContainer(Data[i], model);
        }
    }

    private void CreateAchievementsContainer(AchievementData data, AchievementsModel model)
    {
        GameObject obj = Instantiate(achievementsContainerPrefab, achievementsParentGO);
        obj.GetComponent<AchievementsContainer>().Init(data, model);
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
