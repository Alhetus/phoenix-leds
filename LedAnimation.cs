using System.Collections.Generic;

namespace PhoenixLeds
{
    public class LedAnimation
    {
        public string Name { get; }
        public AnimationType AnimationType { get; }
        public bool RotateToFaceCenterPanel { get; }
        public int AnimationFrameWidth { get; }
        public int AnimationFrameHeight { get; }
        public List<AnimationFrame> AnimationFrames { get; }
        public int FramesPerSecond { get; }

        public LedAnimation(string name, AnimationType animationType, bool rotateToFaceCenterPanel,
            int animationFrameWidth, int animationFrameHeight, List<AnimationFrame> animationFrames,
            int framesPerSecond)
        {
            Name = name;
            AnimationType = animationType;
            RotateToFaceCenterPanel = rotateToFaceCenterPanel;
            AnimationFrameWidth = animationFrameWidth;
            AnimationFrameHeight = animationFrameHeight;
            AnimationFrames = animationFrames;
            FramesPerSecond = framesPerSecond;
        }
    }
}
