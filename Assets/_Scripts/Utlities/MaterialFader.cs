using UnityEngine;

public class MaterialFader : MonoBehaviour
{
    public float transparencySpeed = 0.1f;
    private float _transparency = 1f;
    private Renderer _renderer;
    private Color _currentColor;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _currentColor = _renderer.material.color;
    }

    void Update()
    {
        _transparency -= transparencySpeed * Time.deltaTime;
        _renderer.material.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, _transparency);
    }
}