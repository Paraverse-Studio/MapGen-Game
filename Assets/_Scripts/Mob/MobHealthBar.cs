using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Paraverse.Mob.Stats;
using TMPro;
using Paraverse.Mob.Controller;
using System;

public class MobHealthBar : MonoBehaviour
{
    [System.Serializable]
    public struct Override
    {
        public Image healthBar;
        public Image damageBar;
        public TextMeshProUGUI healthValueDisplay;
        public TextMeshProUGUI nameLabel;
    }

    [System.Serializable]
    public struct Settings
    {
        public bool showWhenSelected;
        public bool hideName;
        public bool hideBar;
        public bool hideSelection;
    }

    [Header("Health Bar UI")]
    public Transform bodyToFollow;

    public Settings settings;
    public Override overrideProperties;
    private GameObject _healthBarPrefab;
    private Transform _healthBarsFolder;
    private GameObject _healthBarObject;
    private MobStats _mobStats;
    private MobController _mobController;
    private Selectable _selectable;
    private bool _healthBarSetupComplete = false;
    private float _health = 1.0f;
    private float _totalHealth = 2.0f;

    private Image _healthBar;
    private Image _healthDamageBar;
    private TextMeshProUGUI _nameLabel;
    private GameObject _targetIcon;
    private TextMeshProUGUI _healthValueDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (!bodyToFollow) bodyToFollow = transform;

        _healthBarsFolder = GlobalSettings.Instance.healthBarFolder;
        _healthBarPrefab = GlobalSettings.Instance.healthBarPrefab;

        if (overrideProperties.healthBar == null)
        {
            CreateHealthBar();
        }
        else 
        {
            LoadCustomHealthBar();
        }        

        UpdateHealthBar();

        if (TryGetComponent(out _mobStats))
        {
            _mobStats.OnHealthChange.AddListener(UpdateHealthBar);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!_healthBarSetupComplete) return;

        _healthBar.fillAmount = _health / _totalHealth;

        _healthDamageBar.fillAmount = Mathf.Lerp(_healthDamageBar.fillAmount, _healthBar.fillAmount, Time.deltaTime * 2f);
    }

    // main updater
    public void UpdateHealthBar(int currentHP = 1, int totalHP = 1)
    {
        _health = (float)currentHP;

        _totalHealth = (float)totalHP;

        if (_healthValueDisplay) _healthValueDisplay.text = currentHP + " / " + totalHP;
    }

    private void CreateHealthBar()
    {
        _healthBarObject = Instantiate(_healthBarPrefab, transform.position, Quaternion.identity);
        HealthBarViewController controller = _healthBarObject.GetComponent<HealthBarViewController>();
        _healthBar = controller.healthBar;
        _healthDamageBar = controller.damageBar;
        _targetIcon = controller.targetIcon;
        _nameLabel = controller.nameLabel;

        // Custom for Player's HP
        if (null != overrideProperties.healthBar)
        {
            _healthBar = overrideProperties.healthBar;
            _healthDamageBar = overrideProperties.damageBar;
            _healthValueDisplay = overrideProperties.healthValueDisplay;
        }

        // Settings
        if (settings.hideBar) controller.barContainer.gameObject.SetActive(false);
        if (settings.hideName) controller.nameLabel.gameObject.SetActive(false);
        if (settings.hideSelection) controller.targetIcon.SetActive(false);

        // Find this object's bounds height, using either a collider or mesh renderer
        Collider collider = GetComponentInChildren<Collider>();
        Renderer renderer = GetComponentInChildren<Renderer>();

        float height = collider ? collider.bounds.size.y : renderer.bounds.size.y;

        _nameLabel.text = gameObject.name;
        _healthBarObject.transform.SetParent(_healthBarsFolder);
        FollowTarget ft = _healthBarObject.GetComponent<FollowTarget>();
        ft.target = bodyToFollow;
        ft._offset = new Vector3(0, height * 1.1f, 0);

        _healthBarSetupComplete = true;

        // Turn off the HP Huds for those that only show it when selected
        if (settings.showWhenSelected) StartCoroutine(DoAfterDelay(0.2f, () => _healthBarObject.gameObject.SetActive(false))); 
    }

    private IEnumerator DoAfterDelay(float f, Action action)
    {
        yield return new WaitForSeconds(f);
        action?.Invoke();
    }

    private void LoadCustomHealthBar()
    {        
        _healthBar = overrideProperties.healthBar;
        _healthDamageBar = overrideProperties.damageBar;
        _healthValueDisplay = overrideProperties.healthValueDisplay;        

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
        ResetHealth();
        if (_healthBarObject) _healthBarObject.SetActive(true);        
    }



}
