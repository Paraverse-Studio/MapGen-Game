using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private float damage;

    private void Start()
    {
    }

    public void Init(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().ApplyDamage(damage);
        }
    }
}
