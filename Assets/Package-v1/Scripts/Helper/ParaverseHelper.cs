using System;
using UnityEngine;

namespace Paraverse.Helper
{
    public static class ParaverseHelper
    {
        /// <summary>
        /// Gets the distance between two gameobjects.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float GetDistance(Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }

        /// <summary>
        /// Rotates the gameobject to target direction with given rotation speed.
        /// </summary>
        /// <param name="from">Rotate from</param>
        /// <param name="to">Rotate to</param>
        /// <param name="speed">Rotation Speed</param>
        /// <returns></returns>
        public static Quaternion FaceTarget(Transform from, Transform to, float speed)
        {
            Vector3 targetDir = (to.position - from.position).normalized;
            targetDir = new Vector3(targetDir.x, 0.0f, targetDir.z);
            Quaternion lookRot = Quaternion.LookRotation(targetDir);
            return Quaternion.Slerp(from.rotation, lookRot, speed * Time.deltaTime);
        }
    }
}