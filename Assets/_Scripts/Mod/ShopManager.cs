using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    #endregion

    #region private variables
    private ContentFitterRefresher _refresher;
    private List<ModPair> _modPool = new();
    private List<SO_Mod> _purhasedMods = new();

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _refresher = ShopWindow.GetComponent<ContentFitterRefresher>();
    }

    private void ClearShop()
    {
        foreach (Transform c in shopItemsFolder) Destroy(c.gameObject);
    }

    public void CalculateShopItems(int userCurrencyAmount, IEnumerable<SO_Mod> userCurrentMods)
    {
        // 1.0  Clear resources
        ClearShop();
        _modPool.Clear();

        // 2.0  Refresh available mods list, sort them by their price
        AvailableMods.Sort((a, b) => a.Cost.CompareTo(b.Cost));

        // 3.0  Find user's currency index point: index of the highest priced item the user can buy
        int userCurrencyIndex = -1;
        for (int i = 0; i < AvailableMods.Count; ++i)
        {
            if (AvailableMods[i].Cost < userCurrencyAmount) userCurrencyIndex = i;
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

        // 5.0  From the polled amount, randomly pick the amount of mods to show on shop
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
        modCard.titleLabel.text = modPair.mod.Title;
        modCard.imageHolder.sprite = modPair.mod.Image;
        modCard.descriptionLabel.text = modPair.mod.Description;
        modCard.costLabel.text = modPair.mod.Cost.ToString();

        modCard.purchaseButton.onClick.AddListener(() => OnClickPurchaseItem(modCard.gameObject, modPair.index));
    }

    public void OnClickPurchaseItem(GameObject modCard, int shopItemIndex)
    {
        Debug.Log("Purchased item: " + AvailableMods[shopItemIndex].Title);
        _purhasedMods.Add(AvailableMods[shopItemIndex]);
        AvailableMods.Remove(AvailableMods[shopItemIndex]);
        Destroy(modCard);
        _refresher.RefreshContentFitters();
    }

}
