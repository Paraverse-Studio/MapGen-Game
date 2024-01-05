using UnityEngine;

namespace Paraverse.Stats
{
    public class TempStatModifier : StatModifier
    {
        public float Duration { get; set; } = 0f; // time-based
        public int DurationRounds { get; set; } = 0; // round # based

        public TempStatModifier(float value, float duration)
        {
            Value = value;
            Duration = duration;
        }

        public TempStatModifier(float value, int durationInRounds)
        {
            Value = value;
            DurationRounds = durationInRounds;
        }
    }
}