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
    [Header("References:")]
    public GameObject ShopWindow;

    [Header("Elements:")]
    public GameObject shopItemPrefab;
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

    // Start is called before the first frame update
    void Start()
    {
        _refresher = ShopWindow.GetComponent<ContentFitterRefresher>();
    }

    private void ClearShop()
    {
        foreach (Transform c in shopItemsFolder) Destroy(c.gameObject);
    }

    public void CalculateShopMods(int userCurrencyAmount, IEnumerable<SO_Mod> userCurrentMods)
    {
        // 1.0  Clear resources, and prepare
        ClearShop();
        _modPool.Clear();

        // 2.0  Refresh available mods list, sort them by their price
        AvailableMods.Sort((a, b) => a.Cost.CompareTo(b.Cost));

        // 3.0  Find user's currency index point: index of the highest priced item the user can buy
        int userCurrencyIndex = -1;
        for (int i = 0; i < AvailableMods.Count; ++i)
        {
            if (AvailableMods[i].Cost < userCurrencyAmount) userCurrencyIndex = userCurrencyAmount;
        }

        // 4.0  Then poll a couple of mods from that price point and below
        for (int i = userCurrencyIndex - 1; i >= 0; --i)
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
        List<ModPair> _modsToShow = (List<ModPair>)_modPool.Take(cardsQuantity);

        // 6.0  Show the finalized mods to the user
        for (int i = 0; i < cardsQuantity; ++i)
        {
            InstantiateModCard(_modsToShow[i]);
        }

        _refresher.RefreshContentFitters();
    }

    private void InstantiateModCard(ModPair modPair)
    {
        GameObject go = Instantiate(shopItemPrefab, shopItemsFolder);

        // Attach references here
    }

}
