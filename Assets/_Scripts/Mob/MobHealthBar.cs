using Paraverse.Mob.Stats;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public bool isBoss;
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
    private bool _healthBarShowable = true;

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
    private GameObject _enragedMark;
    private GameObject _healthBarContainer;

    // Run-time
    private HealthBarViewController _healthBarController;

    // Start is called before the first frame update
    void Start()
    {
        if (!bodyToFollow) bodyToFollow = transform;

        _healthBarsFolder = GlobalSettings.Instance.healthBarFolder;
        _healthBarPrefab = GlobalSettings.Instance.healthBarPrefab;

        gameObject.AddComponent<MobHealthFlash>();

        if (overrideProperties.healthBar != null || settings.isBoss)
        {
            LoadCustomHealthBar();
        }
        else
        {
            CreateHealthBar();
        }

        if (TryGetComponent(out _mobStats))
        {
            _mobStats.OnHealthChange.AddListener(UpdateHealthBar);
            _mobStats.OnEnergyChange.AddListener(UpdateEnergyBar);
        }

        if (TryGetComponent(out _selectable))
        {
            _selectable.events.OnSelected.AddListener(ActivateTargetIcon);
            _selectable.events.OnDeselected.AddListener(DeactivateTargetIcon);
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
        if (!_healthBarSetupComplete || !_healthBarShowable) return;

        //if (Time.frameCount % 30 == 0) _nameLabel.text = gameObject.name;

        // Health
        if (_healthBar)
        {
            _healthBar.fillAmount = Mathf.Clamp(_health / _totalHealth, 0f, 1f);
            _healthDamageBar.fillAmount = Mathf.Lerp(_healthDamageBar.fillAmount, _healthBar.fillAmount, Time.deltaTime * 2f);
        }

        // Energy
        if (_energyBar)
        {
            float goalAmount = _energy / _totalEnergy;
            if (_energyBar.fillAmount < goalAmount) _energyBar.fillAmount = Mathf.Lerp(_energyBar.fillAmount, goalAmount, Time.deltaTime * 10f);
            else _energyBar.fillAmount = goalAmount;

            _energyLerpBar.fillAmount = Mathf.Lerp(_energyLerpBar.fillAmount, _energyBar.fillAmount, Time.deltaTime * 2f);
        }
    }

    // main updater
    public void UpdateHealthBar(int currentHP = -1, int totalHP = -1)
    {
        bool firstInvocation = false; // if this is the first time this function is called (to set up)

        int healthChange = (int)_health - currentHP;
        if (_health == -1f)
        {
            firstInvocation = true;
            healthChange = totalHP - currentHP;
        }

        _health = (float)currentHP;
        _totalHealth = (float)totalHP;

        if (_healthValueDisplay) _healthValueDisplay.text = currentHP + " / " + totalHP;

        if (healthChange == 0) return;
        if (!_healthBarSetupComplete || !_healthBarShowable) return;

        PopupTextOnHealthChange(healthChange);
    }

    public void PopupTextOnHealthChange(int healthChange)
    {
        if (!GlobalSettings.Instance.ScreenSpaceCanvas)
        {
            Debug.Log("Health Bar: No screen space canvas provided in Global Settings to create pop up text!");
            return;
        }

        PopupText popupObj = Instantiate(GlobalSettings.Instance.popupTextPrefab, GlobalSettings.Instance.ScreenSpaceCanvas.transform);
        float xOffset = Random.Range(-0.25f, 0.25f);
        float zOffset = Random.Range(-0.25f, 0.25f);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(bodyToFollow.transform.position + new Vector3(xOffset, 0.5f, zOffset));
        popupObj.transform.position = new Vector3(screenPosition.x, screenPosition.y, 0);

        TextMeshProUGUI textObj = popupObj.text;
        if (healthChange >= _totalHealth * 0.65f)
        {
            textObj.fontSize *= 1.55f;
        }
        else if (healthChange >= _totalHealth * 0.4f)
        {
            textObj.fontSize *= 1.25f;
        }
        else if (healthChange <= _totalHealth * 0.1f)
        {
            textObj.fontSize /= 1.25f;
        }
        textObj.text = Mathf.Abs(healthChange).ToString();
        textObj.color = (healthChange >= 0) ? GlobalSettings.Instance.damageColour : GlobalSettings.Instance.healColour;
    }

    public void UpdateEnergyBar(int currentEnergy = 1, int totalEnergy = 1)
    {
        _energy = (float)currentEnergy;
        _totalEnergy = (float)totalEnergy;
    }

    private void CreateHealthBar()
    {
        _healthBarObject = Instantiate(_healthBarPrefab, transform.position, Quaternion.identity);
        _healthBarController = _healthBarObject.GetComponent<HealthBarViewController>();
        _healthBar = _healthBarController.healthBar;
        _healthDamageBar = _healthBarController.damageBar;
        _energyBar = _healthBarController.energyBar;
        _energyLerpBar = _healthBarController.energyLerpBar;
        _targetIcon = _healthBarController.targetIcon;
        _nameLabel = _healthBarController.nameLabel;
        _healthBarContainer = _healthBarController.healthBarContainer;

        // Not needed? They're done in LoadCreateBar instead of CreateHealthBar
        if (false)
        {
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

            if (settings.isBoss)
            {
                BossHealthBarModel model = _healthBarsFolder.GetComponent<BossHealthBarModel>();
                _healthBar = model.healthBar;
                _healthDamageBar = model.damageBar;
                _healthValueDisplay = model.healthValueDisplay;
                _energyBar = model.energyBar;
                _energyLerpBar = model.energyLerpBar;
                _nameLabel = model.nameLabel;
                _healthBarContainer = model.healthBarContainer;
                _healthBarContainer.SetActive(true);
            }
        }

        // Settings
        _healthBarContainer.gameObject.SetActive(!settings.hideHealthBar);
        _nameLabel.gameObject.SetActive(!settings.hideName);
        _targetIcon.SetActive(settings.showSelectionAllTimes);
        _healthBarController.energyBarContainer.gameObject.SetActive(settings.showEnergyBar);

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
        // Custom for player (HUD health bar)
        if (overrideProperties.healthBar != null)
        {
            _healthBar = overrideProperties.healthBar;
            _healthDamageBar = overrideProperties.damageBar;
            _healthValueDisplay = overrideProperties.healthValueDisplay;
            _energyBar = overrideProperties.energyBar;
            _energyLerpBar = overrideProperties.energyLerpBar;
            //_nameLabel = overrideProperties.nameLabel;
            //_nameLabel.text = gameObject.name;
        }

        // custom for bosses (HUD health bar)
        if (settings.isBoss && _healthBarsFolder.TryGetComponent(out BossHealthBarModel model))
        {
            _healthBar = model.healthBar;
            _healthDamageBar = model.damageBar;
            _healthValueDisplay = model.healthValueDisplay;
            _energyBar = model.energyBar;
            _energyLerpBar = model.energyLerpBar;
            _nameLabel = model.nameLabel;
            _nameLabel.text = gameObject.name;
            _healthBarContainer = model.healthBarContainer;
            _healthBarContainer.SetActive(true);
        }

        _healthBarSetupComplete = true;
    }

    public void MarkEnragedMob()
    {
        if (_nameLabel)
        {
            _healthBarController.enragedMark.SetActive(true);
            _nameLabel.color = GlobalSettings.Instance.enragedNameColour;
        }
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
        if (_healthBarContainer) _healthBarContainer.SetActive(false);
        Destroy(_healthBarObject);
    }

    public void ToggleHealthBarReady(bool o)
    {
        _healthBarShowable = o;
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
