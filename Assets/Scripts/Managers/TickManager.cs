using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TickDelayOption
{
    t0, t2, t5, t10, t20, t30, t60, t120
}


[System.Serializable]
public class TickElement
{
    public Block block;
    public int frameDelay;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.frameCount % 60 == 0)
        {
            size = tickElements.Count;            
        }

        for (int i = 0; i < size; ++i)
        {
            if (null != tickElements[i] && tickElements[i].block)
            {
                tickElements[i].block.Tick();
            }
        }
    }

    public void Unsubscribe(Block block)
    {
        for (int i = 0; i < tickElements.Count; ++i)
        {
            if (tickElements[i].block == block) tickElements.RemoveAt(i);
        }
    }

    public void Subscribe(Block block)
    {
        TickElement newTickElement = new TickElement();
        newTickElement.block = block;

        tickElements.Add(newTickElement);
    }


}
