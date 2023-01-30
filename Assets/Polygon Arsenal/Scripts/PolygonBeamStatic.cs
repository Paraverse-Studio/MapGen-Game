using Paraverse.Mob;
using Paraverse.Mob.Combat;
using UnityEngine;

namespace PolygonArsenal
{

    public class PolygonBeamStatic : MonoBehaviour
    {
        private MobCombat mob;

        [Header("Prefabs")]
        public GameObject beamLineRendererPrefab; //Put a prefab with a line renderer onto here.
        public GameObject beamStartPrefab; //This is a prefab that is put at the start of the beam.
        public GameObject beamEndPrefab; //Prefab put at end of beam.

        private GameObject beamStart;
        private GameObject beamEnd;
        private GameObject beam;
        private LineRenderer line;

        [Header("Beam Options")]
        public bool alwaysOn = true; //Enable this to spawn the beam when script is loaded.
        public bool beamCollides = true; //Beam stops at colliders
        public float beamLength = 100; //Ingame beam length
        public float beamEndOffset = 0f; //How far from the raycast hit point the end effect is positioned
        public float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
        public float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. 
                                                //Example: if texture is 200 pixels in height and 600 in length, set this to 3

        [Header("Knockback Effect")]
        [SerializeField]
        private string targetTag = StringData.PlayerTag;
        [SerializeField]
        private float dotIntervalTimer = 1f;
        [SerializeField]
        private KnockBackEffect knockBackEffect;
        private float dotTimer = 0f;
        [SerializeField, Tooltip("Applies hit animation to target.")]
        protected bool applyHitAnim = true;
        public ScalingStatData scalingStatData;


        void FixedUpdate()
        {
            if (beam) //Updates the beam
            {
                dotTimer += Time.deltaTime;
                line.SetPosition(0, transform.position);

                Vector3 end;
                RaycastHit hit;
                if (beamCollides && Physics.CapsuleCast(transform.position + (transform.forward * 0.5f), transform.position + (transform.up * -0.5f), 2f, transform.forward, out hit, beamLength)) //Checks for collision
                {
                    end = hit.point - (transform.forward * beamEndOffset);

                    if (hit.transform.CompareTag(targetTag) && dotTimer >= dotIntervalTimer)
                    {
                        if (null == mob)
                            mob = hit.transform.GetComponentInChildren<MobCombat>();
                        dotTimer = 0f;
                        DamageLogic(hit.collider);
                    }
                }
                else
                    end = transform.position + (transform.forward * beamLength);

                line.SetPosition(1, end);

                if (beamStart)
                {
                    beamStart.transform.position = transform.position;
                    beamStart.transform.LookAt(end);
                }
                if (beamEnd)
                {
                    beamEnd.transform.position = end;
                    beamEnd.transform.LookAt(beamStart.transform.position);
                }

                float distance = Vector3.Distance(transform.position, end);
                line.material.mainTextureScale = new Vector2(distance / textureLengthScale, 1); //This sets the scale of the texture so it doesn't look stretched
                line.material.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0); //This scrolls the texture along the beam if not set to 0
            }
            else
                mob = null;
        }

        public void SpawnBeam() //This function spawns the prefab with linerenderer
        {
            if (beamLineRendererPrefab)
            {
                if (beamStartPrefab)
                    beamStart = Instantiate(beamStartPrefab);
                if (beamEndPrefab)
                    beamEnd = Instantiate(beamEndPrefab);
                beam = Instantiate(beamLineRendererPrefab);
                beam.transform.position = transform.position;
                beam.transform.parent = transform;
                beam.transform.rotation = transform.rotation;
                line = beam.GetComponent<LineRenderer>();
                line.useWorldSpace = true;
                line.positionCount = 2;
            }
            else
                print("Add a hecking prefab with a line renderer to the SciFiBeamStatic script on " + gameObject.name + "! Heck!");
        }

        public void RemoveBeam() //This function removes the prefab with linerenderer
        {
            if (beam)
                Destroy(beam);
            if (beamStart)
                Destroy(beamStart);
            if (beamEnd)
                Destroy(beamEnd);
        }

        /// <summary>
        /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
        /// </summary>
        public float ApplyCustomDamage(IMobController controller)
        {
            float totalDmg =
                scalingStatData.flatPower +
                (controller.Stats.AttackDamage.FinalValue * scalingStatData.attackScaling) +
                (controller.Stats.AbilityPower.FinalValue * scalingStatData.abilityScaling);

            controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(totalDmg));
            return totalDmg;
        }

        private void DamageLogic(Collider other)
        {
            IMobController controller = other.GetComponent<IMobController>();
            if (null != controller)
            {
                ApplyCustomDamage(controller);

                // Apply knock back effect
                if (null != knockBackEffect)
                {
                    KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
                    controller.ApplyKnockBack(mob.transform.position, effect);
                }
                else if (applyHitAnim)
                {
                    controller.ApplyHitAnimation();
                }
            }
        }
    }
}