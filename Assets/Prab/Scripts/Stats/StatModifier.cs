using UnityEngine;

namespace Paraverse.Stats
{
    public class StatModifier
    {
        public float Value { get; set; }

        public StatModifier(float value)
        {
            Value = value;
        }

        public StatModifier()
        {
        }
    }
}
