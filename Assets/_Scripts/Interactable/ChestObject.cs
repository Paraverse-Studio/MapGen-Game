using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestObject : MonoBehaviour
{
    [System.Serializable]
    public struct ChestTier
    {
        public string chestName;
        public GameObject model;
        public GameObject modelGlow;
        public SO_Item reward;
    }

    [SerializeField]
    private GameObject chestObject;

    [SerializeField]
    private ParticleSystem chestGlowFX;

    [SerializeField]
    private ParticleSystem deathFX;
 
    [SerializeField]
    private ChestTier[] chestTiers;

    [SerializeField]
    private int tier;

    private Interactable interactable;
    private Selectable selectable;

    // Start is called before the first frame update
    void Start()
    {
        if (tier == -1) tier = Random.Range(0, chestTiers.Length - 1);

        Initialize(tier);
    }

    public void Initialize(int tierProvided)
    {
        tier = Mathf.Clamp(tierProvided, 0, chestTiers.Length - 1);

        // Set this object to the current tier provided (set its name and looks)
        for (int i = 0; i < chestTiers.Length; ++i)
        {
            if (i == tier)
            {
                chestTiers[i].model.SetActive(true);
                chestTiers[i].modelGlow.SetActive(true);
            }
            else
            {
                chestTiers[i].model.SetActive(false);
                chestTiers[i].modelGlow.SetActive(false);
            }
        }

        chestObject.name = chestTiers[tier].chestName;

        // Add Selectable for making this object display an outline and its name
        selectable = chestObject.AddComponent<Selectable>();
        selectable.priority = Selectable.SelectablePriority.whenIsolated;
        selectable.type = Selectable.SelectableType.informational;

        // Add an Interactable for making this object be interacted with user's Interact press and from a distance
        interactable = chestObject.AddComponent<Interactable>();
        interactable.interactable = true;
        interactable.proximityRange = 3f;
        interactable.OnInteract.AddListener(OpenChest);
    }

    public void OpenChest()
    {
        ChestsManager.Instance.ItemDisplay.Display(new List<SO_Item>{ chestTiers[tier].reward}, DisposeChest);

        interactable.OnInteract.RemoveListener(OpenChest);
        Destroy(interactable);
        Destroy(selectable);
        Destroy(chestTiers[tier].modelGlow.gameObject);
        Instantiate(deathFX, transform.position, Quaternion.identity);
    }

    // This is called from closing the rewards screen, because we want player to see
    // chest is gone after viewing the reward screen
    public void DisposeChest()
    {
        Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
 
    
}
