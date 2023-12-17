using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    public int TimeObjectID;
}


public class TimeManager : MonoBehaviour
{
    [System.Serializable]
    public class TimeChangePair
    {
        public TimeChanger timeChangeClass;
        public float timeRequested;

        public TimeChangePair (TimeChanger c = null, float t = 1f)
        {
            timeChangeClass = c; timeRequested = t;
        }
    }

    public static TimeManager Instance;
    public List<TimeChangePair> timeRequestList = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RequestTimeChange(TimeChanger changeClass, float timeValue)
    {
        // Check if this request is coming from a class already in the list, if not add it
        TimeChangePair requestClass = new();

        for (int i = 0; i < timeRequestList.Count; ++i)
        {
            if (timeRequestList[i].timeChangeClass.TimeObjectID == changeClass.TimeObjectID)
            {
                requestClass = timeRequestList[i];
                timeRequestList[i].timeRequested = timeValue;
            }
        }

        if (null == requestClass.timeChangeClass)
        {
            requestClass = new TimeChangePair(changeClass, timeValue);
            timeRequestList.Add(requestClass);
        }

        // if a class is requesting back to normal time, it can be removed
        if (timeValue == 1f)
        {
            timeRequestList.Remove(requestClass);
        }

        // Now, find the class requesting lowest time scale, and apply that
        float lowestTime = 1f;

        for (int i = 0; i < timeRequestList.Count; ++i)
        {
            if (timeRequestList[i].timeRequested < lowestTime)
            {
                lowestTime = timeRequestList[i].timeRequested;
            }
        }

        Time.timeScale = lowestTime;
    }
}
