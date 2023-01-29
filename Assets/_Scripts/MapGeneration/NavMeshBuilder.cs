using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBuilder : MonoBehaviour
{
    public static NavMeshBuilder Instance;

    public NavMeshSurface surface;

    public bool update = false;
    public float updateDelay = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    public void BuildNavMesh(NavMeshSurface s = null)
    {
        if (surface) surface.BuildNavMesh();
        else if (s)
        {
            surface = s;
            s.BuildNavMesh();
        }
        else
        {
            Debug.LogError("Trying to build Nav Mesh without a NavMeshSurface provided!");
        }
    }

    public void UpdateNavMesh()
    {
        surface.UpdateNavMesh(surface.navMeshData);
    }

}
