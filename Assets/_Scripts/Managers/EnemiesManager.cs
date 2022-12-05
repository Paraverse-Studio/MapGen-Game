using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;

    private List<GameObject> _enemies = new List<GameObject>();

    public List<GameObject> Enemies
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    public void AddEnemy(GameObject enemyObj)
    {
        _enemies.Add(enemyObj);
    }

    public bool RemoveEnemy(GameObject enemyObj)
    {
        if (_enemies.Contains(enemyObj))
        {
            _enemies.Remove(enemyObj);
            return true;
        }
        else
        {
            return false;
        }
    }

}
