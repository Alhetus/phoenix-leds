namespace PhoenixLeds
{
    public class AnimationFrame
    {
        public byte[] PixelBytes { get; }
        public byte[]? PixelBytesRotated90 { get; }
        public byte[]? PixelBytesRotated180 { get; }
        public byte[]? PixelBytesRotated270 { get; }

        public AnimationFrame(byte[] pixelBytes) {
            PixelBytes = pixelBytes;
        }

        public AnimationFrame(byte[] pixelBytes, byte[] pixelBytesRotated90,
            byte[] pixelBytesRotated180, byte[] pixelBytesRotated270)
        {
            PixelBytes = pixelBytes;
            PixelBytesRotated90 = pixelBytesRotated90;
            PixelBytesRotated180 = pixelBytesRotated180;
            PixelBytesRotated270 = pixelBytesRotated270;
        }
    }
}
