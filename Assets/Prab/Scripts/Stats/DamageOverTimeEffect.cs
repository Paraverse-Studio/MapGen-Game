using UnityEngine;

namespace Paraverse.Stats
{
    public class DamageOverTimeEffect : MonoBehaviour
    {
        public string Name { get; }
        public string Description { get; }
        public int Damage { get; }
        public float Duration { get; set; }

        public DamageOverTimeEffect(string name, string description, int damage, float duration)
        {
            Name = name;
            Description = description;
            Damage = damage;
            Duration = duration;
        }
    }
}