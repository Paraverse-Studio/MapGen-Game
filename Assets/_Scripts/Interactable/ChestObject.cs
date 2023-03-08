using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ChestObject : MonoBehaviour
{
    [System.Serializable]
    public struct LootItem
    {
        public SO_Item item;
        public int amount;
        public float chance;
    }

    [System.Serializable]
    public struct ChestTier
    {
        public string chestName;
        public GameObject model;
        public GameObject modelGlow;
        [MinMaxSlider(0f, 5f)]
        public Vector2 numToSpawn;
        public List<LootItem> lootTable;
    }

    [SerializeField]
    private GameObject chestObject;
    [SerializeField]
    private ParticleSystem deathFX;
    [SerializeField]
    private int tier;
    [SerializeField]
    private ChestTier[] chestTiers;    

    private Interactable _interactable;
    private Selectable _selectable;

    // Start is called before the first frame update
    void Start()
    {
        if (tier == -1) tier = Random.Range(0, chestTiers.Length);

        Initialize(tier);
    }

    public void Initialize(int tierProvided)
    {
        tier = Mathf.Clamp(tierProvided, 0, chestTiers.Length);

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
        _selectable = chestObject.AddComponent<Selectable>();
        _selectable.priority = Selectable.SelectablePriority.whenIsolated;
        _selectable.type = Selectable.SelectableType.informational;

        // Add an Interactable for making this object be interacted with user's Interact press and from a distance
        _interactable = chestObject.AddComponent<Interactable>();
        _interactable.interactable = true;
        _interactable.proximityRange = 3f;
        _interactable.OnInteract.AddListener(OpenChest);
    }

    public void OpenChest()
    {
        // 1. Decide the loot
        List<SO_Item> rewards = new List<SO_Item> { chestTiers[tier].lootTable[0].item };

        // Generate loot randomly until you have all
        int numToSpawn = (int)Random.Range(chestTiers[tier].numToSpawn.x, chestTiers[tier].numToSpawn.y);

        // Keep performing following algorithm until required num of reward items are calculated
        while (rewards.Count < numToSpawn)
        {
            // loot table logic
            float total = chestTiers[tier].lootTable.Sum(lootItem => lootItem.chance);
            float randomNumber = Random.Range(0, total);

            for (int i = 0; i < chestTiers[tier].lootTable.Count; ++i)
            {
                if (randomNumber <= chestTiers[tier].lootTable[i].chance)
                {
                    SO_Item decidedItem = chestTiers[tier].lootTable[i].item;

                    // by this point, an item is decided to add to the list of rewards to give. 

                    // if it's a consumable type, modify its quantity
                    if (decidedItem is SO_Consumable)
                    {
                        
                    }

                    // If it's a mod,
                    // Remove this mod from available pool, and add to purchased pool (since mods are unique and limited)

                    // finally, all tuning of the reward item are done, so add it to the rewards list
                    rewards.Add(decidedItem);

                    // we chose this loot item to add, so remove it from loot table cause we don't want repeat reward items 
                    chestTiers[tier].lootTable.RemoveAt(i);

                    break;
                }
                else
                {
                    randomNumber -= chestTiers[tier].lootTable[i].chance;
                }
            }

            

        }

        // 2. Open the rewards UI and display the loot
        ChestsManager.Instance.ItemDisplay.Display(rewards, DisposeChest);

        _interactable.OnInteract.RemoveListener(OpenChest);
        Destroy(_interactable);
        Destroy(_selectable);
        Destroy(chestTiers[tier].modelGlow.gameObject);
    }

    // This is called from closing the rewards screen, because we want player to see
    // chest is gone after viewing the reward screen
    public void DisposeChest()
    {
        Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
 
    
}
