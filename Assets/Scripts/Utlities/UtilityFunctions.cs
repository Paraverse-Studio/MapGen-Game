using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions 
{
    
    public static bool IsDistanceLessThan(Vector3 a, Vector3 b, float compareValue)
    {
        float dist = (a - b).sqrMagnitude;
        if (dist < (compareValue * compareValue)) return true;
        else return false;
    }

    public static void UpdateLODlevels(Transform root)
    {
        LODGroup[] objectLods = root.GetComponentsInChildren<LODGroup>();
        foreach (LODGroup lod in objectLods)
        {
            LODGroup.crossFadeAnimationDuration = 0.3f;
            lod.fadeMode = LODFadeMode.CrossFade;
            lod.animateCrossFading = true;
            lod.fadeMode = LODFadeMode.CrossFade;
            LODGroup.crossFadeAnimationDuration = 0.3f;
            LOD[] lods = lod.GetLODs();

            int size = lods.Length;
            int index = 0;
            for (int i = size - 1; i >= 0; --i)
            {
                lods[i].screenRelativeTransitionHeight = 0.015f + (0.025f * index);
                index++;
            }
            
            lod.SetLODs(lods);
        }
    }

}
