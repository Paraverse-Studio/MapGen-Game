using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagedButton : MonoBehaviour
{
    Button button;
    public bool selectOnStart = true;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (true == gameObject.activeInHierarchy && selectOnStart)
        {
            UIManager.Instance.SelectButton(button);
        }
    }

    private void OnDisable()
    {
        Debug.Log("AYY I GOT CALLED!! " + gameObject.name);
        button.interactable = false;
        button.interactable = true;
        UIManager.Instance.DeselectButton(button);
    }   
    

}
