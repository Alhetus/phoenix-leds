namespace PhoenixLeds
{
    public class AnimationFrame
    {
        public byte[] PixelBytes { get; set; }

        public AnimationFrame(int frameWidth, int frameHeight)
        {
            // Init the array with enough capacity to hold pixel data of one frame of animation. Use 24bit pixel values.
            // For example 12x12 pixels 24bit each (RGB channels 8 bit each)
            // 24 / 8 = 3 bytes
            PixelBytes = new byte[frameWidth * frameHeight * 3];
        }
    }
}
