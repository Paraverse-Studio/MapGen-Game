using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public string hoverMessage;
  private float timeToWait = 0.5f;

  public void SetHoverMessage(string message)
  {
    hoverMessage = message;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    SetHoverMessage(eventData.pointerEnter.gameObject.name);
    StopAllCoroutines();
    StartCoroutine(StartTimer());
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    StopAllCoroutines();
    HoverTipManager.OnMouseLoseFocus();
  }

  private void ShowMessage()
  {
    HoverTipManager.OnMouseHover(hoverMessage, Input.mousePosition);
  }

  private IEnumerator StartTimer()
  {
    yield return new WaitForSeconds(timeToWait);

    ShowMessage();
  }
}
