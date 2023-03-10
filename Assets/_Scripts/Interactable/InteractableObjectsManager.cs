using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum InteractableObjects
{
    blacksmith, merchant, trader
}

public class InteractableObjectsManager : MonoBehaviour
{
    public static InteractableObjectsManager Instance;

    [System.Serializable]
    public struct InteractableWindow
    {
        public InteractableObjects type;
        public GameObject obj;
    }

    public List<InteractableWindow> windows;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
