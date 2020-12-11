using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PhoenixLeds
{
    public static class TextureLoader
    {
        /// <summary>
        /// Loads animation frames from a texture using the specified frameWidth and frameHeight.
        /// Will resize the frames to LedGridWidth and LedGridHeight defined in global settings.
        /// </summary>
        public static List<AnimationFrame> LoadAnimationFramesFromTexture(int frameWidth, int frameHeight,
            string texturePath, bool createRotatedFrames)
        {
            if (!File.Exists(texturePath)) {
                Console.WriteLine($"Error: Texture '{texturePath}' does not exist!");
                return new List<AnimationFrame>();
            }

            try {
                using Image<Rgb24> image = Image.Load<Rgb24>(texturePath);

                var animationFrames = new List<AnimationFrame>();

                var horizontalFrameCount = image.Width / frameWidth;
                var verticalFrameCount = image.Height / frameHeight;
                var animationFrameWidth = GlobalSettings.LedGridWidth;
                var animationFrameHeight = GlobalSettings.LedGridHeight;

                for (var yIndex = 0; yIndex < verticalFrameCount; yIndex++) {
                    for (var xIndex = 0; xIndex < horizontalFrameCount; xIndex++) {
                        using Image<Rgb24> clone = image.Clone();

                        var x = xIndex * frameWidth;
                        var y = yIndex * frameHeight;

                        var cropRect = new Rectangle(x, y, frameWidth, frameHeight);

                        // Crop the image to the current frame and resize the cropped image to animation frame size
                        clone.Mutate(img => img
                            .Crop(cropRect)
                            .Resize(animationFrameWidth, animationFrameHeight, KnownResamplers.Welch));

                        // Get raw bytes of pixel data of the generated image
                        var pixelBytes = GetImageBytes(clone);

                        // No rotation needed, just add the frame with not-rotated byte array
                        if (!createRotatedFrames)
                            animationFrames.Add(new AnimationFrame(pixelBytes));
                        // Create the rotated byte arrays
                        else {
                            clone.Mutate(img => img.Rotate(90));
                            var pixelBytesRotated90 = GetImageBytes(clone);

                            clone.Mutate(img => img.Rotate(90));
                            var pixelBytesRotated180 = GetImageBytes(clone);

                            clone.Mutate(img => img.Rotate(90));
                            var pixelBytesRotated270 = GetImageBytes(clone);

                            animationFrames.Add(new AnimationFrame(pixelBytes, pixelBytesRotated90,
                                pixelBytesRotated180, pixelBytesRotated270));
                        }
                    }
                }

                return animationFrames;
            }
            catch (Exception e) {
                Console.WriteLine($"Error: Could not load texture '{texturePath}': {e.Message}");
                return new List<AnimationFrame>();
            }
        }

        private static byte[] GetImageBytes(Image<Rgb24> clone) {
            var memoryGroup = clone.GetPixelMemoryGroup();
            var pixelData = memoryGroup
                .SelectMany(row => MemoryMarshal.AsBytes(row.Span).ToArray())
                .ToArray();

            return pixelData;
        }
    }
}
