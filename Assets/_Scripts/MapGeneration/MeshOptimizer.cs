using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class MeshOptimizer : MonoBehaviour
{
    public static MeshOptimizer Instance;
    public int pieces = 10;
    public bool onStart = false;
    public Material mat;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!onStart) return;

        CombineMeshes(gameObject);
    }

    public void CombineMeshes(GameObject t)
    {
        MeshFilter[] meshFilters = t.GetComponentsInChildren<MeshFilter>();

        GameObject g = new();
        g.AddComponent<MeshFilter>();
        g.AddComponent<MeshRenderer>();

        List<CombineInstance> combine = new List<CombineInstance>();

        for (int i = 0; i < meshFilters.Length; ++i)
        {
            if (i > pieces)
            {
                g.transform.GetComponent<MeshFilter>().mesh = new Mesh();
                g.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
                g.transform.gameObject.SetActive(true);
                g.GetComponent<MeshRenderer>().material = mat;

                g = new();
                g.AddComponent<MeshFilter>();
                g.AddComponent<MeshRenderer>();

                combine = new List<CombineInstance>();
            }
            CombineInstance c = new();
            c.mesh = meshFilters[i].sharedMesh;
            c.transform = meshFilters[i].transform.localToWorldMatrix;
            combine.Add(c);

            meshFilters[i].gameObject.SetActive(false);
        }
        //g.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //g.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
        //g.transform.gameObject.SetActive(true);
        //g.GetComponent<MeshRenderer>().material = mat;
    }
}