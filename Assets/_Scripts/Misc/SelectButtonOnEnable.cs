using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonOnEnable : MonoBehaviour
{
    Button button;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (true == gameObject.activeInHierarchy) button.Select();
    }

    private void OnDisable()
    {
        button.interactable = false;
        button.interactable = true;
    }   
    

}
