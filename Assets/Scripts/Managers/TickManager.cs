using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITickElement
{
    void Tick();
}



[System.Serializable]
public enum TickDelayOption
{
    t0 = 1, 
    t2 = 2,
    t3 = 3,
    t4 = 4,
    t5 = 5,
    t6 = 6,
    t10 = 10, 
    t20 = 20, 
    t30 = 30, 
    t60 = 60, 
    t120 = 120
}

[System.Serializable]
public class TickElement
{
    public ITickElement script;
    public TickDelayOption frameDelay;
}

public class TickManager : MonoBehaviour
{
    public static TickManager Instance;
    public List<TickElement> tickElements;

    private int size = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.frameCount % 60 == 0)
        {
            
        }

        size = tickElements.Count;

        for (int i = 0; i < size; ++i)
        {
            if (null != tickElements[i] && null != tickElements[i].script)
            {
                if (Time.frameCount % (int)tickElements[i].frameDelay == 0) tickElements[i].script.Tick();
            }
        }

    }

    public void Unsubscribe(ITickElement unlisteningScript)
    {
        for (int i = 0; i < tickElements.Count; ++i)
        {
            if (null != tickElements[i] && tickElements[i].script == unlisteningScript)
            {
                tickElements.RemoveAt(i);
            }
        }
    }

    public void Subscribe(ITickElement listeningScript, TickDelayOption delayOption = TickDelayOption.t0)
    {
        TickElement newTickElement = new TickElement();
        newTickElement.script = listeningScript;
        newTickElement.frameDelay = delayOption;

        tickElements.Add(newTickElement);
    }


}
