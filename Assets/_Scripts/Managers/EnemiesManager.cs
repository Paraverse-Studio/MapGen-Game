using Paraverse.Mob.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemiesManager : MonoBehaviour, ITickElement
{
    public static EnemiesManager Instance;

    [Header("Properties")]
    public ParticleSystem deathVFX;

    [Header("Performance")]
    public bool hideFarEnemies;
    public float hideEnemyDistance;
    public TickDelayOption checkDistanceDelay;

    [Header("Enemies List")]
    [SerializeField]
    private List<MobController> _enemies = new List<MobController>();

    [Header("Events")]
    public MobControllersListEvent OnEnemiesListUpdated = new MobControllersListEvent();

    private Transform _player;

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
        _player = GlobalSettings.Instance.player.transform;
        if (hideFarEnemies) TickManager.Instance?.Subscribe(this, gameObject, checkDistanceDelay);
    }

    public void Tick()
    {
        for(int i = 0; i < Enemies.Count; ++i)
        {
            if ((Enemies[i].transform.position - _player.position).sqrMagnitude > (hideEnemyDistance * hideEnemyDistance))
            {
                Enemies[i].gameObject.SetActive(false);
            }
            else
            {
                Enemies[i].gameObject.SetActive(true);
            }
        }
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
