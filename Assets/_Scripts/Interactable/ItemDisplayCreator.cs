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

    private System.Action _closeEvent;
    private List<GameObject> _createdObjects;

    public void Display(List<SO_Item> items, System.Action closeCallback)
    {
        foreach (Transform c in _container) Destroy(c.gameObject);

        for (int i = 0; i < items.Count; ++i)
        {
            ItemCard card = Instantiate(_itemCardPrefab, _container);
            card.Item = items[i];
            card.UpdateDisplay();
            _createdObjects.Add(card.gameObject);
        }        

        _closeEvent = closeCallback;
        gameObject.SetActive(true);
        if (_refresher) _refresher.RefreshContentFitters();
    }

    public void OnDisable()
    {
        for (int i = _createdObjects.Count - 1; i > 0; --i)
        {
            Destroy(_createdObjects[i]);
        }

        _closeEvent?.Invoke();
    }

}
