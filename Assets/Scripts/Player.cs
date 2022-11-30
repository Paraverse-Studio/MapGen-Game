using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 10000f;

    public void ApplyDamage(float damage)
    {
        health -= damage;
        Debug.Log(damage + " points of damage applied to " + gameObject.name + ".");
    }
}
