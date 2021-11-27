using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;


    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    public UnityEvent OnPlay = new UnityEvent();


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
        if (Input.GetKeyDown(KeyCode.O))
        {
            PauseGame();
        }
        
    }

    public void StartGame()
    {
        OnPlay?.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused? 1f:0f;
    }

}
