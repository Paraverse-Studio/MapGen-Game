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

    [Header("Show HUD Text: ")]
    public bool showHudText = true;
    public UnityEvent OnToggleHudText = new UnityEvent();

    public void ToggleHudText()
    {
        showHudText = !showHudText;
        OnToggleHudText?.Invoke();
    }

    [Space(20)]
    [Header("Folders for objects: ")]
    public Transform uiFolder;
    public Transform healthBarFolder;


    public GameObject playerPrefab;

    [Space(10)]
    [Header("Backup Safe Position: ")]
    public Vector3 backupSafePosition;



}
