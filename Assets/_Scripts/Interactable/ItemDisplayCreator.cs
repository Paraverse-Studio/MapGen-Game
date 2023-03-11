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


    [SerializeField, Header("Context message")]
    private TextMeshProUGUI _contextText;

    private List<SO_Item> _items;
    private System.Action _closeEvent;
    private List<GameObject> _createdObjects = new();

    private ItemCard _customCard;
    

    public void Display(List<SO_Item> items, System.Action closeCallback = null)
    {
        // if this screen is already open (a chest loot is being shown),
        // force the older instance to close, and provide the loot, and then show this one
        if (gameObject.activeSelf) gameObject.SetActive(false);

        _items = items;
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
            _createdObjects.Add(card.gameObject);
        }

        _closeEvent = closeCallback;
        gameObject.SetActive(true);
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
            _createdObjects.Add(card.gameObject);
        }
        gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        for (int i = _createdObjects.Count - 1; i > 0; --i)
        {
            if (null != _createdObjects[i]) Destroy(_createdObjects[i]);
        }

        _closeEvent?.Invoke();
    }

}
