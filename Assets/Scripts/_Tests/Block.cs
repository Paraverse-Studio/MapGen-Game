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

    [Header("Override Settings: ")]
    public BlockOverrideSettings overrideSettings;    

    private TextMeshPro _display;

    private string _textToDisplay = "";
    public string TextToDisplay
    {
        set { _textToDisplay = value; }
    }

    // private MeshRenderer;
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
    public void UpdateBlock()
    {
        UpdateBlockItem();
        UpdateSize();
        gameObject.layer = type.layer.LayerIndex;
        if (_collider) _collider.gameObject.layer = type.layer.LayerIndex;
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

    // Update is called once per frame
    void Update()
    {        
        if (_display)
        {
            _display.text = _textToDisplay;
        }        
    }

    private void ToggleText()
    {
        _display.enabled = !_display.enabled;
    }

    private void UpdateBlockItem()
    {
        // Once models are in
        UpdateReferences();

        if (_renderer)
        {
            // Get the current value of the material properties in the renderer
            _renderer.GetPropertyBlock(_propBlock);

            // Assign our new value
            _propBlock.SetColor("_BaseColor", type.blockColour);

            if (overrideSettings.useOverrideColor)
            {
                if (overrideSettings.overrideSOColor != null)
                {
                    _propBlock.SetColor("_BaseColor", overrideSettings.overrideSOColor.color);
                }
                else _propBlock.SetColor("_BaseColor", overrideSettings.overrideColor);
            }
            // Apply the edited values to the renderer
            _renderer.SetPropertyBlock(_propBlock);
        }


        if (overrideSettings.overridePrefab)
        {
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            if (_currentPrefab) Destroy(_currentPrefab);

            _currentPrefab = GameObject.Instantiate(overrideSettings.overridePrefab, Vector3.zero, Quaternion.identity);

            _currentPrefab.transform.SetParent(transform);

            _currentPrefab.transform.localPosition = new Vector3(0, 0.5f, 0);
            _currentPrefab.transform.localRotation = Quaternion.identity;
            _currentPrefab.transform.localScale = Vector3.one;
        }
        else if (type.prefabVariations.Length > 0)
        {
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            if (_currentPrefab) Destroy(_currentPrefab);

            _currentPrefab = GameObject.Instantiate(type.prefabVariations[Random.Range(0, type.prefabVariations.Length)], Vector3.zero, Quaternion.identity);

            _currentPrefab.transform.SetParent(transform);

            _currentPrefab.transform.localPosition = new Vector3(0, 0.5f, 0);
            _currentPrefab.transform.localRotation = Quaternion.identity;
            _currentPrefab.transform.localScale = Vector3.one;
        }       

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
}
