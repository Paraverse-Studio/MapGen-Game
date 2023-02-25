using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcement
{
    public string Title = string.Empty;
    public string Text = string.Empty;
    public Sprite Image = null;
    public float Duration = 3f;
    public int Type = 0;

    public Announcement()
    {
    }

    public Announcement AddTitle(string t)
    {
        Title = t;
        return this;
    }

    public Announcement AddText(string t)
    {
        Text = t;
        return this;
    }

    public Announcement AddImage(Sprite s)
    {
        Image = s;
        return this;
    }

    public Announcement OverrideDuration(float f)
    {
        Duration = Mathf.Max(f, 0.1f);
        return this;
    }

    public Announcement AddType(int i)
    {
        Type = Mathf.Clamp(i, 0, AnnouncementManager.Instance.announcementTypes.Length);
        Duration = AnnouncementManager.Instance.announcementTypes[Type].defaultDuration;
        return this;
    }
}


public class AnnouncementManager : MonoBehaviour
{
    [System.Serializable]
    public struct AnnouncementType
    {
        public int Index;
        public AnnouncementItem Item;
        public float defaultDuration;
    }
    public static AnnouncementManager Instance;
    public static int CurrentAnnouncementCount;
    private static Queue<Announcement> _announcements = new();

    [Header("References")]
    public Transform announcementsContainer;
    public float delayBetweenAnnouncements;

    public AnnouncementType[] announcementTypes;

    private void Awake()
    {
        Instance = this;
    }

    // This variation is to invoke through button callbacks
    public void QueueAnnounceType1(string msg)
    {
        QueueAnnouncement(new Announcement().AddType(1).AddText(msg));
    }

    public void QueueAnnouncement(Announcement a)
    {    
        if (null != a)
        {
            _announcements.Enqueue(a);
            CurrentAnnouncementCount = _announcements.Count;
        }

        if (_announcements.Count <= 1) ProcessQueue();
    }

    public void QueueAnnouncementUnique(Announcement a)
    {
        foreach(Announcement announcement in _announcements)
        {
            if (a.Text == announcement.Text) return;
        }

        QueueAnnouncement(a);
    }

    private void ProcessQueue()
    {
        if (_announcements.TryPeek(out Announcement a)) 
        {
            AnnouncementItem item = Instantiate(announcementTypes[a.Type].Item, announcementsContainer);
            item.Announcement = a;
            StartCoroutine(IPlayAnnouncement(item));
        }
    }

    private IEnumerator IPlayAnnouncement(AnnouncementItem item)
    {
        item.Animator.SetTrigger("Entry");
        yield return new WaitForSecondsRealtime(item.Announcement.Duration);
        item.Animator.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(1.2f);
        _announcements.Dequeue();
        Destroy(item.gameObject, 5f);

        yield return new WaitForSecondsRealtime(delayBetweenAnnouncements);
        ProcessQueue();
    }
}
