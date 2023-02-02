using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    [SerializeField]
    private float deathTimer = 5f;
    private float curDeathTimer = 0f;

    private void Update()
    {
        if (curDeathTimer >= deathTimer)
            Destroy(gameObject);

        curDeathTimer += Time.deltaTime;
    }
}
