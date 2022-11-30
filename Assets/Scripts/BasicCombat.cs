using UnityEngine;

namespace Paraverse.Enemy
{
    public class BasicCombat : MonoBehaviour, ICombat
    {
        // important unity components
        private Animator anim;

        [Header("Target Values")]
        [SerializeField, Tooltip("Target tag (Player by default).")]
        private string targetTag = "Player";
        private Transform player;

        [Header("Basic Attack Values")]
        [SerializeField, Tooltip("Basic attack damage.")]
        private float basicAttackDamage = 5f;
        [SerializeField, Tooltip("Basic attack range.")]
        private float basicAttackRange = 2f;
        public float BasicAttackRange { get { return basicAttackRange; } }
        [SerializeField, Tooltip("Cooldown in between each basic attack.")]
        private float basicAttackCooldown = 3f;
        private float curBasicAttackCooldown;
        [SerializeField, Tooltip("Basic attack weapon collider.")]
        private GameObject basicAttackCollider;
        [SerializeField, Tooltip("AttackCollider script of Basic attack collider.")]
        private AttackCollider basicAttackColliderScript;
        [SerializeField, Tooltip("Name of basic attack animation.")]
        private string basicAttackAnimationName = "BasicAttack";

        [Header("Projectile Values")]
        [SerializeField, Tooltip("Basic Attack Projectile Prefab")]
        private GameObject projPf;
        [SerializeField, Tooltip("The projectile's origin position.")]
        private Transform projOrigin;
        [SerializeField, Tooltip("The projectile's speed.")]
        private float basicAttackProjSpeed = 10f;

        // Constantly updates the distance from player
        private float distanceFromPlayer;

        // Sets to true when character is doing an action (Attack, Stun).
        private bool isInteracting = false;
        public bool IsInteracting { get { return isInteracting; } }
        
        // Returns true when character is within basic attack range and cooldown is 0.
        public bool CanBasicAttack { get { return distanceFromPlayer <= basicAttackRange && curBasicAttackCooldown <= 0; } }


        protected virtual void Start()
        {
            if (basicAttackCollider == null)
            {
                Debug.LogError(gameObject.name + " needs to have a basic attack collider!");
                return;
            }
            if (anim == null) anim = GetComponent<Animator>();
            if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();

            // Gets and initiates attack collider script on the enemy 
            distanceFromPlayer = Vector3.Distance(transform.position, player.position);
            basicAttackCollider.SetActive(true);
            basicAttackColliderScript = basicAttackCollider.GetComponent<AttackCollider>();
            basicAttackColliderScript.Init(basicAttackDamage);
            basicAttackCollider.SetActive(false);
        }

        private void Update()
        {
            distanceFromPlayer = Vector3.Distance(transform.position, player.position);
            AttackCooldownHandler();
            
            // used to control player movement - isInteracting => curSpeed = 0f
            isInteracting = anim.GetBool("isInteracting");
        }

        /// <summary>
        /// Responsible for handling basic attack animation and cooldown.
        /// </summary>
        public virtual void BasicAttackHandler()
        {
            if (curBasicAttackCooldown <= 0)
            {
                isInteracting = true;
                anim.Play(basicAttackAnimationName);
                curBasicAttackCooldown = basicAttackCooldown;
            }
        }

        /// <summary>
        /// Handles basic attack cooldowns
        /// </summary>
        protected virtual void AttackCooldownHandler()
        {
            curBasicAttackCooldown -= Time.deltaTime;
            curBasicAttackCooldown = Mathf.Clamp(curBasicAttackCooldown, 0f, basicAttackCooldown);
        }

        #region Animation Event Methods
        /// <summary>
        /// Enables basic weapon collider.
        /// </summary>
        protected void EnableBasicAttackCollider()
        {
            basicAttackCollider.SetActive(true);
        }

        /// <summary>
        /// Disables basic attack collider.
        /// </summary>
        protected void DisableBasicAttackCollider()
        {
            basicAttackCollider.SetActive(false);
        }

        /// <summary>
        /// Fires a projectile.
        /// </summary>
        public void FireProjectile()
        {
            GameObject go = Instantiate(projPf, projOrigin.position, transform.rotation);
            Projectile proj = go.GetComponent<Projectile>();
            proj.Init(basicAttackProjSpeed, basicAttackRange, basicAttackDamage);
        }
        #endregion
    }
}