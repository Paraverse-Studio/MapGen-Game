using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplayCreator : MonoBehaviour
{
    [SerializeField]
    private ContentFitterRefresher _refresher;

    [SerializeField]
    private Transform _container;

    [SerializeField]
    private ItemCard _itemCardPrefab;

    private GameObject _player;
    private List<SO_Item> _items;
    private System.Action _closeEvent;
    private List<GameObject> _createdObjects = new();

    public void Display(List<SO_Item> items, System.Action closeCallback)
    {
        // if this screen is already open (a chest loot is being shown),
        // force the older instance to close, and provide the loot, and then show this one
        if (gameObject.activeSelf) gameObject.SetActive(false);

        _items = items;
        _player = GlobalSettings.Instance.player;
        foreach (Transform c in _container)
        {
            if (null != c.gameObject) Destroy(c.gameObject);
        }

        for (int i = 0; i < _items.Count; ++i)
        {
            ItemCard card = Instantiate(_itemCardPrefab, _container);
            card.Item = _items[i];
            card.UpdateDisplay();
            _createdObjects.Add(card.gameObject);
        }        

        _closeEvent = closeCallback;
        gameObject.SetActive(true);
        //if (_refresher) _refresher.RefreshContentFitters();
    }

    private void GiveItemsToPlayer()
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            Debug.Log($"Obtained item {_items[i].GetTitle()}!");
            _items[i].Activate(_player);            
        }
    }

    // With this design, we want to give player the rewards when the window is closed.
    // in case any obtaining of any item shows a VFX.
    public void OnDisable()
    {
        GiveItemsToPlayer();

        for (int i = _createdObjects.Count - 1; i > 0; --i)
        {
            Destroy(_createdObjects[i]);
        }

        _closeEvent?.Invoke();
    }

}
