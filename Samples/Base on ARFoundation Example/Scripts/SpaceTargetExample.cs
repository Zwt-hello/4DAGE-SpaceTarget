using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceTargetExample : DefaultTrackableEventHandler
{
    [SerializeField] Text logText;
    public override void OnTrackingFound()
    {
        base.OnTrackingFound();
        logText.text = "Target Found";
    }
    public override void OnTrackingLost()
    {
        base.OnTrackingLost();
        logText.text = "Target Lost";
    }
   
}
