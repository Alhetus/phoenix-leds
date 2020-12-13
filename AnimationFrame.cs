using System;

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

        public byte[] GetBytesByPanel(Panel panel) {
            return panel switch {
                Panel.Left => PixelBytesRotated270 ?? new byte[0],
                Panel.Down => PixelBytesRotated180 ?? new byte[0],
                Panel.Up => PixelBytes,
                Panel.Right => PixelBytesRotated90 ?? new byte[0],
                _ => throw new ArgumentOutOfRangeException(nameof(panel), panel, null)
            };
        }
    }
}
