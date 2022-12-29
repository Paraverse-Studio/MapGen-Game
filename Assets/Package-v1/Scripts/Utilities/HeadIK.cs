using UnityEngine;

namespace Paraverse.IK
{
    public class HeadIK : MonoBehaviour
    {
        private Animator anim;
        public Transform LookAtObj { get { return _lookAtObj; } }
        [SerializeField]
        private Transform _lookAtObj;


        private void Start()
        {
            anim = GetComponent<Animator>();            
        }

        private void OnAnimatorIK()
        {
            if (anim)
            {
                if (_lookAtObj != null) 
                {
                    anim.SetLookAtWeight(1f);
                    anim.SetLookAtPosition(_lookAtObj.position);
                }
                else
                {
                    anim.SetLookAtWeight(0f);
                }
            }
        }

        public void SetLookAtObj(Transform lookAtObj)
        {
            _lookAtObj = lookAtObj;
        }

        public void SetLookAtToNull(Transform lookAtObj)
        {
            _lookAtObj = null;
        }

        private void OnEnable()
        {
            SelectableSystem.Instance?.OnTargetLocked.AddListener(SetLookAtObj);
            SelectableSystem.Instance?.OnTargetUnlocked.AddListener(SetLookAtToNull);
        }

        private void OnDisable()
        {
            SelectableSystem.Instance?.OnTargetLocked.RemoveListener(SetLookAtObj);
            SelectableSystem.Instance?.OnTargetUnlocked.RemoveListener(SetLookAtToNull);
        }
    }
}

