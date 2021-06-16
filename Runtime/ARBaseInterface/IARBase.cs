using UnityEngine;

namespace SpaceTarget.Runtime
{
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
        ARBaseData.Intrinsics ARCameraIntrinsics();

        /// <summary>
        /// ARCamera raw image data , 
        /// Please select the corresponding TextureFormat(RGBA32 or RGB24) to implement
        /// </summary>
        /// <returns></returns>
        ARBaseData.CameraImageData ARCameraRawImageData();

        /// <summary>
        /// ARSession tracking state
        /// </summary>
        /// <returns></returns>
        ARBaseSessionTrackingState ARSessionTrackingState();
        
    }
}