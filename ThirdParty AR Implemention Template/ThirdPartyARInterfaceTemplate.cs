using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTarget.Runtime;

public class ThirdPartyARInterfaceTemplate : IARBase
{
    public virtual ARBaseSessionTrackingState ARSessionTrackingState()
    {
        throw new System.NotImplementedException();
        //to do
    }

    public virtual ARBaseData.Intrinsics ARCameraIntrinsics()
    {
        throw new System.NotImplementedException();
        //to do
    }

    public virtual ARBaseData.CameraImageData ARCameraRawImageData()
    {
        throw new System.NotImplementedException();
        //to do
    }

    public virtual Pose ARCameraTrackingPose()
    {
        throw new System.NotImplementedException();
        //to do
    }
}
