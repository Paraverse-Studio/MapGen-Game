using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthFlash : MonoBehaviour
{
    [SerializeField]
    private Material _flashMaterial;

    private MobStats _mobStats;
    private Renderer[] _renderers;
    private List<Material> _originalMaterials = new List<Material>();

    private IEnumerator _iDamageFlash;
    private bool _alreadyRunning = false;
    private float _flashDuration = 0.1f;
    private float _delayDuration = 0.06f;
    private int _cachedHealth = -1; 

    // Start is called before the first frame update
    void Start()
    {
        if (null == _flashMaterial && GlobalSettings.Instance)
        {
            _flashMaterial = GlobalSettings.Instance.FlashMaterial;
        }

        _renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in _renderers)
        {
            _originalMaterials.Add(r.material);
        }

        _iDamageFlash = IDamageFlash();

        if (TryGetComponent(out _mobStats))
        {
            _mobStats.OnHealthChange.AddListener(FlashWhenDamage);
        }
    }

    public void FlashWhenDamage(int currentHealth, int totalHealth)
    {
        if (_cachedHealth != -1)
        {
            if (currentHealth < _cachedHealth) DamageFlash();
        }
        else
        {
            if (currentHealth < totalHealth) DamageFlash(); 
        }

        _cachedHealth = currentHealth;
    }

    void DamageFlash()
    {
        if (_alreadyRunning)
        {   
            StopCoroutine(_iDamageFlash);
        }

        _iDamageFlash = IDamageFlash();
        StartCoroutine(_iDamageFlash);
    }

    private IEnumerator IDamageFlash()
    {
        _alreadyRunning = true;

        ToggleMaterial(true);
        yield return new WaitForSeconds(_flashDuration);

        ToggleMaterial(false);
        yield return new WaitForSeconds(_delayDuration);

        ToggleMaterial(true);
        yield return new WaitForSeconds(_flashDuration);
        ToggleMaterial(false);

        _alreadyRunning = false;
    }

    public void ToggleMaterial(bool flashOn)
    {
        for (int i = 0; i < _renderers.Length; ++i)
        {
            _renderers[i].material = flashOn? _flashMaterial : _originalMaterials[i];
        }
    }

}