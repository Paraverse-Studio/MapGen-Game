using UnityEngine;

namespace Paraverse.Stats
{

    public abstract class Effect : MonoBehaviour
    {
        public string Name { get; }
        public string Description { get; }

        public Effect(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public abstract void Execute();
    }
}