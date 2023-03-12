using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum InteractableObjects
{
    spellbookMaster, // lets you buy and upgrade skills
    merchant, // lets you buy Effect mods
    trader, // lets you sell mods (not yet available)
    skillGiver, // Ronny, gives you a free random skill
    trainer
}

public class InteractableObjectsManager : MonoBehaviour
{
    public static InteractableObjectsManager Instance;

    [System.Serializable]
    public struct InteractableWindow
    {
        public InteractableObjects type;
        public ItemDisplayCreator display;
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
