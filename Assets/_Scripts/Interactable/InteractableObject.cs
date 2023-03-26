using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject objectModel;

    [SerializeField]
    private InteractableObjects thisInteractable;

    private Interactable _interactable;
    private Selectable _selectable;

    private bool _initialized = false;
    private ItemDisplayCreator _display;
    private List<SO_Item> _items = null;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Add Selectable for making this object display an outline and its name
        _selectable = objectModel.AddComponent<Selectable>();
        _selectable.priority = Selectable.SelectablePriority.whenIsolated;
        _selectable.type = Selectable.SelectableType.informational;

        // Add an Interactable for making this object be interacted with user's Interact press and from a distance
        _interactable = objectModel.AddComponent<Interactable>();
        _interactable.interactable = true;
        _interactable.proximityRange = 3f;
        _interactable.OnInteract.AddListener(InteractWithObject);
    }


    public void InteractWithObject()
    {
        _display = InteractableObjectsManager.Instance.windows.Find(x => x.type == thisInteractable).display;

        if (thisInteractable == InteractableObjects.spellbookMaster)
        {
            // Displaying left side: list of available skills to buy
            if (null == _items)
            {
                _items = ModsManager.Instance.AvailableMods.Where(mod => (mod is SO_SkillMod && !ModsManager.Instance.PurchasedMods.Contains(mod))).ToList();
                IListExtensions.Shuffle(_items);

                for (int i = _items.Count - 1; i >= 3; --i) _items.RemoveAt(i);
            }

            for (int i = _items.Count - 1; i >= 0; --i) if (ModsManager.Instance.PurchasedMods.Contains(_items[i])) _items.RemoveAt(i);
            _display.Display(_items, null);

            // Displaying right side: show the latest purchased skill you have
            SO_Item latestSkill = null;
            for (int i = ModsManager.Instance.PurchasedMods.Count - 1; i >= 0; --i)
            {
                if (null != ModsManager.Instance.PurchasedMods[i] && ModsManager.Instance.PurchasedMods[i] is SO_SkillMod)
                {
                    latestSkill = ModsManager.Instance.PurchasedMods[i];
                }
            }
            _display.DisplayCustomCard(latestSkill);

        }
        else if (thisInteractable == InteractableObjects.merchant)
        {
            if (null == _items)
            {
                _items = ModsManager.Instance.AvailableMods.Where(mod => mod is SO_EffectMod).ToList();
                IListExtensions.Shuffle(_items);
                for (int i = 4; i < _items.Count; ++i) _items.RemoveAt(i);
            }
            _display.Display(_items, null);
        }
        else if (thisInteractable == InteractableObjects.trainer)
        {
            if (null == _items)
            {
                _items = ModsManager.Instance.AvailableMods.Where(mod => mod is SO_StatMod).ToList();
                IListExtensions.Shuffle(_items);
                for (int i = 4; i < _items.Count; ++i) _items.RemoveAt(i);
            }
            _display.Display(_items, null);
        }
        else if (thisInteractable == InteractableObjects.skillGiver)
        {
            _display.Display(null, ()=>
                {
                    var chest = Instantiate(MapCreator.Instance.chestPrefab, 
                        transform.position + new Vector3(0, 0.5f, 0) + transform.forward, 
                        UtilityFunctions.GetCameraFacingRotation(reverse: true));
                    chest.Initialize(3); // skill chest
                    Destroy(_interactable);
                });
        }

        _initialized = true;

    }

    public void ResetInteractable()
    {
        _initialized = false;
        _items = null;
    }

}
