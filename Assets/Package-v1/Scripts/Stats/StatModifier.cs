using UnityEngine;

namespace Paraverse.Stats
{
    public class StatModifier
    {
        public float Value { get; }

        public StatModifier(float value)
        {
            Value = value;
        }

        public StatModifier()
        {
        }
    }
}
