using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MobHealthBar : MonoBehaviour
{
    [Header("Health Bar UI")]
    public Transform mobBody;

    private GameObject _healthBarPrefab;
    private Transform _healthBarsFolder;
    private GameObject _healthBarObject;
    private bool _healthBarSetupComplete = false;
    private int _health = 1;
    private int _totalHealth = 2;

    private Image _healthBar;
    private Image _healthDamageBar;

    public IntIntEvent OnHealthChange = new IntIntEvent();

    public delegate void OnDeathDel();
    public event OnDeathDel OnDeathEvent;

    private void Awake()
    {
        if (!mobBody) mobBody = transform;
        _healthBarsFolder = GlobalSettings.Instance.healthBarFolder;
        _healthBarPrefab = GlobalSettings.Instance.healthBarPrefab;
    }

    // Start is called before the first frame update
    void Start()
    {        
        if (null == _healthBarObject)
        {
            CreateHealthBar();
        }
        else
        {
            _healthBarObject.SetActive(true);
        }

        UpdateHealthBar();
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
        _health = currentHP;

        _totalHealth = totalHP;
    }

    private void CreateHealthBar()
    {
        _healthBarObject = Instantiate(_healthBarPrefab, transform.position, Quaternion.identity);
        _healthBar = _healthBarObject.GetComponent<HealthBarController>().healthBar;
        _healthDamageBar = _healthBarObject.GetComponent<HealthBarController>().damageBar;

        _healthBarObject.transform.SetParent(_healthBarsFolder);
        FollowTarget ft = _healthBarObject.GetComponent<FollowTarget>();
        ft.target = mobBody;
        ft._offset = new Vector3(0, 2.4f, 0);
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
