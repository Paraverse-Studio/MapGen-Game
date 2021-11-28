using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MobHealth : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Transform healthBarFolder;

    [Header("Health Bar settings")]
    public float healthBarHeight = 2.0f;

    private GameObject _healthBar;
    private Transform _healthBarGroup;

    private MobController _mobController;

    private int _health;
    public int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, _totalHealth); }
    }
    [SerializeField]
    private int _totalHealth;


    private void Awake()
    {
        _mobController = GetComponent<MobController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateHealthBar();
        _healthBarGroup = _healthBar.transform.GetChild(0);
        SetTotalHealth(_totalHealth);
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void CreateHealthBar()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(_mobController.Body.position + new Vector3(0, healthBarHeight, 0));
        screenPoint.z = 0;

        _healthBar = GameObject.Instantiate(healthBarPrefab, screenPoint, Quaternion.identity);
        _healthBar.transform.SetParent(healthBarFolder);
        FollowTarget2D ft = _healthBar.GetComponent<FollowTarget2D>();
        ft.target = _mobController.Body;
        ft._offset = new Vector3(0, healthBarHeight, 0);        
    }

    [Button]
    public void IncreaseTotalHealth()
    {
        _totalHealth++;
        SetTotalHealth(_totalHealth);
    }
    
    public void SetTotalHealth(int val = 1)
    {
        for (int i = _healthBarGroup.childCount-1; i > 0; --i)
        {
            Destroy(_healthBarGroup.GetChild(i).gameObject);
        }

        _totalHealth = val;

        for (int i = 0; i < _totalHealth - 1; ++i)
        {
            Instantiate(_healthBarGroup.GetChild(0), _healthBarGroup);
        }
        UpdateBarWidthFromHealth();
    }


    public void UpdateBarWidthFromHealth()
    {
        int width = (int)Mathf.Min((_totalHealth * Mathf.Max(35 - (_totalHealth * 1.65f), 16f)) + 50, 170+(_totalHealth*6));

        RectTransform rt = _healthBarGroup.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }


    public void TakeDamage(int dmg)
    {
        Health -= dmg;
    }



}
