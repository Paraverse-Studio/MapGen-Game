using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance;
    private void Awake() => Instance = this;

    [Header("Show HUD Text: ")]
    public bool showHudText = true;
    public UnityEvent OnToggleHudText = new UnityEvent();

    [Button]
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
