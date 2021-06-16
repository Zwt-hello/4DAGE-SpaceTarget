using UnityEngine.XR.ARFoundation;
using SpaceTarget.Runtime;

public class ARFoundationProvider :IARBaseProvider
{
    private ARCameraManager mARCameraManager;
    private ARSession mARSession;

    public ARFoundationProvider(ARCameraManager arcameraManger,ARSession session)
    {
        mARCameraManager = arcameraManger;
        mARSession = session;
    }
    public IARBase Create()
    {
        return new ARFoundationImplemention(mARCameraManager, mARSession);
    }
    
}
