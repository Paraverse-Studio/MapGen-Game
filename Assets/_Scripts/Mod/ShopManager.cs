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
        public SO_Item item;
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
    public TextMeshProUGUI descriptionMessage;

    [Header("Shop Algorithm:")]
    [Tooltip("Number of mod cards to display")]
    public int cardsQuantity;

    [Tooltip("From pool, grab this many items to randomize from")]
    public int pollQuantity;

    [Header("Events:")]
    public UnityEvent OnPurchaseItem = new UnityEvent();
    #endregion

    #region private variables
    private ContentFitterRefresher _refresher;
    private List<ModPair> _modPool = new(); // used intermediate in the shop calculation

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
    }

    private void ClearShop()
    {
        foreach (Transform c in shopItemsFolder) Destroy(c.gameObject);
        _modCards.Clear();
    }

    public void CalculateShopItems(int userCurrencyAmount, IEnumerable<SO_Mod> userCurrentMods)
    {
        // 1.0  Clear resources, and ready available mods
        if (null == ModsManager.Instance.AvailableMods || ModsManager.Instance.AvailableMods.Count == 0)
        {
            Debug.Log("Shop Manager: There are no available mods in the list.");
            return;
        }
        ClearShop();
        _modPool.Clear();
        for (int i = 0; i < ModsManager.Instance.AvailableMods.Count; ++ i)
        {
            if (null == ModsManager.Instance.AvailableMods[i]) ModsManager.Instance.AvailableMods.RemoveAt(i);            
            else ModsManager.Instance.AvailableMods[i].AutofillDescription();            
        }

        // 2.0  Sort remaining available mods by their price
        goldText.text = _playerStats.Gold.ToString();
        ModsManager.Instance.AvailableMods.Sort((a, b) => a.GetCost().CompareTo(b.GetCost()));

        // 3.0  Find user's currency index point: index of the highest priced item the user can buy
        int userCurrencyIndex = -1;
        for (int i = 0; i < ModsManager.Instance.AvailableMods.Count; ++i)
        {
            if (ModsManager.Instance.AvailableMods[i].GetCost() <= userCurrencyAmount) userCurrencyIndex = i;
        }

        // 4.0  Then poll a couple of mods from that price point and below
        for (int i = userCurrencyIndex; i >= 0; --i)
        {
            if (ModsManager.Instance.AvailableMods[i].CanPurchase(userCurrencyAmount, userCurrentMods))
            {
                _modPool.Add(new ModPair { item = ModsManager.Instance.AvailableMods[i], index = i });
            }
            if (_modPool.Count >= pollQuantity) break;
        }

        // 5.0  From the polled amount, randomly pick the mods to show on shop
        System.Random rand = new();
        _modPool = _modPool.OrderBy(_ => rand.Next()).ToList();

        List<ModPair> _modsToShow = new();
        int availableModPool = Mathf.Min(cardsQuantity, _modPool.Count);

        for (int i = 0; i < availableModPool; ++i)
        {
            if (null != _modPool[i].item) _modsToShow.Add(_modPool[i]);
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
        modCard.Item = modPair.item;
        modCard.descriptionLabel = descriptionMessage;
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
        if (_playerStats.Gold < shopItem.item.GetCost())
        {
            // cannot purchase it, notify user of insuffucient gold
            return;
        }

        // Logistics
        Debug.Log($"Obtained item {ModsManager.Instance.AvailableMods[shopItem.index].GetTitle()}!");

        _playerStats.UpdateGold(-shopItem.item.GetCost());
        goldText.text = _playerStats.Gold.ToString();

        // the mod itself handles what the mod will do for the player when activated
        shopItem.item.Activate(_player);

        // Item-specific type code
        if (shopItem.item is SO_Mod)
        {
            // the following code handles refreshing the ModsManager.Instance.AvailableMods list 
            if (shopItem.item is not SO_StatMod) ModsManager.Instance.PurchasedMods.Add(ModsManager.Instance.AvailableMods[shopItem.index]);
            SO_Mod returnValue = shopItem.item.Consume();

            // normally returnValue is null cause u bought the mod, so its gone from Available
            ModsManager.Instance.AvailableMods[shopItem.index] = returnValue;
        }    

        Destroy(modCard.gameObject);
        OnPurchaseItem?.Invoke();

        RefreshShopItemValues();
        _refresher.RefreshContentFitters();
    }

}
