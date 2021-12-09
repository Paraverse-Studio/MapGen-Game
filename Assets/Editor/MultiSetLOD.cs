using UnityEditor;
using UnityEngine;
public class MultiSetLOD : MonoBehaviour
{
    [MenuItem("Component/LOD/Set Default LOD Range", false, 0)]
    static void DoSetDefaultLODRange()
    {
        var list = Selection.GetFiltered<GameObject>(SelectionMode.TopLevel);
        foreach (var go in list)
        {
            var d = go.GetComponent<LODGroup>();
            if (null != d)
            {
                var lods = d.GetLODs();
                lods[0].screenRelativeTransitionHeight = 0.15f;
                lods[1].screenRelativeTransitionHeight = 0.10f;
                lods[2].screenRelativeTransitionHeight = 0.05f;
                d.SetLODs(lods);
                PrefabUtility.RecordPrefabInstancePropertyModifications(d);
            }
        }
    }
}