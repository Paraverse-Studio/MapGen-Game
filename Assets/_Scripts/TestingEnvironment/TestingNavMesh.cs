using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestingNavMesh : MonoBehaviour
{
    public NavMeshSurface surface;

    private void Start()
    {
        //surface = GetComponentInChildren<NavMeshSurface>();
    }


    public void BuildNavMesh()
    {
        surface.BuildNavMesh();
    }

    public void UpdateNavMesh()
    {
        surface.UpdateNavMesh(surface.navMeshData);
    }
}
