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
        public float chance;
        public bool randomMod;
        public ModType modType;
        public bool randomConsumable;
        [MinMaxSlider(1f, 500f)]
        public Vector2 consumableAmount;
        public SO_Item overrideItem;
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
    List<SO_Item> rewards = new();

    // Start is called before the first frame update
    void Start()
    {
        //if (tier == -1) tier = Random.Range(0, chestTiers.Length);

        //Initialize(tier);
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

        // Generate loot randomly until you have all
        int numToSpawn = Random.Range((int)chestTiers[tier].numToSpawn.x, (int)(chestTiers[tier].numToSpawn.y + 1));

        int safetyCounter = 0;
        // Keep performing following algorithm until required num of reward items are calculated
        while (rewards.Count < numToSpawn)
        {
            // loot table logic
            safetyCounter++;
            if (safetyCounter > 100) break;
            float total = 0;
            foreach (LootItem lootItem in chestTiers[tier].lootTable)
            {
                total += lootItem.chance;
            }

            float randomNumber = Random.Range(0, total);

            for (int i = 0; i < chestTiers[tier].lootTable.Count; ++i)
            {
                if (randomNumber <= chestTiers[tier].lootTable[i].chance)
                {
                    SO_Item decidedItem = chestTiers[tier].lootTable[i].overrideItem;

                    // by this point, an option from the loot table is decided to add to the list of rewards to give. 

                    // If no SO_item is provided in the entry, then must be a random mod from randomMod property
                    if (null == decidedItem)
                    {
                        int indexOfMod = ModsManager.Instance.GetMod(chestTiers[tier].lootTable[i].modType, out SO_Mod returnedMod);
                        if (-1 == indexOfMod) continue;

                        decidedItem = returnedMod;

                        ModsManager.Instance.PurchaseMod(decidedItem);
                    }

                    // if it's a consumable type, modify its quantity
                    else if (decidedItem is SO_Consumable)
                    {
                        float amount = Random.Range(chestTiers[tier].lootTable[i].consumableAmount.x, chestTiers[tier].lootTable[i].consumableAmount.y);
                        decidedItem.Quantity = (int)amount;
                    }

                    // If it's a mod, remove it from available pool, and add to purchased pool (since mods are unique and limited)
                    else if (decidedItem is SO_Mod)
                    {
                        ModsManager.Instance.PurchaseMod(decidedItem);
                    }

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
        _interactable.interactable = false;
        Destroy(_interactable);
        Destroy(_selectable);
        Destroy(chestTiers[tier].modelGlow.gameObject);
    }

    // 3. Step 3 is when the player closes the menu, the mods are activated
    // This is called from closing the rewards screen, because we want player to see
    // chest is gone after viewing the reward screen
    public void DisposeChest()
    {
        for (int i = 0; i < rewards.Count; ++i)
        {
            rewards[i].Activate(GlobalSettings.Instance.player);
            rewards[i].Consume();
        }

        Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
 
    
}
