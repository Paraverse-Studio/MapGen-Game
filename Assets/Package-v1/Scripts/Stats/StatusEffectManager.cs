using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paraverse.Stats
{
    public class StatusEffectManager : MonoBehaviour
    {
        private IMobStats stats;

        [SerializeField]
        private List<DamageOverTimeEffect> damageOverTimeEffects = new List<DamageOverTimeEffect>();
        private int idx = 0;

        private void Start()
        {
            stats = GetComponent<IMobStats>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ++idx;
                int dmg = Random.Range(1, 10);
                int dur = Random.Range(5, 10);

                string n = "DOT: " + idx;
                string d = "name: " + n + " dmg: " + dmg + "dur: " + dur + ".";
                DamageOverTimeEffect dotEffect = new DamageOverTimeEffect(n, d, dmg, dur);

                Debug.Log("created DOT effect: " + d);

                AddDamageOverTimeEffect(dotEffect);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ClearDamageOverTimeEffects();
            }
        }

        public void AddDamageOverTimeEffect(DamageOverTimeEffect effect)
        {
            damageOverTimeEffects.Add(effect);
            if (damageOverTimeEffects.Count == 1)
                StartCoroutine(DamageOverTimeHandler());
        }

        public void ClearDamageOverTimeEffects()
        {
            damageOverTimeEffects.Clear();
        }

        private IEnumerator DamageOverTimeHandler()
        {
            while (damageOverTimeEffects.Count > 0)
            {
                for (int i = 0; i < damageOverTimeEffects.Count; i++)
                {
                    stats.UpdateCurrentHealth(-damageOverTimeEffects[i].Damage);
                    damageOverTimeEffects[i].Duration -= 1f;
                }
                yield return new WaitForSeconds(1f);
                damageOverTimeEffects.RemoveAll(effect => effect.Duration <= 0f);
            }
        }
    }
}
