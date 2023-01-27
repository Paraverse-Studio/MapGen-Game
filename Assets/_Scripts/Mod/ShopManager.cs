using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using Paraverse.Mob.Stats;
using Paraverse.Player;

public class ShopManager : MonoBehaviour
{
    public struct ModPair
    {
        public SO_Mod mod;
        public int index;
    }

    #region public variables
    public static ShopManager Instance;

    [Header("References:")]
    public GameObject ShopWindow;
    public TextMeshProUGUI goldText;

    [Header("Elements:")]
    public ModCard shopItemPrefab;
    public Transform shopItemsFolder;

    [Header("Shop Algorithm:")]
    [Tooltip("Number of mod cards to display")]
    public int cardsQuantity;

    [Tooltip("From pool, grab this many items to randomize from")]
    public int pollQuantity;

    [Header("Mods To Buy:")]
    public List<SO_Mod> AvailableMods;

    [Header("Events:")]
    public UnityEvent OnPurchaseItem = new UnityEvent();
    #endregion

    #region private variables
    private ContentFitterRefresher _refresher;
    private List<ModPair> _modPool = new(); // used intermediate in the shop calculation
    private List<SO_Mod> _purhasedMods = new();
    private List<ModCard> _modCards = new();
    GameObject _player;
    MobStats _playerStats;

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _refresher = ShopWindow.GetComponent<ContentFitterRefresher>();
        _player = GlobalSettings.Instance.player;
        _playerStats = _player.GetComponentInChildren<MobStats>();

        // Load Mods from Resources folder
        Object[] loadedObjects = Resources.LoadAll("FinalizedMods", typeof(SO_Mod));
        AvailableMods.Clear();

        foreach (Object obj in loadedObjects)
        {
            AvailableMods.Add((SO_Mod)obj);
            AvailableMods[AvailableMods.Count - 1].Reset();
        }
    }

    private void ClearShop()
    {
        foreach (Transform c in shopItemsFolder) Destroy(c.gameObject);
        _modCards.Clear();
    }

    public void CalculateShopItems(int userCurrencyAmount, IEnumerable<SO_Mod> userCurrentMods)
    {
        Debug.Log("Shop Manager: Shop calculation invoked! Calculating new items. User gold: " + userCurrencyAmount);

        // 1.0  Clear resources
        if (null == AvailableMods || AvailableMods.Count == 0)
        {
            Debug.Log("Shop Manager: There are no available mods in the list.");
            return;
        }
        ClearShop();
        _modPool.Clear();
        for (int i = 0; i < AvailableMods.Count; ++ i)
        {
            if (null == AvailableMods[i]) AvailableMods.Remove(AvailableMods[i]);
            else
            {
                AvailableMods[i].AutofillDescription();
            }
        }

        // 2.0  Refresh available mods list, sort them by their price
        goldText.text = _playerStats.Gold.ToString();
        AvailableMods.Sort((a, b) => a.GetCost().CompareTo(b.GetCost()));

        // 3.0  Find user's currency index point: index of the highest priced item the user can buy
        int userCurrencyIndex = -1;
        for (int i = 0; i < AvailableMods.Count; ++i)
        {
            if (AvailableMods[i].GetCost() < userCurrencyAmount) userCurrencyIndex = i;
        }

        // 4.0  Then poll a couple of mods from that price point and below
        for (int i = userCurrencyIndex; i >= 0; --i)
        {
            if (AvailableMods[i].CanPurchase(userCurrencyAmount, userCurrentMods))
            {
                _modPool.Add(new ModPair { mod = AvailableMods[i], index = i });
            }
            if (_modPool.Count >= pollQuantity) break;
        }

        Debug.Log("Mod Pool here has " + _modPool.Count);

        // 5.0  From the polled amount, randomly pick the mods to show on shop
        System.Random rand = new();
        _modPool = _modPool.OrderBy(_ => rand.Next()).ToList();

        List<ModPair> _modsToShow = new();
        int availableModPool = Mathf.Min(cardsQuantity, _modPool.Count);

        for (int i = 0; i < availableModPool; ++i)
        {
            if (null != _modPool[i].mod) _modsToShow.Add(_modPool[i]);
        }

        // 6.0  Show the finalized mods to the user
        for (int i = 0; i < _modsToShow.Count; ++i)
        {
            InstantiateModCard(_modsToShow[i]);
        }

        _refresher.RefreshContentFitters();
    }

    private void InstantiateModCard(ModPair modPair)
    {
        ModCard modCard = Instantiate(shopItemPrefab, shopItemsFolder);
        modCard.Mod = modPair.mod;
        modCard.purchaseButton.onClick.AddListener(() => OnClickPurchaseItem(modCard, modPair));
        _modCards.Add(modCard);
    }

    // Iterates over displayed Mods in the shop, and refreshes their buy-ability
    // will disable a mod's button if its cost is greater than user's currency
    private void RefreshShopItemValues()
    {
        for (int i = 0; i < _modCards.Count; ++i)
        {
            if (null == _modCards[i]) continue;

            if (_playerStats.Gold < System.Int32.Parse(_modCards[i].costLabel.text))
            {
                _modCards[i].costLabel.color = Color.red;
                _modCards[i].cardLock.SetActive(true);
            }
        }
    }

    public void OnClickPurchaseItem(ModCard modCard, ModPair shopItem)
    {
        if (_playerStats.Gold < shopItem.mod.GetCost())
        {
            // cannot purchase it, notify user of insuffucient gold
            return;
        }

        // Logistics
        Debug.Log($"Purchased item {AvailableMods[shopItem.index].GetTitle()}!");

        _playerStats.UpdateGold(-shopItem.mod.GetCost());
        goldText.text = _playerStats.Gold.ToString();

        _purhasedMods.Add(AvailableMods[shopItem.index]);

        // the mod itself handles what the mod will do for the player when activated
        shopItem.mod.Activate(_player);

        SO_Mod returnValue = shopItem.mod.Consume();
        // normally returnValue is null cause u bought the mod, so its gone from Available
        AvailableMods[shopItem.index] = returnValue; 

        Destroy(modCard.gameObject);
        OnPurchaseItem?.Invoke();

        RefreshShopItemValues();
        _refresher.RefreshContentFitters();
    }

}
