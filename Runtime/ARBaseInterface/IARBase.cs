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
        /// <returns>ARCamera pose</returns>
        Pose ARCameraTrackingPose();

        /// <summary>
        /// ARCamera intrinsics
        /// </summary>
        /// <returns>Camera intrinsics</returns>
        ARBaseCameraIntrinsics ARCameraIntrinsics();

        /// <summary>
        /// ARCamera raw image data , 
        /// Please select the corresponding TextureFormat(RGBA32 or RGB24) to implement
        /// </summary>
        /// <returns>Camera raw image data</returns>
        ARBaseCameraImageData ARCameraRawImageData();

        /// <summary>
        /// ARSession tracking state
        /// </summary>
        /// <returns>ARSession tracking state</returns>
        ARBaseSessionTrackingState ARSessionTrackingState();
        
    }
}