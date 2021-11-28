using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealth : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Transform healthBarFolder;

    [Header("Health Bar settings")]
    public float healthBarHeight = 2.0f;

    private GameObject _healthBar;

    private MobController _mobController;

    private void Awake()
    {
        _mobController = GetComponent<MobController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        CreateHealthBar();
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
}
