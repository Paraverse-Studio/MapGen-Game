namespace Paraverse.Stats
{
    public class StatModifier
    {
        public string Name { get; set; } = "Stat Modifier";
        public float Value { get; set; }

        public StatModifier(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public StatModifier(float value)
        {
            Value = value;
        }

        public StatModifier()
        {
        }

        public void UpdateValue(float value)
        {
            Value = value;
        }
    }
}
