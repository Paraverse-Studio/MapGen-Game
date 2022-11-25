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

    private Renderer _renderer;
    private Collider _collider;

    private GameObject _currentPrefab = null;
    public GameObject CurrentPrefab
    {
        get { return _currentPrefab; }
        set { _currentPrefab = value; }
    }

    //private void Awake()
    //{        
    //    UpdateHistory("Created");
    //}

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
        ChangeName();
    }

    public void UpdateBlock()
    {
        if (!type) return;

        if (oldType != null && type == oldType) return;

        oldType = type;

        UpdateBlockItem();
        UpdateSize();
        gameObject.layer = type.layer.LayerIndex;
        SetLayerRecursively(gameObject, gameObject.layer);
        if (_collider) _collider.gameObject.layer = type.layer.LayerIndex;          
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
        blockName = "Block "; // + Random.Range(1111, 9999);
        UpdateHistory("Name: " + blockName);
    }


    public void ToggleRenderer(bool onOrOff)
    {
        if (_renderer) _renderer.enabled = onOrOff;
    }
    private void SetLayerRecursively(GameObject go, int layerNumber)
    {
        Transform[] c = go.GetComponentsInChildren<Transform>(true);
        foreach (Transform trans in c)
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public void UpdateBlockItem()
    {
        // Once models are in
        if (oldType == null) UpdateReferences();

        if (type.prefabVariations.Length > 0)
        {
            // 1.0    Remove current block 
            if (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);

                //transform.GetChild(0).gameObject.SetActive(false);
                //transform.GetChild(0).parent = Pool.Instance.gameObject.transform;
            }
            if (_currentPrefab)
            {
                DestroyImmediate(_currentPrefab);
                //_currentPrefab.SetActive(false);
                //_currentPrefab = null;
            }

            // 2.0    Add new block
            if (type.prefabVariations.Length > 0)
            {
                _currentPrefab = Instantiate(type.prefabVariations[Random.Range(0, type.prefabVariations.Length)], 
                    Vector3.zero, Quaternion.identity);
                UtilityFunctions.UpdateLODlevels(_currentPrefab.transform);
            }


            // 3.0    New block's Settings
            bool isWaterType = false;
            if (MapGeneration.Instance) isWaterType = (type == MapGeneration.Instance.M.blockSet.water);

            if (!isWaterType)
            {
                _currentPrefab.transform.SetParent(transform);
                CheckBoxCollider(_currentPrefab);
                _currentPrefab.isStatic = true;
            }
            else
            {
                _currentPrefab.isStatic = false;
                _currentPrefab.transform.SetParent(GlobalSettings.Instance.waterVolume.transform);
                TickManager.Instance?.Subscribe(this, gameObject);
            }

            _currentPrefab.transform.localPosition = new Vector3(0, 0, 0);
            _currentPrefab.transform.localRotation = Quaternion.identity;

            ApplyRandomRotation(_currentPrefab);

            //UpdateHistory("Type changed to " + System.Enum.GetName(typeof(BlockType), (int)type.blockType));
        }

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
        if (!GlobalSettings.Instance.recordBlockHistory) return;
        string time = System.DateTime.Now.ToString("hh:mm:ss");
        blockHistory += "\n" + time + "  " + msg;        
    }


    private void CheckBoxCollider(GameObject obj)
    {        
        if (!obj.GetComponent<BoxCollider>())
        {
            obj.AddComponent<BoxCollider>();
        }
    }

    private void ApplyRandomRotation(GameObject obj)
    {
        int x = 0, y = 0, z = 0;
        if (type.rotationRandomization.x)
        {
            int random = Random.Range(0, 4);
            x = (random * 90);
        }
        if (type.rotationRandomization.y)
        {
            int random = Random.Range(0, 4);
            y = (random * 90);
        }
        if (type.rotationRandomization.z)
        {
            int random = Random.Range(0, 4);
            z = (random * 90);
        }
        obj.transform.localRotation = Quaternion.Euler(x,y,z);
    }

    private void OnDestroy()
    {
        TickManager.Instance?.Unsubscribe(this);

        if (_currentPrefab) DestroyImmediate(_currentPrefab);
    }

}
