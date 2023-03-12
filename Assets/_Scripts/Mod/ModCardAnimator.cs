using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModCardAnimator : MonoBehaviour
{
    public Transform cardFrame;
    public float mouseZOffset;
    private bool highlighted = false;
    public void Highlight(bool onOrOff) => highlighted = onOrOff;

    // Update is called once per frame
    void Update()
    {
        if (null == cardFrame) return;
        if (null == Input.mousePosition) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mouseZOffset;
        var lookDir = cardFrame.position - mousePos;

        cardFrame.rotation = Quaternion.Slerp(cardFrame.rotation, highlighted? Quaternion.LookRotation(lookDir) : Quaternion.identity, 2.5f * Time.deltaTime);

    }

}
