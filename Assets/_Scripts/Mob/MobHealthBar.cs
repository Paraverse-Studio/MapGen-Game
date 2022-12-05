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
    }

    [Header("Health Bar UI")]
    public Transform mobBody;

    public Override overrideProperties;

    private GameObject _healthBarPrefab;
    private Transform _healthBarsFolder;
    private GameObject _healthBarObject;
    private MobStats _mobStats;
    private bool _healthBarSetupComplete = false;
    private float _health = 1.0f;
    private float _totalHealth = 2.0f;

    private Image _healthBar;
    private Image _healthDamageBar;
    private TextMeshProUGUI _healthValueDisplay;
    private GameObject _targetIcon;

    // Start is called before the first frame update
    void Start()
    {
        if (!mobBody) mobBody = transform;

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

        TargetLock.Instance.OnTargetLocked.AddListener(ActivateTargetIcon);
        TargetLock.Instance.OnTargetUnlocked.AddListener(DeactivateTargetIcon);
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
        _healthBar = _healthBarObject.GetComponent<HealthBarController>().healthBar;
        _healthDamageBar = _healthBarObject.GetComponent<HealthBarController>().damageBar;
        _targetIcon = _healthBarObject.GetComponent<HealthBarController>().targetIcon;

        if (null != overrideProperties.healthBar)
        {
            _healthBar = overrideProperties.healthBar;
            _healthDamageBar = overrideProperties.damageBar;
            _healthValueDisplay = overrideProperties.healthValueDisplay;
        }

        _healthBarObject.transform.SetParent(_healthBarsFolder);
        FollowTarget ft = _healthBarObject.GetComponent<FollowTarget>();
        ft.target = mobBody;
        ft._offset = new Vector3(0, 2.4f, 0);

        _healthBarSetupComplete = true;
    }

    private void LoadCustomHealthBar()
    {        
        _healthBar = overrideProperties.healthBar;
        _healthDamageBar = overrideProperties.damageBar;
        _healthValueDisplay = overrideProperties.healthValueDisplay;        

        _healthBarSetupComplete = true;
    }

    public void ActivateTargetIcon(MobController obj)
    {
        if (obj == _mobStats.gameObject && _targetIcon)
        {
            _targetIcon.SetActive(true);
        }
    }

    public void DeactivateTargetIcon(MobController obj)
    {
        if (obj == _mobStats.gameObject && _targetIcon)
        {
            _targetIcon.SetActive(false);
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
