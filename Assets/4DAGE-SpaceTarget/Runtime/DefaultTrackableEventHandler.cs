using UnityEngine;
using UnityEngine.Events;
using ARBase.Runtime;

public class DefaultTrackableEventHandler : MonoBehaviour
{
    protected SpaceTargetTrackableBehaviour mSpaceTargetTrackableBehaviour;

    public UnityEvent OnTargetFound;
    public UnityEvent OnTargetLost;

    protected virtual void Start()
    {
        mSpaceTargetTrackableBehaviour = GetComponent<SpaceTargetTrackableBehaviour>();
        if (mSpaceTargetTrackableBehaviour)
        {
            mSpaceTargetTrackableBehaviour.RegisterTrackingStatusEvent(OnTrackingFound, OnTrackingLost);
        }
    }
    protected virtual void OnDestroy()
    {
        if (mSpaceTargetTrackableBehaviour)
        {
            mSpaceTargetTrackableBehaviour.UnregisterTrackingLostEvent(OnTrackingFound, OnTrackingLost);
        }
    }
    public virtual void OnTrackingFound()
    {
        var rendererComponents = mSpaceTargetTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = mSpaceTargetTrackableBehaviour.GetComponentsInChildren<Collider>(true);
        var canvasComponents = mSpaceTargetTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;

        OnTargetFound?.Invoke();
        Debug.Log("Space Target Found");
    }
    public virtual void OnTrackingLost()
    {
        var rendererComponents = mSpaceTargetTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = mSpaceTargetTrackableBehaviour.GetComponentsInChildren<Collider>(true);
        var canvasComponents = mSpaceTargetTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;

        OnTargetLost?.Invoke();
        Debug.Log("Space Target Lost");
    }
}
