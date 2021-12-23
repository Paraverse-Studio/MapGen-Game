using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimation : MonoBehaviour, ITickElement
{
    public bool useCookieTexture;

    private Light lighting;
    private Texture cookieTexture;



    // Start is called before the first frame update
    void Start()
    {
        lighting = GetComponent<Light>();
        if (!lighting ) { Debug.Log("No Light component found on this object."); enabled = false; }

        cookieTexture = lighting.cookie;
        lighting.cookie = useCookieTexture ? cookieTexture : null;

        TickManager.Instance.Subscribe(this, gameObject, TickDelayOption.t120);
    }

    public void Tick()
    {
        lighting.cookie = useCookieTexture ? cookieTexture : null;
    }


}
