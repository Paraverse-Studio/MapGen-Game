using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Block : MonoBehaviour
{
    [System.Serializable]
    public struct BlockOverrideSettings
    {
        public bool useOverrideColor;
        public SO_Color overrideSOColor;
        public Color overrideColor;
        public Vector3 overrideScale;
        public GameObject overridePrefab;
    }

    [Header("Block Item: ")]
    public SO_BlockItem type;
    private SO_BlockItem oldType = null;

    [Header("Override Settings: ")]
    public BlockOverrideSettings overrideSettings;   


    // PRIVATE //////////////////////////
    private string blockName = "";
    public string BlockName => blockName;

    [Space(20)]
    [TextArea(6, 10)]
    public string blockHistory = "";


    private TextMeshPro _display;

    private string _textToDisplay = "";
    public string TextToDisplay
    {
        set { _textToDisplay = value; }
    }

    private Renderer _renderer;
    private Collider _collider;
    private MaterialPropertyBlock _propBlock;

    private GameObject _currentPrefab = null;

    private Vector3 _targetPosition;
    public Vector3 TargetPosition
    {
        get { return _targetPosition; }
        set { _targetPosition = value; }
    }


    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        UpdateReferences();
        blockHistory = "Awake.";
    }

    private void UpdateReferences()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _collider = GetComponentInChildren<Collider>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        // SetupHudText();

        if (type)
        {
            UpdateBlock();
        }
    }

    private void Start()
    {
        ChangeName();
    }

    public void UpdateBlock()
    {
        if (!type) return;

        if (oldType != null && type == oldType) return;

        UpdateBlockItem();
        UpdateSize();
        gameObject.layer = type.layer.LayerIndex;
        if (_collider) _collider.gameObject.layer = type.layer.LayerIndex;

        oldType = type;    
    }

    private void ChangeName()
    {
        blockName = "Object " + Random.Range(1111, 9999);
    }

    private void SetupHudText()
    {
        GameObject displayObject = Instantiate(Resources.Load("HUDText", typeof(GameObject))) as GameObject;
        displayObject.transform.SetParent(GlobalSettings.Instance.uiFolder);
        displayObject.transform.localPosition = Vector3.zero;
        displayObject.GetComponent<FollowTarget>().target = transform;
        _display = displayObject.GetComponent<TextMeshPro>();

        GlobalSettings.Instance.OnToggleHudText.AddListener(ToggleText);
    }

    private void ToggleText()
    {
        _display.enabled = !_display.enabled;
    }

    private void UpdateBlockItem()
    {
        // Once models are in
        UpdateReferences();

        if (_renderer && (type.prefabVariations.Length == 0) && overrideSettings.overridePrefab == null)
        {
            if (overrideSettings.useOverrideColor)
            {
                // Get the current value of the material properties in the renderer
                _renderer.GetPropertyBlock(_propBlock);

                // Assign our new value
                _propBlock.SetColor("_BaseColor", type.blockColour);

                if (overrideSettings.overrideSOColor != null)
                {
                    _propBlock.SetColor("_BaseColor", overrideSettings.overrideSOColor.color);
                }
                else _propBlock.SetColor("_BaseColor", overrideSettings.overrideColor);

                // Apply the edited values to the renderer
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

        if (overrideSettings.overridePrefab || type.prefabVariations.Length > 0)
        {
            if (transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).parent = Pool.Instance.gameObject.transform;
            }
            if (_currentPrefab)
            {
                _currentPrefab.SetActive(false);
                _currentPrefab = null;
            }

            if (overrideSettings.overridePrefab)
            {
                _currentPrefab = Pool.Instance.Instantiate(overrideSettings.overridePrefab.name, Vector3.zero, Quaternion.identity, false);
            }
            else if (type.prefabVariations.Length > 0)
            {
                _currentPrefab = Pool.Instance.Instantiate(type.prefabVariations[Random.Range(0, type.prefabVariations.Length)].name, Vector3.zero, Quaternion.identity, false);
            }

            _currentPrefab.transform.SetParent(transform);

            _currentPrefab.transform.localPosition = new Vector3(0, 0.5f - 0.1f, 0);
            _currentPrefab.transform.localRotation = Quaternion.identity;
            _currentPrefab.transform.localScale = Vector3.one;

            UpdateHistory("Type changed to " + System.Enum.GetName(typeof(BlockType), (int)type.blockType));
        }

        UpdateReferences();

    }

    private void UpdateSize()
    {
        transform.localScale = type.defaultScale;
        if (overrideSettings.overrideScale != Vector3.zero)
        {
            if (transform.childCount > 0)
            {
                transform.GetChild(0).localScale = overrideSettings.overrideScale;
            }
            else
            {
                transform.localScale = overrideSettings.overrideScale;
            }
        }
    }

    public void UpdateHistory(string msg)
    {       
        string time = System.DateTime.Now.ToString("hh:mm:ss");
        blockHistory += "\n" + time + "  " + msg;
        
    }



}
