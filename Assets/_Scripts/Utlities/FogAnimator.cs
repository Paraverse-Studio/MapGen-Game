using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogAnimator : MonoBehaviour
{
    Material mat;
    public float speed;
    public float xSpeed;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float f = mat.GetFloat("_yOffset");
        f += Time.deltaTime * speed;
        //vec.x += Time.deltaTime * xSpeed;
        mat.SetFloat("_yOffset", f);

        Vector3 vec = mat.GetVector("Vector3_478B0511");
        vec.z += Time.deltaTime * xSpeed;
        mat.SetVector("Vector3_478B0511", vec);
    }
}
