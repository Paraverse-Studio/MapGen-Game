using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TestBlock : MonoBehaviour
{
    public TestBlockType type;

    private TextMeshPro display;

    private string textToDisplay = "";
    public string TextToDisplay
    {
        set { textToDisplay = value; }
    }

    // private MeshRenderer;
    private Renderer _renderer;
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
        _renderer = GetComponent<Renderer>();
    }

    private void UpdateType()
    {
        UpdateColour();
        UpdateSize();
        gameObject.layer = type.layer.LayerIndex;
    }

    // Start is called before the first frame update
    void Start()
    {        
        // SetupHudText();

        UpdateType();
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
        if (display)
        {
            display.text = textToDisplay;
        }
    }

    private void ToggleText()
    {
        display.enabled = !display.enabled;
    }

    private void UpdateColour()
    {        
        // Get the current value of the material properties in the renderer
        _renderer.GetPropertyBlock(_propBlock);

        // Assign our new value
        _propBlock.SetColor("_BaseColor", type.blockColour);

        // Apply the edited values to the renderer
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void UpdateSize()
    {
        transform.localScale = type.defaultScale;
    }
}
