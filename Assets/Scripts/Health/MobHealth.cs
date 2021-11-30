using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.Events;

public class MobHealth : MonoBehaviour
{
    [Header("Health Bar UI")]
    public bool useHealthBar = true;
    public GameObject healthBarPrefab;
    public float healthBarHeight = 2.0f;

    [Header("Health Bar Settings")]
    [SerializeField]
    private int _totalHealth;

    private GameObject _healthBar;
    private Transform _healthBarGroup;

    private MobController _mobController;

    private int _health;
    public int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, _totalHealth); }
    }
    

    private List<Image> _healthBars;
    private int _healthBarMarker = 0;

    public UnityEvent OnHealthChange = new UnityEvent();

    private void Awake()
    {
        _mobController = GetComponent<MobController>();
        _healthBars = new List<Image>();
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateHealthBar();
        _healthBarGroup = _healthBar.transform.GetChild(0);
        _health = _totalHealth;
        _healthBarMarker = _health;
        SetTotalHealth(_totalHealth);       
        OnHealthChange.AddListener(UpdateHealthBars);
    }


    // Update is called once per frame
    void Update()
    {
        //if (_health < _healthBarMarker)
        //{
        //    for (int i = _totalHealth; i >= 0; --i)
        //    {
        //        StartCoroutine(ELerpMarker(false, _healthBars[i] ));
        //        _healthBarMarker--;
        //        if (_health >= _healthBarMarker) continue;
        //    }
        //}
        //else if (_health > _healthBarMarker)
        //{
        //    for (int i = 0; i < _totalHealth; ++i)
        //    {
        //        _healthBars[i].fillAmount = 1f;
        //        if (_health <= _healthBarMarker) continue;
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.L)) TakeDamage(-1);
        if (Input.GetKeyDown(KeyCode.J)) IncreaseTotalHealth(-1);
    }

    private IEnumerator ELerpMarker(bool upOrDown, Image image)
    {
        for (int i = 0; i < 80; ++i)
        {
            image.fillAmount += Time.deltaTime * (upOrDown ? 1 : -1);
            yield return new WaitForSeconds(0.016666f);
        }
    }


    private void UpdateHealthBars()
    {
        for (int i = 0; i < _healthBars.Count; ++i)
        {
            if ((i + 1) <= _health)
                _healthBars[i].fillAmount = 1f;
            else
                _healthBars[i].fillAmount = 0f;
        }
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        OnHealthChange?.Invoke();
    }


    private void CreateHealthBar()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(_mobController.Body.position + new Vector3(0, healthBarHeight, 0));
        screenPoint.z = 0;

        _healthBar = GameObject.Instantiate(healthBarPrefab, screenPoint, Quaternion.identity);
        _healthBar.transform.SetParent(GlobalSettings.Instance.healthBarFolder);
        FollowTarget2D ft = _healthBar.GetComponent<FollowTarget2D>();
        ft.target = _mobController.Body;
        ft._offset = new Vector3(0, healthBarHeight, 0);
    }

    [Button]
    public void IncreaseTotalHealth(int i = 1)
    {
        _totalHealth += i;
        SetTotalHealth(_totalHealth);
    }

    public void SetTotalHealth(int val = 1)
    {
        _totalHealth = val;
        _health = Mathf.Min(_health, _totalHealth);

        int size = _healthBarGroup.childCount;

        if (size > _totalHealth)
        {
            for (int i = size - 1; i > 0; --i)
            {
                if ((i + 1) > _totalHealth)
                {
                    Destroy(_healthBarGroup.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            for (int i = size; i < _totalHealth; ++i)
            {
                Instantiate(_healthBarGroup.GetChild(0), _healthBarGroup);
            }
        }
        RefreshBarWidthFromHealth();
        RefreshHealthBarsList();
        UpdateHealthBars();
    }

    public void RefreshBarWidthFromHealth()
    {
        int width = (int)Mathf.Min((_totalHealth * Mathf.Max(35 - (_totalHealth * 1.65f), 16f)) + 50, 170 + (_totalHealth * 6));

        RectTransform rt = _healthBarGroup.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
    public void RefreshHealthBarsList()
    {
        _healthBars = new List<Image>();
        for (int i = 0; i < _totalHealth; ++i)
        {
            _healthBars.Add(_healthBarGroup.GetChild(i).transform.GetChild(1).gameObject.GetComponent<Image>());
        }
    }

}
