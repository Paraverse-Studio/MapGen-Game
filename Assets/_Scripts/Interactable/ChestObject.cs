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
        RonnyChest = 3,
        Mystic = 4
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

        public ChestTier Clone()
        {
            ChestTier newChest = new();
            newChest.chestName = chestName;
            newChest.model = model;
            newChest.modelGlow = modelGlow;
            newChest.numToSpawn = numToSpawn;
            newChest.lootTable = new List<LootItem>();
            for (int i = 0; i < lootTable.Count; ++i)
            {
                newChest.lootTable.Add(lootTable[i]);
            }
            return newChest;
        }
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
    List<SO_Mod> rewardsThatAreStats = new();

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
        //_selectable = chestObject.AddComponent<Selectable>();
        //_selectable.priority = Selectable.SelectablePriority.whenIsolated;
        //_selectable.type = Selectable.SelectableType.informational;

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
        ChestTier thisChest = chestTiers[tier].Clone();

        // Since the SO_Consumable Gold doesn't really make sense as a ScriptableObject since whatever amount value you 
        // put on it, it'll save that to the Unity files, we will reset it to 0 here, so that we can customize it for this chest
        // We need to do this for all future quantity type consumable items we add to chests
        ModsManager.Instance.GoldItem.Quantity = 0;

        int safetyCounter = 0;
        // Keep performing following algorithm until required num of reward items are calculated
        while (rewards.Count < numToSpawn)
        {
            // loot table logic
            safetyCounter++;
            if (safetyCounter > 100) { break; }

            // This would happen once we've gifted the player one of each item in the loot table,
            // then if we are to gift the player more items, refill this chest with reference chest to go again
            if (thisChest.lootTable.Count == 0) 
            { 
                thisChest = chestTiers[tier].Clone();
            }

            float total = 0; // adding all the chances up (will likely always be 100 AT FIRST, but after items are pulled out, it lessens)
            foreach (LootItem lootItem in thisChest.lootTable)
            {
                total += lootItem.chance;
            }

            float randomNumber = Random.Range(0, total);

            for (int i = thisChest.lootTable.Count - 1; i >= 0; --i)
            {
                if (randomNumber <= thisChest.lootTable[i].chance)
                {
                    SO_Item decidedItem = thisChest.lootTable[i].overrideItem;

                    // by this point, an option from the loot table is decided to add to the list of rewards to give. 

                    // NO OVERRIDE, RANDOM MOD: If no override SO_item is provided in the entry, then must be a random mod from random Mod property
                    if (null == decidedItem)
                    {
                        int indexOfMod = ModsManager.Instance.GetMod(thisChest.lootTable[i].modType, out SO_Mod returnedMod, avoidMods: rewardsThatAreStats);
                        if (-1 == indexOfMod)
                        {
                            // if getting this type of mod returned nothing, it means there's no mods left of this type,
                            // so remove this loot item from table so we don't keep searching for it, move onto the other 
                            // possible loot items
                            thisChest.lootTable.RemoveAt(i);
                            continue;
                        }

                        decidedItem = returnedMod;

                        ModsManager.Instance.PurchaseMod(decidedItem);
                    }

                    // OVERRIDE CONSUMABLE: if it's a consumable type, modify its quantity
                    else if (decidedItem is SO_Consumable)
                    {
                        float amount = Random.Range(thisChest.lootTable[i].consumableAmount.x, thisChest.lootTable[i].consumableAmount.y);

                        if (amount > 1)
                        {
                            // if something has more than 1 quantity, we can provide more of it if it loots twice,
                            // and then also remove its previous appearance, so we don't get 2 appearances of it, instead we get one with x2 (example)
                            if (decidedItem.Quantity > 0)
                            {
                                rewards.Remove(rewards.Find(rewardItem => rewardItem.ID == decidedItem.ID));
                                numToSpawn--;
                            }
                            decidedItem.Quantity += (int)amount; 
                        }
                        else // if this is a consumable that only has 1 of it, then we likely don't want to give player 2 appearances of it
                        {
                            chestTiers[tier].lootTable.Remove(chestTiers[tier].lootTable.Find(lootItem => lootItem.overrideItem == thisChest.lootTable[i].overrideItem));
                        }
                    }

                    // OVERRIDE MOD: If it's a mod, remove it from available pool, and add to purchased pool (since mods are unique and limited)
                    else if (decidedItem is SO_Mod)
                    {
                        ModsManager.Instance.PurchaseMod(decidedItem);
                    }

                    // finally, all tuning of the reward item are done, so add it to the rewards list
                    rewards.Add(decidedItem);
                    if (decidedItem is SO_StatMod) rewardsThatAreStats.Add((SO_StatMod)decidedItem);

                    // we chose this loot item to add, so remove it from loot table cause we don't want repeat reward items on first loop
                    thisChest.lootTable.RemoveAt(i);

                    break;
                }
                else
                {
                    // part of the loot table magic, by removing this item's chance, we can decide on a random item faster 
                    //randomNumber -= thisChest.lootTable[i].chance;
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
            var item = rewards[i];

            // Effect Mods Limiter
            if (item is SO_EffectMod effectMod && ModsManager.Instance.PurchasedMods.Count(mod => mod is SO_EffectMod) > ModsManager.MaxEffectMods)
            {
                // need to do this cause chestObj already adds the item to purchased list before we even get here
                ModsManager.Instance.PurchasedMods.RemoveAll(mod => mod.ID == item.ID);

                var effectsReplacer = FindObjectOfType<EffectsReplacer>(true);
                effectsReplacer.Display(ModsManager.Instance.PurchasedMods.Where(mod => mod is SO_EffectMod).ToList(),
                    () =>
                    {
                        int modLevelToActivate = -1;
                        if (item is SO_Mod mod && mod.Activated)
                        {
                            modLevelToActivate = mod.ModLevel + 1;
                        }
                        item.Activate(GlobalSettings.Instance.player, modLevelToActivate);
                        item.Consume();

                        ModsManager.Instance.PurchaseMod(item);
                    },
                    () =>
                    {
                        // Do nothing
                    });
                effectsReplacer.DisplayCustomCard(item);
            }
            else
            {
                int modLevelToActivate = -1;
                if (item is SO_Mod mod && mod.Activated)
                {
                    modLevelToActivate = mod.ModLevel + 1;
                }
                item.Activate(GlobalSettings.Instance.player, modLevelToActivate);
                item.Consume();
            }
        }

        Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
 
    
}
