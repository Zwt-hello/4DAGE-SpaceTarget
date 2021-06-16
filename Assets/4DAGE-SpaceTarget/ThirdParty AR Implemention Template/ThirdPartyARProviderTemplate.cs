using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTarget.Runtime;

public class ThirdPartyARProviderTemplate : IARBaseProvider
{
    public IARBase Create()
    {
        return new ThirdPartyARInterfaceTemplate();
    }
}
