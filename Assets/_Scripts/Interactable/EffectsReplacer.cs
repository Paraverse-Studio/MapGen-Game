using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectsReplacer : TimeChanger
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

    [SerializeField]
    private Button _replaceButton;

    private List<SO_Item> _items;
    private List<ItemCard> _createdObjects = new();
    private MobStats _player;
    private ItemCard _customCard;
    private SO_Item _selectedItem = null;
    private System.Action _replaceAction;
    private System.Action _cancelAction;

    

    private void OnEnable()
    {
        _player = GlobalSettings.Instance.player.GetComponent<MobStats>();
        TimeManager.Instance.RequestTimeChange(this, 0f);
    }

    public void Display(List<SO_Item> items, System.Action onReplace = null, System.Action onCancel = null)
    {
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
                card.UpdateDisplay(null, -1);                
                card.OnClickCard.AddListener(SelectItem);

                _createdObjects.Add(card);
            }
        }

        UnselectAll();
        _replaceAction = onReplace;
        _cancelAction = onCancel;

        gameObject.SetActive(true);
        _replaceButton.interactable = false;
    }

    public void UnselectAll()
    {
        for (int i = 0; i < _createdObjects.Count; ++i)
        {
            if (null != _createdObjects[i])
            {
                _createdObjects[i].ToggleSelect(false);
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

    public void SelectItem(ItemCard item)
    {
        UnselectAll();
        item.ToggleSelect(true);
        _selectedItem = item.Item;
        _replaceButton.interactable = true;
    }

    public void OnDisable()
    {
        TimeManager.Instance.RequestTimeChange(this, 1f);
        _cancelAction?.Invoke();
    }

    public void Replace()
    {
        if (_selectedItem is SO_EffectMod effectMod)
        {
            effectMod.Deactivate();
        }
        ModsManager.Instance.PurchasedMods.RemoveAll(mod => mod.ID == _selectedItem.ID);

        _replaceAction?.Invoke();
        gameObject.SetActive(false);
    }

}
