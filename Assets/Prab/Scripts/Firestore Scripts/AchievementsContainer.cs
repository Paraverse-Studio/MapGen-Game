using ParaverseWebsite.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsContainer : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public Image Image;
    public TextMeshProUGUI ProgressCount;
    public TextMeshProUGUI Description;

    public AchievementsContainer(string title, string description, string progressCount, Sprite sprite)
    {
        Title.text = title;
        Description.text = description;
        ProgressCount.text = progressCount;
        Image.sprite = sprite;
    }

    public void Init(AchievementData data, int curCount, int nextCount)
    {
        Title.text = data.Title;
        Description.text = data.Description;
        ProgressCount.text = $"{curCount} / {nextCount}";
        Image.sprite = data.Sprite;
    }
}
