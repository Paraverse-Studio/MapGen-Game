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
            lods[2].screenRelativeTransitionHeight = 0.01f; 
            lods[1].screenRelativeTransitionHeight = 0.03f;
            lods[0].screenRelativeTransitionHeight = 0.06f;
            
            lod.SetLODs(lods);
        }
    }

}
