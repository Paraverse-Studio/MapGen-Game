using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Paraverse.Mob.Stats;
using TMPro;
using Paraverse.Mob.Controller;

public class MobHealthBar : MonoBehaviour
{
    [System.Serializable]
    public struct Override
    {
        public Image healthBar;
        public Image damageBar;
        public TextMeshProUGUI healthValueDisplay;
        public TextMeshProUGUI nameLabel;
        public Image energyBar;
        public Image energyLerpBar;
    }

    [System.Serializable]
    public struct Settings
    {
        public bool showWhenSelected;
        public bool hideName;
        public bool hideHealthBar;
        public bool showSelectionAllTimes; // this one's not even used
        public bool showEnergyBar;
        public float heightAdjustment;
    }

    [Header("Health Bar UI")]
    public Transform bodyToFollow;

    public Settings settings;
    public Override overrideProperties;
    private GameObject _healthBarPrefab;
    private Transform _healthBarsFolder;
    private GameObject _healthBarObject;
    private MobStats _mobStats;
    private Selectable _selectable;
    private bool _healthBarSetupComplete = false;

    // local copies (for lerping without constant reference)
    private float _health = -1.0f;
    private float _totalHealth = -1.0f;
    private float _energy = 1.0f;
    private float _totalEnergy = 2.0f;

    private Image _healthBar;
    private Image _healthDamageBar;
    private TextMeshProUGUI _nameLabel;
    private GameObject _targetIcon;
    private TextMeshProUGUI _healthValueDisplay;
    private Image _energyBar;
    private Image _energyLerpBar;

    // Start is called before the first frame update
    void Start()
    {
        if (!bodyToFollow) bodyToFollow = transform;

        _healthBarsFolder = GlobalSettings.Instance.healthBarFolder;
        _healthBarPrefab = GlobalSettings.Instance.healthBarPrefab;

        gameObject.AddComponent<MobHealthFlash>();

        if (overrideProperties.healthBar == null)
        {
            CreateHealthBar();
        }
        else 
        {
            LoadCustomHealthBar();
        }        

        if (TryGetComponent(out _mobStats))
        {
            _mobStats.OnHealthChange.AddListener(UpdateHealthBar);
            _mobStats.OnEnergyChange.AddListener(UpdateEnergyBar);
        }

        if (TryGetComponent(out _selectable))
        {
            _selectable.OnSelected.AddListener(ActivateTargetIcon);
            _selectable.OnDeselected.AddListener(DeactivateTargetIcon);
        }
    }

    private void ResetHealth()
    {
        _health = _totalHealth;
        if (null != _healthBarObject) UpdateHealthBar();
        if (null != _healthBarObject) UpdateEnergyBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_healthBarSetupComplete) return;
        if (!_mobStats) DestroyImmediate(gameObject);

        // Health
        _healthBar.fillAmount = Mathf.Clamp(_health / _totalHealth, 0f, 1f);
        _healthDamageBar.fillAmount = Mathf.Lerp(_healthDamageBar.fillAmount, _healthBar.fillAmount, Time.deltaTime * 2f);


        // Energy
        float goalAmount = _energy / _totalEnergy;
        if (_energyBar.fillAmount < goalAmount) _energyBar.fillAmount = Mathf.Lerp(_energyBar.fillAmount, goalAmount, Time.deltaTime * 10f);
        else _energyBar.fillAmount = goalAmount;

