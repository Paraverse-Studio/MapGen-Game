using Paraverse.Mob.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    public ParticleSystem deathVFX;

    [SerializeField]
    private List<MobController> _enemies = new List<MobController>();

    public MobControllersListEvent OnEnemiesListUpdated = new MobControllersListEvent();

    public List<MobController> Enemies
    {
        get { return _enemies; }
    }

    public int EnemiesCount
    {
        get { return _enemies.Count; }
    }


    private void Awake()
    {
        Instance = this;
        _enemies.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  

    public void ResetEnemiesList()
    {
        _enemies.Clear();
    }

    public void AddEnemy(GameObject enemyObj)
    {
        MobController enemy = enemyObj.GetComponentInChildren<MobController>();
        if (enemy)
        {
            _enemies.Add(enemy);
            OnEnemiesListUpdated?.Invoke(Enemies);

            enemy.OnDeathEvent += RemoveEnemy;
            //enemy.OnDeathEvent += SpawnDeathVFX;
        }
    }

    public void RemoveEnemy(Transform enemyObj)
    {
        MobController enemy = enemyObj.gameObject.GetComponentInChildren<MobController>();
        if (enemy)
        {
            if (_enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
                OnEnemiesListUpdated?.Invoke(Enemies);
            }
        }        
    }

    private void SpawnDeathVFX(Transform t)
    {
        Instantiate(deathVFX, t.transform.position, Quaternion.identity);
    }

}
