using SpaceTarget.Runtime;
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFoundationImplemention :IARBase
{
    private ARCameraManager m_ARCameraManager;
    private ARSession m_ARSession;
    private Texture2D m_CameraTexture;

    public ARFoundationImplemention(ARCameraManager arCameraManager,ARSession session)
    {
        m_ARCameraManager = arCameraManager;
        m_ARSession = session;
    }

    /// <summary>
    /// Get ARKit/ARCore camera intrinsics
    /// </summary>
    /// <returns></returns>
    public ARBaseCameraIntrinsics ARCameraIntrinsics()
    {
        ARBaseCameraIntrinsics camIntrinsics = new ARBaseCameraIntrinsics();
        if (m_ARCameraManager != null)
        {
            if(m_ARCameraManager.TryGetIntrinsics(out XRCameraIntrinsics intrinsics))
            {
                camIntrinsics.focalLength = intrinsics.focalLength;
                camIntrinsics.principalPoint = intrinsics.principalPoint;
                camIntrinsics.resolution = intrinsics.resolution;
            }
        }
        return camIntrinsics;
    }


    /// <summary>
    /// Get camera rawImage(TextureFormat = RGBA32) data
    /// </summary>
    /// <returns></returns>
    public unsafe ARBaseCameraImageData ARCameraRawImageData()
    {
        ARBaseCameraImageData data = new ARBaseCameraImageData();

        if (!m_ARCameraManager.TryGetLatestImage(out XRCameraImage image))
        {
            return data;
        }

        var format = TextureFormat.RGB24;

        if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
        {
            m_CameraTexture = new Texture2D(image.width, image.height, format, false);
        }
        var conversionParams = new XRCameraImageConversionParams(image, format, CameraImageTransformation.None);
        var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            image.Dispose();
        }
        //m_CameraTexture.Apply();

        data.rawImageData = rawTextureData.ToArray();
        data.supportedTextureFormat = SupportedTextureFormat.RGB24;
        data.rawImageOrientation = CameraImageOrientation.LEFT;//Tip: Save raw image to local to see which direction is 

        return data;
    }

    public Pose ARCameraTrackingPose()
    {
        Pose pose = new Pose
        {
            position = m_ARCameraManager.transform.position,
            rotation = m_ARCameraManager.transform.rotation
        };
        return pose;
    }

    public ARBaseSessionTrackingState ARSessionTrackingState()
    {
        ARBaseSessionTrackingState baseTrackingState = ARBaseSessionTrackingState.NONE;

        switch (m_ARSession.subsystem.trackingState)
        {
            case TrackingState.None:
                baseTrackingState = ARBaseSessionTrackingState.NONE;
                break;
            case TrackingState.Limited:
                baseTrackingState = ARBaseSessionTrackingState.LIMITED;
                break;
            case TrackingState.Tracking:
                baseTrackingState = ARBaseSessionTrackingState.TRACKING;
                break;
        }
        return baseTrackingState;
    }
}
