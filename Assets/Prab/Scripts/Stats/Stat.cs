using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paraverse.Stats
{
    [Serializable]
    public class Stat
    {
        public float BaseValue { get; }
        public float FinalValue { get { return BaseValue + StatModifierSumValue() + TempStatModifierSumValue(); } }
        public float StatModValue { get { return StatModifierSumValue(); } }
        public float TempStatModValue { get { return TempStatModifierSumValue(); } }

        private List<StatModifier> _modifiers = new List<StatModifier>();
        private List<TempStatModifier> _tempStatModifier = new List<TempStatModifier>();

        public Stat(float value)
        {
            BaseValue = value;
        }

        /// <summary>
        /// Run this method in Update of MobStats
        /// </summary>
        public void TempStatModifierHandler()
        {
            for (int i = 0; i < _tempStatModifier.Count; i++)
            {
                _tempStatModifier[i].Duration -= Time.deltaTime;
            }
            _tempStatModifier.RemoveAll(temp => temp.Duration <= 0f);
        }

        public float StatModifierSumValue()
        {
            float value = 0f;
            for (int i = 0; i < _modifiers.Count; i++)
            {
                value += _modifiers[i].Value;
            }
            return value;
        }

        public float TempStatModifierSumValue()
        {
            float value = 0f;
            for (int i = 0; i < _tempStatModifier.Count; i++)
            {
                value += _tempStatModifier[i].Value;
            }
            return value;
        }

        public void AddMod(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveMod(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
        }

        public void ClearAllMods()
        {
            _modifiers.Clear();
        }

        public void AddTempMod(TempStatModifier modifier)
        {
            _tempStatModifier.Add(modifier);
        }

        public void RemoveTempMod(TempStatModifier modifier)
        {
            _tempStatModifier.Remove(modifier);
        }

        public void ClearAllTempMods()
        {
            _tempStatModifier.Clear();
        }
    }
}