using UnityEngine;

namespace SpaceTarget.Runtime
{
    public class ARBaseData
    {
        /// <summary>
        /// ARCamera intrinsics
        /// </summary>
        public struct Intrinsics
        {
            public Vector2Int resolution;
            public Vector2 focalLength;
            public Vector2 principalPoint;
        }

        /// <summary>
        /// ARCamera raw texture data
        /// </summary>
        public struct CameraImageData
        {
            public byte[] rawImageData;
            public SupportedTextureFormat supportedTextureFormat;
            public CameraImageOrientation rawImageOrientation;
        }
        
    }
}