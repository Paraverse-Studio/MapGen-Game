using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Block : MonoBehaviour
{
    [Header("Block Item: ")]
    public SO_BlockItem type;

    [Header("Override Settings: ")]
    public bool useOverrideColor = false;
    public SO_Color overrideSOColor;
    public Color overrideColor;
    public Vector3 overrideScale;

    private TextMeshPro display;

    private string textToDisplay = "";
    public string TextToDisplay
    {
        set { textToDisplay = value; }
    }

    // private MeshRenderer;
    private Renderer _renderer;
    private Collider _collider;
    private MaterialPropertyBlock _propBlock;

    private Vector3 _targetPosition;
    public Vector3 TargetPosition
    {
        get { return _targetPosition; }
        set { _targetPosition = value; }
    }
    private bool reached = false;
    private Vector3 velocity;

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
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
    private void UpdateBlock()
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
        display = displayObject.GetComponent<TextMeshPro>();

        GlobalSettings.Instance.OnToggleHudText.AddListener(ToggleText);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            if (type) UpdateBlock();
            if (display)
            {
                display.text = textToDisplay;
            }
        }
    }

    private void ToggleText()
    {
        display.enabled = !display.enabled;
    }

    private void UpdateBlockItem()
    {        
        // Once models are in, 

        // Get the current value of the material properties in the renderer
        _renderer.GetPropertyBlock(_propBlock);

        // Assign our new value
        _propBlock.SetColor("_BaseColor", type.blockColour);

        if (useOverrideColor)
        {
            if (overrideSOColor != null)
            {
                _propBlock.SetColor("_BaseColor", overrideSOColor.color);
            }
            else _propBlock.SetColor("_BaseColor", overrideColor);
        }

        // Apply the edited values to the renderer
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void UpdateSize()
    {
        transform.localScale = type.defaultScale;
        if (overrideScale != Vector3.zero)
        {
            transform.localScale = overrideScale;
        }
    }
}
