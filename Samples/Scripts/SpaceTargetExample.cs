using UnityEngine;
using UnityEngine.UI;

public class SpaceTargetExample : DefaultTrackableEventHandler
{
    [SerializeField] Text logText;

    public override void OnTrackingFound()
    {
        base.OnTrackingFound();
        logText.text = "Tracking Found";
    }
    public override void OnTrackingLost()
    {
        base.OnTrackingLost();
        logText.text = "Tracking Lost";
    }
}
