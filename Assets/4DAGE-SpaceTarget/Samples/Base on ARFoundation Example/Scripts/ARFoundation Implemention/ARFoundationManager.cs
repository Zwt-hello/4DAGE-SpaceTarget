﻿using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using SpaceTarget.Runtime;
using System.Collections;

public class ARFoundationManager : MonoBehaviour
{
    [SerializeField] SpaceTargetBehaviour m_SpaceTarget;
    [SerializeField] ARCameraManager m_ARCameraManager;
    [SerializeField] ARSession m_ARSession;

    [SerializeField] ARAnchorManager m_ARAnchorManager;
    private ARAnchor mAnchor;
    private Transform mAnchorTarget;

    private void Start()
    {
        StartCoroutine(CheckDeviceSupport(() => Initialization()));
    }
    private void OnDestroy()
    {
        m_SpaceTarget.StopTracking();
    }
    private void Initialization()
    {
        IARBaseProvider mARProvider = new ARFoundationProvider(m_ARCameraManager, m_ARSession);
        m_SpaceTarget.StartTracking(mARProvider);

        //Use AR Anchor to anchored the target
        m_SpaceTarget.OnTargetPoseChange += ((target, pose) =>
          {
              m_ARAnchorManager.AddAnchor(pose);
              mAnchorTarget = target;
          });

        if (mAnchorTarget != null)
        {
            RegisterARAnchorChangeEvent(mAnchorTarget);
        }

        //ARFoundation setting
        m_ARCameraManager.focusMode = CameraFocusMode.Fixed;
        ARSession.stateChanged += (sessionState) =>
        {
            if (sessionState.state == ARSessionState.SessionTracking)
                SetCameraConfigurations();
        };
    }

    bool camSeted = false;
    public void SetCameraConfigurations()
    {
        if (camSeted)
            return;

        //auto focus
        m_ARCameraManager.focusMode = CameraFocusMode.Fixed;
        using (var configurations = m_ARCameraManager.GetConfigurations(Allocator.Temp))
        {
            int index = 0;
            int compareH = 0;
            for (int i = 0; i < configurations.Length; i++)
            {
                Debug.LogFormat("Camera configurations {0}：{1}", i, configurations[i].resolution);
                //取最小分辨率
                if (i == 0)
                {
                    //init compareW
                    compareH = configurations[i].height;
                    index = i;
                }
                else
                {
                    if (compareH > configurations[i].height)
                    {
                        compareH = configurations[i].height;
                        index = i;
                    }
                }
            }
            var configuration = configurations[index];
            m_ARCameraManager.currentConfiguration = configuration;
            Debug.LogFormat("Set CPU Image resolution：{0}", configuration.resolution);
            camSeted = true;
        }
    }

    IEnumerator CheckDeviceSupport(System.Action callback)
    {
        if(ARSession.state == ARSessionState.None||ARSession.state == ARSessionState.CheckingAvailability)
        {
            yield return ARSession.CheckAvailability();
        }
        if(ARSession.state == ARSessionState.Unsupported)
        {
            GameObject tip = Instantiate(Resources.Load("UnSupportTipUI") as GameObject);
            yield return new WaitForSeconds(5f);
            Destroy(tip);
        }
        else
        {
            callback?.Invoke();
        }
    } 
    private void RegisterARAnchorChangeEvent(Transform anchorTarget)
    {
        m_ARAnchorManager.anchorsChanged += (args) =>
        {
            if (mAnchor == null) return;
            foreach(var anchor in args.updated)
            {
                if (mAnchor.trackableId == anchor.trackableId)
                {
                    anchorTarget.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
                }
            }
        };
    }
}
