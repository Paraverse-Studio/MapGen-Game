using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class HierarchyName : MonoBehaviour
{
    public string objectName;

    private string nom = "";
    private float time = 0f;
    public SO_Int length;

    // Update is called once per frame
    void OnGUI()
    {
        time += Time.deltaTime;

        if (time > 1f)
        {
            time = 0f;
            UpdateHierarchyName();
        }
    }

    private void UpdateHierarchyName()
    {
        if (!length) return;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        int totalLengthRequired = length.value;

        int numOfDashes = (totalLengthRequired - objectName.Length - 4);

        nom = "";

        for (int i = 0; i < numOfDashes / 2; ++i)
        {
            nom += "—";
        }
        nom += "  " + objectName.ToUpper() + "  ";
        for (int i = 0; i < numOfDashes / 2; ++i)
        {
            nom += "—";
        }
        gameObject.name = nom;
    }


}
