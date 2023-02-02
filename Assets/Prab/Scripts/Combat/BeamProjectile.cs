using Paraverse;
using Paraverse.Mob.Combat;
using UnityEngine;

public class BeamProjectile : Projectile
{
    [Header("Beam Properties")]
    protected MobCombat targetMob;
    [SerializeField, Tooltip("The projectile is a beam.")]
    protected bool isBeam = false;

    [Header("Beam Prefabs")]
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


    protected override void Update()
    {
        if (beam) //Updates the beam
        {
            dotTimer += Time.deltaTime;
            line.SetPosition(0, transform.position);

            Vector3 end;
            if (beamCollides && Physics.CapsuleCast(transform.position + (transform.forward * 0.5f), transform.position + (transform.up * -0.5f), 2f, transform.forward, out RaycastHit hit, beamLength)) //Checks for collision
            {
                end = hit.point - (transform.forward * beamEndOffset);

                if (hit.transform.CompareTag(targetTag) && dotTimer >= dotIntervalTimer)
                {
                    if (null == mob)
                        targetMob = hit.transform.GetComponentInChildren<MobCombat>();

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
            targetMob = null;
    }
}
