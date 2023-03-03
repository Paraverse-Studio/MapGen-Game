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
        _transparency = _renderer.material.color.a;
    }

    void Update()
    {
        _transparency -= transparencySpeed * Time.deltaTime;
        if (_transparency < 0f) _transparency = 0f;
        _renderer.material.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, _transparency);
    }
}