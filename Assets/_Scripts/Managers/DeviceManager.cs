using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceManager : MonoBehaviour
{
    [Header("Android specific")]
    public GameObject[] androidObjects;

    [Header("PC specific")]
    public GameObject[] pcObjects;

    [Header("WebGL specific")]
    public GameObject[] webObjects;


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in androidObjects) go.SetActive(false);
        foreach (GameObject go in pcObjects) go.SetActive(false);
        foreach (GameObject go in webObjects) go.SetActive(false);

#if UNITY_ANDROID
        foreach (GameObject go in androidObjects) go.SetActive(true);
#endif

    }

}
