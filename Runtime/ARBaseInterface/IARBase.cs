using UnityEngine;

namespace SpaceTarget.Runtime
{
    /// <summary>
    /// SpaceTarget's basic AREngine interface
    /// </summary>
    public interface IARBase
    {
        /// <summary>
        /// The realtime pose of ARCamera
        /// </summary>
        /// <returns></returns>
        Pose ARCameraTrackingPose();

        /// <summary>
        /// ARCamera intrinsics
        /// </summary>
        /// <returns></returns>
        ARBaseCameraIntrinsics ARCameraIntrinsics();

        /// <summary>
        /// ARCamera raw image data , 
        /// Please select the corresponding TextureFormat(RGBA32 or RGB24) to implement
        /// </summary>
        /// <returns></returns>
        ARBaseCameraImageData ARCameraRawImageData();

        /// <summary>
        /// ARSession tracking state
        /// </summary>
        /// <returns></returns>
        ARBaseSessionTrackingState ARSessionTrackingState();
        
    }
}