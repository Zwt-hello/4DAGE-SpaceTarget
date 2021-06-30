using UnityEngine;

namespace SpaceTarget.Runtime
{
    /// <summary>
    /// AR camera intrinsics
    /// </summary>
    public struct ARBaseCameraIntrinsics
    {
        /// <summary>
        /// Raw image resolution
        /// </summary>
        public Vector2Int resolution;

        /// <summary>
        /// Camera's focal length
        /// </summary>
        public Vector2 focalLength;

        /// <summary>
        /// Raw image principal point
        /// </summary>
        public Vector2 principalPoint;
    }
}