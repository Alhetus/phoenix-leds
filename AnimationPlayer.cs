using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixLeds
{
    public class AnimationPlayer
    {
        private readonly LedAnimationModel _ledAnimationModel;
        private readonly LedAnimation _animation; // Animation that the animation player is playing
        private readonly Panel _panel; // Panel that the animation is playing on
        private bool _isPlayingAnimation;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;

        public AnimationPlayer(LedAnimationModel ledAnimationModel, LedAnimation animation, Panel panel) {
            _ledAnimationModel = ledAnimationModel;
            _animation = animation;
            _panel = panel;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
        }

        public void PlayAnimation() {
            _isPlayingAnimation = true;
            Task.Run(RunAnimationAsync);
        }

        private async Task RunAnimationAsync() {
            var updateRateMilliseconds = (int) Math.Round(1 / (float) _animation.FramesPerSecond * 1000);
            var frameIndex = 0;

            while (_isPlayingAnimation) {
                if (frameIndex > _animation.AnimationFrames.Count - 1) {
                    if (_animation.AnimationType == AnimationType.PlayOnce) {
                        _isPlayingAnimation = false;
                        _ledAnimationModel.SetBlackFrameForPanel(_panel);
                        return;
                    }

                    frameIndex = 0;
                }

                // Store panel animation frame in model
                var currentFrame = _animation.AnimationFrames[frameIndex];

                _ledAnimationModel.SetPanelAnimationFrameBytes(_panel, _animation.RotateToFaceCenterPanel ? 
                        currentFrame.GetBytesByPanel(_panel) : currentFrame.PixelBytes);

                // Wait as much as the frame rate of the animation requires
                try {
                    await Task.Delay(updateRateMilliseconds, _token);
                }
                catch (OperationCanceledException) {
                    _ledAnimationModel.SetBlackFrameForPanel(_panel);
                    return;
                }
            }

            _ledAnimationModel.SetBlackFrameForPanel(_panel);
        }

        public void StopAnimation() {
            _isPlayingAnimation = false;
            _tokenSource.Cancel();
        }

        public bool IsPlayingAnimation() {
            return _isPlayingAnimation;
        }
    }
}
