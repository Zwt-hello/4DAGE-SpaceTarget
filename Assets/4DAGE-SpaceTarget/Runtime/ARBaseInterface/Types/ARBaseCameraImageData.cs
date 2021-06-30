
namespace SpaceTarget.Runtime
{
    /// <summary>
    /// AR camera raw image data struct
    /// </summary>
    public struct ARBaseCameraImageData
    {
        /// <summary>
        /// raw image byte array
        /// </summary>
        public byte[] rawImageData;

        /// <summary>
        /// Color with texture format, Please select the corresponding textureFormat(RGBA32 or RGB24) of raw image data
        /// </summary>
        public SupportedTextureFormat supportedTextureFormat;

        /// <summary>
        /// The orientation of raw image
        /// </summary>
        public CameraImageOrientation rawImageOrientation;
    }
}