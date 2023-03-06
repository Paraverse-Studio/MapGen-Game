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
        public MobController controller;
        public ParticleSystem effect;

        public Mob (MobStats g, StatModifier m, MobController c, ParticleSystem ps)
        {
            mob = g; mod = m; controller = c; effect = ps;
        }
    }

    [SerializeField, Header("Speed Decrease on water")]
    private float speedDecreasePercent;

    private Mob playerMob;
    private List<Mob> allMobs = new();
    private bool initialized = false;

    private void Start()
    {
        TickManager.Instance?.Subscribe(this, gameObject, TickDelayOption.t5);
    }

    public void Tick()
    {
        if (!initialized) return;

        // Determine if to apply the mechanic
        for (int i = 0; i < allMobs.Count; ++i)
        {
            if (null == allMobs[i].mob)
            {
                allMobs.RemoveAt(i);
                continue;
            }

            if (allMobs[i].mob.transform.position.y <= 0.5f && allMobs[i].mob.transform.position.y > 0f)
            {
                if (allMobs[i].mod.Value == 0) allMobs[i].mod.Value = -(allMobs[i].mob.MoveSpeed.FinalValue * speedDecreasePercent);
                ToggleEffect(allMobs[i], true);
            }
            else
            {
                allMobs[i].mod.Value = 0;
                ToggleEffect(allMobs[i], false);
            }
        }        
    }

    private void ToggleEffect(Mob mob, bool o)
    {
        if (null == mob.effect) return;
        if (o && !mob.effect.isPlaying && (null == mob.controller || !mob.controller.IsFalling)) mob.effect.Play();
        else if (!o && mob.effect.isPlaying) mob.effect.Stop();
    }

    public void Initialize(ParticleSystem effectObj)
    {
        allMobs.Clear();
        
        // ATTACHING TO PLAYER
        var effect = Instantiate(effectObj);
        effect.Clear();

        GameObject player = GlobalSettings.Instance.player;
        playerMob = new Mob(player.GetComponentInChildren<MobStats>(), new StatModifier(0), null, effect);

        FollowTarget[] fts = effect.GetComponentsInChildren<FollowTarget>();
        foreach (FollowTarget ft in fts) ft.target = player.transform;

        playerMob.mob.MoveSpeed.AddMod(playerMob.mod);
        allMobs.Add(playerMob);        

        // ATTACHING TO MOBS
        foreach (MobController enemyController in EnemiesManager.Instance.Enemies)
        {
            var effectForMob = Instantiate(effectObj);
            effectForMob.Clear();

            Mob enemy = new Mob(enemyController.GetComponentInChildren<MobStats>(), new StatModifier(0), enemyController, effectForMob);

            FollowTarget[] fts2 = effectForMob.GetComponentsInChildren<FollowTarget>();
            foreach (FollowTarget ft2 in fts2) ft2.target = enemy.mob.transform;

            enemy.mob.MoveSpeed.AddMod(enemy.mod);
            allMobs.Add(enemy);
        }

        StartCoroutine(UtilityFunctions.IDelayedAction(1f, () => initialized = true));
    }

    public void Clear()
    {
        for (int i = 0; i < allMobs.Count; ++i)
        {
            allMobs[i].mob.MoveSpeed.RemoveMod(allMobs[i].mod);
            Destroy(allMobs[i].effect);
        }

        allMobs.Clear();
    }

}
