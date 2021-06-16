
namespace SpaceTarget.Runtime
{
    public enum ARBaseSessionTrackingState 
    {
        /// <summary>
        /// AR session tracking reason none
        /// </summary>
        NONE = 0,

        /// <summary>
        /// AR session tracking state is limited, such as tracking lost
        /// </summary>
        LIMITED = 1,

        /// <summary>
        /// AR session tracking state is tracking
        /// </summary>
        TRACKING = 2
    }
}