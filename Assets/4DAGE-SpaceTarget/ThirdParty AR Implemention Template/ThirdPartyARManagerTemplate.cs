using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTarget.Runtime;

public class ThirdPartyARManagerTemplate : MonoBehaviour
{
    [SerializeField] SpaceTargetBehaviour spaceTargetBehaviour;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (spaceTargetBehaviour != null)
        {
            IARBaseProvider mARProvider = new ThirdPartyARProviderTemplate();
            spaceTargetBehaviour.StartTracking(mARProvider);
        }
    }

    private void OnDestroy()
    {
        if (spaceTargetBehaviour != null)
        {
            spaceTargetBehaviour.StopTracking();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
