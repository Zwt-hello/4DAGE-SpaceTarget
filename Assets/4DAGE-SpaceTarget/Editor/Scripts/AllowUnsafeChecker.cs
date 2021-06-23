using UnityEditor;

public class AllowUnsafeChecker 
{
    [InitializeOnLoadMethod]
    private static void AllowUCSetting()
    {
        if (!PlayerSettings.allowUnsafeCode)
            PlayerSettings.allowUnsafeCode = true;
    }
}
