using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestObject : MonoBehaviour
{
    public enum ChestTierType
    {
        Common = 0, 
        Rare = 1, 
        Legendary = 2,
        RonnyChest = 3
    }

    [System.Serializable]
    public struct LootItem
    {
        public float chance;
        [Header("Mod")]
        public bool randomMod;
        public ModType modType;
        [Header("Consumable")]
        public bool randomConsumable;
        [MinMaxSlider(1f, 500f)]
        public Vector2 consumableAmount;
        [Header("Override item")]
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

    private ChestTier referenceChest;

    private Interactable _interactable;
    private Selectable _selectable;
    List<SO_Item> rewards = new();

    public void Initialize(ChestTierType tierProvided)
    {
        tier = Mathf.Clamp((int)tierProvided, 0, chestTiers.Length);

        // Set this object to the current tier provided (set its name and looks)
        for (int i = 0; i < chestTiers.Length; ++i)
        {            
            chestTiers[i].model.SetActive(false); // Set all to off first 
            chestTiers[i].modelGlow.SetActive(false);            
        }

        chestTiers[tier].model.SetActive(true);
        chestTiers[tier].modelGlow.SetActive(true);

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

        // The # of items you have to give to player
        int numToSpawn = Random.Range((int)chestTiers[tier].numToSpawn.x, (int)(chestTiers[tier].numToSpawn.y + 1));

        // Save a copy of this chest as a reference chest in case we empty this chest's loot table and still don't have
        // all the # of items to gift to player, then we refill the current chest with reference chest and go again
        ChestTier thisChest = chestTiers[tier];
        referenceChest = chestTiers[tier];

        int safetyCounter = 0;
        // Keep performing following algorithm until required num of reward items are calculated
        while (rewards.Count < numToSpawn)
        {
            // loot table logic
            safetyCounter++;
            if (safetyCounter > 100) { Debug.LogError("AB - HIT THE SAFETY COUNTER ON CHEST ALGORITHM!"); break; }

            // This would happen once we've gifted the player one of each item in the loot table,
            // then if we are to gift the player more items, refill this chest with reference chest to go again
            if (thisChest.lootTable.Count == 0) thisChest = referenceChest;

            float total = 0; // adding all the chances up (will likely always be 100)
            foreach (LootItem lootItem in thisChest.lootTable)
            {
                total += lootItem.chance;
            }

            float randomNumber = Random.Range(0, total);

            for (int i = 0; i < thisChest.lootTable.Count; ++i)
            {
                if (randomNumber <= thisChest.lootTable[i].chance)
                {
                    SO_Item decidedItem = thisChest.lootTable[i].overrideItem;

                    // by this point, an option from the loot table is decided to add to the list of rewards to give. 

                    // If no override SO_item is provided in the entry, then must be a random mod from randomMod property
                    if (null == decidedItem)
                    {
                        int indexOfMod = ModsManager.Instance.GetMod(thisChest.lootTable[i].modType, out SO_Mod returnedMod);
                        if (-1 == indexOfMod) continue;

                        decidedItem = returnedMod;

                        ModsManager.Instance.PurchaseMod(decidedItem);
                    }

                    // if it's a consumable type, modify its quantity
                    else if (decidedItem is SO_Consumable)
                    {
                        float amount = Random.Range(thisChest.lootTable[i].consumableAmount.x, thisChest.lootTable[i].consumableAmount.y);
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
                    thisChest.lootTable.RemoveAt(i);

                    break;
                }
                else
                {
                    randomNumber -= thisChest.lootTable[i].chance;
                }
            }       
        }

        // 2. Open the rewards UI and display the loot
        ChestsManager.Instance.ItemDisplay.Display(rewards, DisposeChest);

        _interactable.OnInteract.RemoveListener(OpenChest);
        _interactable.interactable = false;
        Destroy(_interactable);
        Destroy(_selectable);
        Destroy(thisChest.modelGlow.gameObject);
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
