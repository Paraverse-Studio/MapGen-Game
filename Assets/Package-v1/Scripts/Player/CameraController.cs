using UnityEngine;

namespace Paraverse.Player
{
    public class CameraController : MonoBehaviour
    {
        #region Variables
        private Camera cam;
        [Tooltip("The target the camera will look at.")]
        private Transform target;
        [SerializeField, Tooltip("The target tag to fetch transform for target.")]
        private string targetTag = "Player";

        #endregion

        #region Start & Update Methods
        private void Start()
        {
            cam = Camera.main;
            target = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
        }

        private void Update()
        {
            LookAt(target);
        }
        #endregion

        #region Camera Methods
        private void LookAt(Transform target)
        {
            cam.transform.LookAt(target);
        }
        #endregion
    }
}
