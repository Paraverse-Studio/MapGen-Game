using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [Header("Destroy this object after: ")]
    public float lifetime;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime) Destroy(gameObject);        
    }

}
