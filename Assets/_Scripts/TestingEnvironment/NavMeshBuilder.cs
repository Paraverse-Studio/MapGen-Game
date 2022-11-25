using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBuilder : MonoBehaviour
{
    public NavMeshSurface surface;

    public void BuildNavMesh()
    {
        if (surface) surface.BuildNavMesh();
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