        _energyLerpBar.fillAmount = Mathf.Lerp(_energyLerpBar.fillAmount, _energyBar.fillAmount, Time.deltaTime * 2f);
    }

    // main updater
    public void UpdateHealthBar(int currentHP = -1, int totalHP = -1)
    {
        int healthChange = (int)_health - currentHP;
        if (_health == -1f) healthChange = totalHP - currentHP;

        _health = (float)currentHP;
        _totalHealth = (float)totalHP;

        if (_healthValueDisplay) _healthValueDisplay.text = currentHP + " / " + totalHP;

        if (!_healthBarSetupComplete) return;

        PopupTextOnHealthChange(healthChange);

    }

    public void PopupTextOnHealthChange(int healthChange)
    {
        if (!GlobalSettings.Instance.ScreenSpaceCanvas)
        {
            Debug.Log("Health Bar: No screen space canvas provided in Global Settings to create pop up text!");
            return;
        }
        GameObject popupObj = Instantiate(GlobalSettings.Instance.popupTextPrefab, GlobalSettings.Instance.ScreenSpaceCanvas.transform);
        float xOffset = Random.Range(-0.25f, 0.25f);
        float zOffset = Random.Range(-0.25f, 0.25f);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(bodyToFollow.transform.position + new Vector3(xOffset, 0.5f, zOffset));
        popupObj.transform.position = new Vector3(screenPosition.x, screenPosition.y, 0);

        TextMeshProUGUI textObj = popupObj.GetComponentInChildren<TextMeshProUGUI>();
        textObj.text = Mathf.Abs(healthChange).ToString();
        textObj.color = (healthChange >= 0)? GlobalSettings.Instance.damageColour : GlobalSettings.Instance.healColour;
    }

    public void UpdateEnergyBar(int currentEnergy = 1, int totalEnergy = 1)
    {
        _energy = (float)currentEnergy;

        _totalEnergy = (float)totalEnergy;
    }

    private void CreateHealthBar()
    {
        _healthBarObject = Instantiate(_healthBarPrefab, transform.position, Quaternion.identity);
        HealthBarViewController controller = _healthBarObject.GetComponent<HealthBarViewController>();
        _healthBar = controller.healthBar;
        _healthDamageBar = controller.damageBar;
        _energyBar = controller.energyBar;
        _energyLerpBar = controller.energyLerpBar;
        _targetIcon = controller.targetIcon;
        _nameLabel = controller.nameLabel;

        // Custom for Player's HP
        if (null != overrideProperties.healthBar)
        {
            _healthBar = overrideProperties.healthBar;
            _healthDamageBar = overrideProperties.damageBar;
            _healthValueDisplay = overrideProperties.healthValueDisplay;
            _energyBar = overrideProperties.energyBar;
            _energyLerpBar = overrideProperties.energyLerpBar;
            _nameLabel = overrideProperties.nameLabel;
        }

        // Settings
        controller.healthBarContainer.gameObject.SetActive(!settings.hideHealthBar);
        controller.nameLabel.gameObject.SetActive(!settings.hideName);
        controller.targetIcon.SetActive(settings.showSelectionAllTimes);
        controller.energyBarContainer.gameObject.SetActive(settings.showEnergyBar);

        // Find this object's bounds height, using either a collider or mesh renderer
        Collider collider = GetComponentInChildren<Collider>();
        Renderer renderer = GetComponentInChildren<Renderer>();

        float height = renderer ? GetEncapsulatedBounds(gameObject).size.y : collider.bounds.size.y;
        height += settings.heightAdjustment;

        _nameLabel.text = gameObject.name;
        _healthBarObject.transform.SetParent(_healthBarsFolder);
        FollowTarget ft = _healthBarObject.GetComponent<FollowTarget>();
        ft.target = bodyToFollow;
        ft.offset = new Vector3(0, height, 0);

        _healthBarSetupComplete = true;

        // Turn off the HP Huds for those that only show it when selected
        if (settings.showWhenSelected) StartCoroutine(DoAfterDelay(0.2f, () => _healthBarObject.gameObject.SetActive(false))); 
    }

    private IEnumerator DoAfterDelay(float f, System.Action action)
    {
        yield return new WaitForSeconds(f);
        action?.Invoke();
    }

    private void LoadCustomHealthBar()
    {        
        _healthBar = overrideProperties.healthBar;
        _healthDamageBar = overrideProperties.damageBar;
        _healthValueDisplay = overrideProperties.healthValueDisplay;
        _energyBar = overrideProperties.energyBar;
        _energyLerpBar = overrideProperties.energyLerpBar;
        _nameLabel = overrideProperties.nameLabel;

        _healthBarSetupComplete = true;
    }

    public void ActivateTargetIcon()
    {
        if (_targetIcon)
        {
            _targetIcon.SetActive(true);
            if (settings.showWhenSelected) _healthBarObject.gameObject.SetActive(true);
        }
    }

    public void DeactivateTargetIcon()
    {
        if (_targetIcon)
        {
            _targetIcon.SetActive(false);
            if (settings.showWhenSelected) _healthBarObject.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (_healthBarObject) _healthBarObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_healthBarObject) _healthBarObject.SetActive(true);        
    }

    private void OnDestroy()
    {
        Destroy(_healthBarObject);
    }

    public void ToggleHealthBarReady(bool o)
    {
        _healthBarSetupComplete = o;
    }

    #region HELPER_FUNCTIONS
    private Bounds GetEncapsulatedBounds(GameObject go)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer r;
        if (bounds.extents.x == 0)
        {
            bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Transform child in go.transform)
            {
                if (child.TryGetComponent(out r))
                {                 
                    bounds.Encapsulate(r.bounds);
                }
                else
                {
                    bounds.Encapsulate(GetEncapsulatedBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }
    #endregion

}
