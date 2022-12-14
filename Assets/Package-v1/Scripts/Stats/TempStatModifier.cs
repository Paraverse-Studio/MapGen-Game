using UnityEngine;

namespace Paraverse.Stats
{
    public class TempStatModifier : MonoBehaviour
    {
        public float Value { get; }
        public float Duration { get; set; } 

        public TempStatModifier(float value, float duration)
        {
            Value = value;
            Duration = duration;
        }
    }
}