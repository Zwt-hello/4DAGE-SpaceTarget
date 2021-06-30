
namespace SpaceTarget.Runtime
{
    /// <summary>
    /// ARBase provider
    /// </summary>
    public interface IARBaseProvider
    {
        /// <summary>
        /// Create 
        /// </summary>
        /// <returns>The implemention of ARBase</returns>
        IARBase Create();
    }
}