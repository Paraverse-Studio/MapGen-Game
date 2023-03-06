using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapMechanics
{
    public void Initialize(ParticleSystem go);

    public void Clear();
}

public class GreenPlains : MonoBehaviour, IMapMechanics, ITickElement
{
    public struct Mob
    {
        public MobStats mob;
        public StatModifier mod;

        public Mob (MobStats g, StatModifier m)
        {
            mob = g; mod = m;
        }
    }

    [SerializeField, Header("Speed Decrease")]
    private float speedDecrease;

    private Mob playerMob;
    private List<ParticleSystem> effects = new();
    private List<Mob> allMobs = new();

    private void Start()
    {
        TickManager.Instance?.Subscribe(this, gameObject, TickDelayOption.t10);
    }

    public void Tick()
    {
        // Determine if to apply the mechanic
        for(int i = 0; i < allMobs.Count; ++i)
        {
            if (null == allMobs[i].mob)
            {
                allMobs.RemoveAt(i);
                continue;
            }

            if (allMobs[i].mob.transform.position.y <= 0.5f && allMobs[i].mob.transform.position.y > 0f)
            {
                if (allMobs[i].mod.Value == 0) allMobs[i].mod.Value = -(allMobs[i].mob.MoveSpeed.FinalValue * 0.33f);
            }
            else
            {
                allMobs[i].mod.Value = 0;
            }
        }

        // Now, apply all parts of the mechanic of this map 
        for (int i = 0; i < effects.Count; ++i)
        {
            if (null == effects[i]) continue;

            var effect = effects[i];

            if (effect.gameObject.transform.position.y <= 0.5f && effect.gameObject.transform.position.y > 0f)
            {
                if (!effect.isPlaying) effect.Play();
            }
            else
            {
                if (effect.isPlaying)
                {
                    effect.Stop();
                    StartCoroutine(UtilityFunctions.IDelayedAction(0.05f, () =>
                    {
                        if (null != effect && effect.isStopped) effect.Clear();
                    }));
                }
            }
        }
    }

    public void Initialize(ParticleSystem effectObj)
    {
        allMobs.Clear();
        playerMob = new Mob(GlobalSettings.Instance.player.GetComponentInChildren<MobStats>(), new StatModifier(speedDecrease));
        playerMob.mob.MoveSpeed.AddMod(playerMob.mod);
        allMobs.Add(playerMob);
        

        foreach (MobController enemyController in EnemiesManager.Instance.Enemies)
        {
            Mob enemy = new Mob(enemyController.GetComponentInChildren<MobStats>(), new StatModifier(speedDecrease));
            enemy.mob.MoveSpeed.AddMod(enemy.mod);
            allMobs.Add(enemy);
        }

        foreach (Mob mob in allMobs)
        {
            var effect = Instantiate(effectObj);

            effect.Stop();

            effects.Add(effect);
            FollowTarget[] fts = effect.GetComponentsInChildren<FollowTarget>();
            foreach (FollowTarget ft in fts) ft.target = mob.mob.transform;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < effects.Count; ++i)
        {
            if (null != effects[i]) Destroy(effects[i].gameObject);
        }

        effects.Clear();

        playerMob.mob.MoveSpeed.RemoveMod(playerMob.mod);
    }

}
