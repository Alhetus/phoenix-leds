using System;
using System.Collections.Generic;
using System.IO;
using PhoenixLeds.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PhoenixLeds
{
    public static class TextureLoader
    {
        public static List<AnimationFrame> LoadTexture(LedAnimationDto animationDto) {
            if (!File.Exists(animationDto.AnimationTexturePath)) {
                Console.WriteLine($"Error: Texture '{animationDto.AnimationTexturePath}' does not exist!");
                return new List<AnimationFrame>();
            }

            var frameWidth = animationDto.AnimationFrameWidth;
            var frameHeight = animationDto.AnimationFrameHeight;

            try {
                using Image<Rgb24> image = Image.Load<Rgb24>(animationDto.AnimationTexturePath);

                var animationFrames = new List<AnimationFrame>();

                var horizontalFrameCount = image.Width / frameWidth;
                var verticalFrameCount = image.Height / frameHeight;
                var animationFrameSize = GlobalSettings.LedGridSize;

                for (var yIndex = 0; yIndex < verticalFrameCount; yIndex++) {
                    for (var xIndex = 0; xIndex < horizontalFrameCount; xIndex++) {
                        using Image<Rgb24> copy = image.Clone();

                        var x = xIndex * frameWidth;
                        var y = yIndex * frameHeight;

                        var cropRect = new Rectangle(x, y, frameWidth, frameHeight);

                        copy.Mutate(img => img
                            .Crop(cropRect)
                            .Resize(animationFrameSize, animationFrameSize, KnownResamplers.Welch));
                        
                        
                    }
                }

                return animationFrames;
            }
            catch (NotSupportedException e) {
                Console.WriteLine($"Error: Could not load texture '{animationDto.AnimationTexturePath}': {e.Message}");
                return new List<AnimationFrame>();
            }
        }
    }
}