using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum LayerEnum {
    def = 0,
    TransparentFX = 1,
    IgnoreRaycast = 2,
    Water = 4,
    UI = 5, 
    Ground = 6,
    Wall = 7,
    Solid = 8,
    Breakable = 9,
    MobNormal = 10,
    MobCollision = 11
}


public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance;
    private void Awake() => Instance = this;


    [Header("Folders for objects: ")]
    [Space(20)]
    public Transform uiFolder;
    public Transform healthBarFolder;

    public Transform waterVolume;

    public bool recordBlockHistory = true;
    public GameObject playerPrefab;

    [Header("Backup Safe Position: ")]
    [Space(10)]
    public Vector3 backupSafePosition;



}
