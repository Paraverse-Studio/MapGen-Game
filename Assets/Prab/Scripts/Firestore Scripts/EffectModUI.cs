using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectModUI : MonoBehaviour
{
  public Image image;

  public void Init(Sprite sprite, string hoverMessage)
  {
    gameObject.name = hoverMessage;
    image.sprite = sprite;
  }
}
