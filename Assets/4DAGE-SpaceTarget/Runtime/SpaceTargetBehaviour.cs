using UnityEngine;
using System;

namespace SpaceTarget.Runtime
{
    /// <summary>
    /// SpaceTarget tracking behaviour
    /// </summary>
    public sealed class SpaceTargetBehaviour :SpaceTargetSubsystem
    {
        /// <summary>
        /// Callback when found 
        /// </summary>
        public event Action OnTargetFound;

        /// <summary>
        /// Callback when lost
        /// </summary>
        public event Action OnTargetLost;

        /// <summary>
        /// Callback while pose changed
        /// </summary>
        public event Action<Transform, Pose> OnTargetPoseChange;

        #region Plubic Methods

        /// <summary>
        /// Start space target subsystem
        /// </summary>
        /// <param name="mARProvider">AR provider</param>
        public void StartTracking(IARBaseProvider mARProvider)
        {

#if UNITY_EDITOR
            Debug.Log("--Editor mode does not support SpaceTarget,Please run on Android / iOS mobile device.--");
#else
            m_ARBase = mARProvider.Create();
            SubsystemStart(OnTargetFound, OnTargetLost, OnTargetPoseChange);
#endif
        }

        /// <summary>
        /// Stop space target subsystem
        /// </summary>
        public void StopTracking()
        {
            m_ARBase = null;
            SubsystemStop();
        }

        /// <summary>
        /// Regist Target Tracking event
        /// </summary>
        /// <param name="TargetFoundCallback">When Target found</param>
        /// <param name="TargetLostCallback">When Target lost</param>
        public void RegisterTargetStatusEvent(Action TargetFoundCallback, Action TargetLostCallback)
        {
            OnTargetFound += TargetFoundCallback;
            OnTargetLost += TargetLostCallback;
        }

        /// <summary>
        /// Unregist Target Tracking event
        /// </summary>
        /// <param name="TargetFoundCallback">When target found</param>
        /// <param name="TargetLostCallback">When target lost</param>
        public void UnregisterTargetLostEvent(Action TargetFoundCallback, Action TargetLostCallback)
        {
            OnTargetFound -= TargetFoundCallback;
            OnTargetLost -= TargetLostCallback;
        }

#endregion

#region  Private Methods

        /// <summary>
        /// AR Instance , which is implementioned
        /// </summary>
        private IARBase m_ARBase;
        private ARBaseCameraIntrinsics m_ARCameraIntrinsics;
        private ARBaseCameraImageData m_ARCameraImageData;

        /// <summary>
        /// Frame Call
        /// </summary>
        private void OnUpdate()
        {
            if (m_ARBase != null)
            {
                if (m_ARBase.ARSessionTrackingState() == ARBaseSessionTrackingState.TRACKING)
                {
                    if (IsSubsystemProcessing())
                    {
                        m_ARCameraIntrinsics = m_ARBase.ARCameraIntrinsics();
                        m_ARCameraImageData = m_ARBase.ARCameraRawImageData();
                    }
                    Pose camPose = m_ARBase.ARCameraTrackingPose();

                    SpaceTargetInternalParm spInternalParams = new SpaceTargetInternalParm
                    {
                        cameraIntrinsis = new SPIntrinsics 
                        {
                            focalLength = m_ARCameraIntrinsics.focalLength,
                            principalPoint = m_ARCameraIntrinsics.principalPoint,
                            resolution = m_ARCameraIntrinsics.resolution
                        },
                        cameraRawImageData = new SPImageData 
                        {
                            rawImageData = m_ARCameraImageData.rawImageData,
                            supportedTextureFormat = (int)m_ARCameraImageData.supportedTextureFormat,
                            rawImageOrientation = (int)m_ARCameraImageData.rawImageOrientation
                        },
                        cameraTrackingPose = camPose
                    };

                    OnXRTracking(spInternalParams);
                }

                if (m_ARBase.ARSessionTrackingState() == ARBaseSessionTrackingState.LIMITED)
                {
                    OnXRTrackingLost();
                }
            }
        }

        private void Update()
        {
#if !UNITY_EDITOR
            OnUpdate();
#endif
        }

#endregion
    }
}