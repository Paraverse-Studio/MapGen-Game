using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDisplayCreator : MonoBehaviour
{
    [SerializeField]
    private ContentFitterRefresher _refresher;

    [SerializeField]
    private Transform _container;

    [SerializeField]
    private ItemCard _itemCardPrefab;

    [SerializeField, Header("For custom card display")]
    private Transform _customCardContainer;

    [SerializeField]
    private ItemCard _customItemCardPrefab;

    [SerializeField, Header("Special properties")]
    private TextMeshProUGUI _goldText;

    [SerializeField, Header("Context message")]
    private TextMeshProUGUI _contextText;

    private List<SO_Item> _items;
    private System.Action _closeEvent;
    private List<ItemCard> _createdObjects = new();
    private MobStats _player;
    private ItemCard _customCard;

    private void OnEnable()
    {
        _player = GlobalSettings.Instance.player.GetComponent<MobStats>();
    }

    public void Display(List<SO_Item> items, System.Action closeCallback = null)
    {
        // if this screen is already open (a chest loot is being shown),
        // force the older instance to close, and provide the loot, and then show this one
        if (gameObject.activeSelf) gameObject.SetActive(false);

        if (null == _player) _player = GlobalSettings.Instance.player.GetComponent<MobStats>();

        _items = items;

        if (_container)
        {
            foreach (Transform c in _container)
            {
                if (null != c.gameObject) Destroy(c.gameObject);
            }

            for (int i = 0; i < _items.Count; ++i)
            {
                ItemCard card = Instantiate(_itemCardPrefab, _container);
                card.Item = _items[i];
                card.descriptionLabel = _contextText;
                card.UpdateDisplay();
                card.OnClickCard.AddListener(PurchaseItem);
                _createdObjects.Add(card);
            }
        }

        if (_goldText) _goldText.text = _player.Gold.ToString();

        RefreshDisplay();

        _closeEvent = closeCallback;
        gameObject.SetActive(true);
    }

    public void RefreshDisplay()
    {
        for (int i = 0; i < _createdObjects.Count; ++i)
        {
            if (null != _createdObjects[i])
            {                
                _createdObjects[i].Lock(_player.Gold < _createdObjects[i].Item.GetCost());
            }
        }
    }


    public void DisplayCustomCard(SO_Item item)
    {
        foreach (Transform c in _customCardContainer)
        {
            if (null != c.gameObject) Destroy(c.gameObject);
        }

        if (item)
        {
            ItemCard card = Instantiate(_customItemCardPrefab, _customCardContainer);
            card.Item = item;
            card.descriptionLabel = _contextText;
            card.UpdateDisplay();
            _createdObjects.Add(card);
        }
        gameObject.SetActive(true);
    }

    public void PurchaseItem(ItemCard item)
    {
        if (_player.Gold < item.Item.GetCost())
        {
            // cannot purchase it, notify user of insuffucient gold
            RefreshDisplay();
            return;
        }

        // Logistics
        Debug.Log($"Obtained item {item.Item.GetTitle()}!");

        _player.UpdateGold(-item.Item.GetCost());
        _goldText.text = _player.Gold.ToString();

        // the mod itself handles what the mod will do for the player when activated
        item.Item.Activate(_player.gameObject);
        item.Item.Consume();

        ModsManager.Instance.PurchaseMod(item.Item);

        Destroy(item.gameObject);

        RefreshDisplay();
    }

    public void OnDisable()
    {
        _closeEvent?.Invoke();
    }

}
