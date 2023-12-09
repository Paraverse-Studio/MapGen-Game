using Paraverse;
using Paraverse.Mob.Combat;
using UnityEngine;

public class BeamProjectile : Projectile
{
    [Header("Beam Properties")]
    //protected MobCombat targetMob;
    [SerializeField, Tooltip("The projectile is a beam.")]
    protected bool isBeam = false;
    private float beamRadius;
    private float beamLength = 100; //Ingame beam length
    private float beamWidth = 1; //Ingame beam width
    private LayerMask targetLayer; //Ingame beam width
    [SerializeField]
    private bool isSticky = false;

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
    public float beamEndOffset = 0f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
    public float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. 
                                            //Example: if texture is 200 pixels in height and 600 in length, set this to 3

    public void Init(MobCombat mob, Vector3 target, ScalingStatData statData, GameObject beamStart, 
        float beamRadius, float beamLength, float beamWidth, LayerMask targetLayer, bool isSticky)
    {
        this.target = target;
        this.mob = mob;
        scalingStatData = statData;   
        this.beamStart = beamStart;
        this.beamRadius = beamRadius;
        this.beamLength = beamLength;
        this.beamWidth = beamWidth;
        this.targetLayer = targetLayer;
        this.isSticky = isSticky;
        pierce = true; // Enables pierce for beams
    }

    protected override void Update()
    {
        if (beam) //Updates the beam
        {
            dotTimer += Time.deltaTime;
            line.SetPosition(0, transform.position);

            Vector3 end;
            if (beamCollides && Physics.CapsuleCast(transform.position + (transform.forward * -3f), transform.position + (transform.forward * -2.5f), beamRadius, transform.forward, out RaycastHit hit, beamLength, targetLayer)) //Checks for collision
            {
                if (isSticky)
                {
                    end = hit.point;
                }
                else
                    end = transform.position + (transform.forward * beamLength);

                if (hit.transform.CompareTag(targetTag) && dotTimer >= dotIntervalTimer)
                {
                    dotTimer = 0f;
                    DamageLogic(hit.collider);
                }
            }
            else
            {
                end = transform.position + (transform.forward * beamLength);
            }

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
    }

    public void SpawnBeam() //This function spawns the prefab with linerenderer
    {
        if (beamLineRendererPrefab)
        {
            beam = Instantiate(beamLineRendererPrefab);
            if (beamEndPrefab)
                beamEnd = Instantiate(beamEndPrefab);
            if (beamStartPrefab)
                beamStart = Instantiate(beamStartPrefab);

            beam.transform.position = transform.position;
            beam.transform.parent = transform;
            beam.transform.rotation = transform.rotation;
            line = beam.GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.startWidth = beamWidth;
            line.endWidth = beamWidth;
        }
        else
            print("Add a hecking prefab with a line renderer to the SciFiBeamStatic script on " + gameObject.name + "! Heck!");
    }
}
