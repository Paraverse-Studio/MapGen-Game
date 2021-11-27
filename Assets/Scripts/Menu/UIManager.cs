using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public MenuScreen[] menus;

    public static UIManager Instance;

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

    public void CloseAll()
    {
        for (int i = 0; i < menus.Length; ++i)
        {
            menus[i]?.End();
        }
    }

    public void CloseAllAndOpen(MenuScreen menu)
    {
        CloseAll();
        OpenMenu(menu);
    }

    public void OpenMenu(MenuScreen menu)
    {
        for (int i = 0; i < menus.Length; ++i)
        {
            if (menus[i] == menu)
            {
                Debug.Log("Set active: " + menu.gameObject.name);
                menu.gameObject.SetActive(true);
                menu.Show();
            }
        }
    }


}
