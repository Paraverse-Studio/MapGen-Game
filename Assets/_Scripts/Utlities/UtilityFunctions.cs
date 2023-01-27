using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions 
{
    
    public static bool IsDistanceLessThan(Vector3 a, Vector3 b, float compareValue)
    {
        return (a - b).sqrMagnitude < (compareValue * compareValue);
    }

    public static void UpdateLODlevels(Transform root)
    {
        LODGroup[] objectLods = root.GetComponentsInChildren<LODGroup>();
        foreach (LODGroup lod in objectLods)
        {
            lod.animateCrossFading = true;
            lod.fadeMode = LODFadeMode.CrossFade;
            LODGroup.crossFadeAnimationDuration = 0.3f;
            LOD[] lods = lod.GetLODs();

            int size = lods.Length;
            int index = 0;
            for (int i = size - 1; i >= 0; --i)
            {
                lods[i].screenRelativeTransitionHeight = 0.015f + (0.020f * index);
                index++;
            }

            if (GlobalSettings.Instance.QualityLevel == 1)
            {
                lod.ForceLOD(size - 1);
            }
            
            lod.SetLODs(lods);
        }
    }

    public static void TeleportObject(GameObject obj, Vector3 spot)
    {
        CharacterController cc;
        if (obj.TryGetComponent(out cc))
        {
            cc.enabled = false;
        }
        obj.transform.position = spot;

        if (cc) cc.enabled = true;
    }

    public static string ToRoman(int number)
    {
        if ((number < 0) || (number > 3999)) return "Invalid";
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        return "Invalid";
    }

}
