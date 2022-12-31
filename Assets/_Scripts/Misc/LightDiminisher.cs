using UnityEngine;

public class LightDiminisher : MonoBehaviour
{
    public Light _light;
    public float speed;

    void Update()
    {
        _light.intensity -= (Time.deltaTime * speed);
    }
}
