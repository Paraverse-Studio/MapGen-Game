using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        [SerializeField, Tooltip("Speed of the projectile.")]
        private string targetTag = "Player";
        [SerializeField, Tooltip("Speed of the projectile.")]
        private float speed;
        [SerializeField, Tooltip("Range of the projectile.")]
        private float range;
        [SerializeField, Tooltip("Range of the projectile.")]
        private float damage;
        [SerializeField, Tooltip("Projectile is destroyed after this duration.")]
        private float deathTimer = 5f;
        private float curdeathTimer = 0f;
        private Vector3 origin;
        #endregion

        #region Start & Update
        private void Start()
        {
            origin = transform.position;
        }

        private void Update()
        {
            float distanceFromOrigin = ParaverseHelper.GetDistance(transform.position, origin);
            if (distanceFromOrigin > range || curdeathTimer >= deathTimer)
            {
                Destroy(gameObject);
            }

            curdeathTimer += Time.deltaTime;
            transform.position += (transform.forward * speed * Time.deltaTime);
        }
        #endregion

        public void Init(float speed, float range, float damage)
        {
            this.speed = speed;
            this.range = range;
            this.damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                IMobController controller = other.GetComponent<IMobController>();
                controller.Stats.UpdateCurrentHealth((int)-damage);
                controller.ApplyHitAnimation();
                Destroy(gameObject);
            }
        }
    }
}