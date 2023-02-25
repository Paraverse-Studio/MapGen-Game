using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementItem : MonoBehaviour
{
    [Header("Announcement Properties")]
    public Announcement Announcement;
    public Animator Animator;

    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI textLabel;
    public Image image;

    public AnnouncementItem(Announcement a)
    {
        Announcement = a;
        UpdateDisplay();
    }

    private void OnEnable()
    {
        UpdateDisplay();
    }

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (null == Announcement) return;

        if (titleLabel) titleLabel.text = Announcement.Title;
        if (textLabel) textLabel.text = Announcement.Text;
        if (image) image.sprite = Announcement.Image;
    }
}
