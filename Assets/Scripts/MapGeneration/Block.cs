using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Block : MonoBehaviour, ITickElement
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
    [TextArea(6, 15)]
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
    public GameObject CurrentPrefab
    {
        get { return _currentPrefab; }
        set { _currentPrefab = value; }
    }

    private void Awake()
    {        
        UpdateHistory("Created");
    }

    private void UpdateReferences()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _collider = GetComponentInChildren<Collider>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        TickManager.Instance?.Unsubscribe(this);

        if (type)
        {
            UpdateBlock();
        }
    }

    private void Start()
    {
        _propBlock = new MaterialPropertyBlock();
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

    // Not using Update(), because there's way too many blocks to have update on,
    // instead, if a block needs update, it'll subscribe to TickManager and get 
    // its tick called every frame
    public void Tick()
    {
        Vector3 newSpot = new Vector3(transform.position.x, 0, transform.position.z);
        _currentPrefab.transform.position = newSpot;
    }


    private void ChangeName()
    {
        blockName = "Object " + Random.Range(1111, 9999);
        UpdateHistory("Name: " + blockName);
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

    public void ToggleRenderer(bool onOrOff)
    {
        if (_renderer) _renderer.enabled = onOrOff;
    }


    public void UpdateBlockItem()
    {
        // Once models are in
        UpdateReferences();

        if (_renderer && (type.prefabVariations.Length == 0) && overrideSettings.overridePrefab == null)
        {
            if (overrideSettings.useOverrideColor)
            {
                if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

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
            // 1.0    Remove current block 
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


            // 2.0    Add new block
            if (overrideSettings.overridePrefab)
            {
                _currentPrefab = Pool.Instance.Instantiate(overrideSettings.overridePrefab.name, Vector3.zero, Quaternion.identity, false);
            }
            else if (type.prefabVariations.Length > 0)
            {
                _currentPrefab = Pool.Instance.Instantiate(type.prefabVariations[Random.Range(0, type.prefabVariations.Length)].name, Vector3.zero, Quaternion.identity, false);
            }


            // 3.0    New block's Settings
            bool isWaterType = false;
            if (MapGeneration.Instance) isWaterType = (type == MapGeneration.Instance.blocks.water);

            if (!isWaterType)
            {
                _currentPrefab.transform.SetParent(transform);
            }
            else
            {
                _currentPrefab.transform.SetParent(Pool.Instance.waterVolume.transform);
                TickManager.Instance?.Subscribe(this);
            }

            _currentPrefab.transform.localPosition = new Vector3(0, 0.5f - 0.1f, 0);
            _currentPrefab.transform.localRotation = Quaternion.identity;
            _currentPrefab.transform.localScale = Vector3.one;


            UpdateHistory("Type changed to " + System.Enum.GetName(typeof(BlockType), (int)type.blockType));
        }

        UpdateReferences();
        //if (_renderer) _renderer.enabled = false;
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
