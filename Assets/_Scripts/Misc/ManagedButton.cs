using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManagedButton : MonoBehaviour
{
    Button button;
    public bool selectOnStart = true;
    public bool delayInteractionOnStart = false;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (true == gameObject.activeInHierarchy && selectOnStart)
        {
            if (!delayInteractionOnStart)
            {
                UIManager.Instance.SelectButton(button);
            }
            else
            {
                StartCoroutine(IDelayedAction(() => UIManager.Instance.SelectButton(button), 0.15f));
            }
        }
    }

    private IEnumerator IDelayedAction(System.Action a, float f)
    {
        yield return new WaitForSecondsRealtime(f);
        a?.Invoke();
    }

    private void OnDisable()
    {
        button.interactable = false;
        button.interactable = true;
        UIManager.Instance.DeselectButton(button);
    }   
    

}
