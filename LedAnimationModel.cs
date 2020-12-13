using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoenixLeds
{
    public class LedAnimationModel
    {
        private readonly ConcurrentDictionary<Panel, byte[]> _panelAnimationFrameBytes = new ConcurrentDictionary<Panel, byte[]>();
        private List<LedAnimation> LedAnimations { get; set; } = new List<LedAnimation>();
        private Dictionary<Panel, AnimationPlayer> PlayingAnimations { get; set; } = new Dictionary<Panel, AnimationPlayer>();
        private readonly int _updateRateMilliseconds;
        private readonly byte[] _blackFrame; 

        public LedAnimationModel(int updateLedsFramesPerSecond) {
            // Load initial black frames to panel animation frames
            _blackFrame = GetBlackFrame(GlobalSettings.LedGridWidth, GlobalSettings.LedGridHeight);

            _panelAnimationFrameBytes[Panel.Left] = _blackFrame;
            _panelAnimationFrameBytes[Panel.Down] = _blackFrame;
            _panelAnimationFrameBytes[Panel.Up] = _blackFrame;
            _panelAnimationFrameBytes[Panel.Right] = _blackFrame;

            _updateRateMilliseconds = (int) Math.Round(1 / (float) updateLedsFramesPerSecond * 1000);
            Console.WriteLine($"Led animation update loop running at {updateLedsFramesPerSecond}fps");
            Task.Run(UpdateLoop);
        }

        private static byte[] GetBlackFrame(int ledGridWidth, int ledGridHeight) {
            // 3 bytes per RGB color
            var byteCount = ledGridWidth * ledGridHeight * 3;
            var blackFrame = new byte[byteCount];

            for (var i = 0; i < byteCount; i++) {
                blackFrame[i] = 0x00;
            }

            return blackFrame;
        }

        public void SetLedAnimations(List<LedAnimation> ledAnimations) {
            LedAnimations = ledAnimations;
        }

        public void SetPanelAnimationFrameBytes(Panel panel, byte[] bytes) {
            _panelAnimationFrameBytes[panel] = bytes;
        }

        public void SetBlackFrameForPanel(Panel panel) {
            _panelAnimationFrameBytes[panel] = _blackFrame;
        }

        public void SetPlayingAnimation(Panel panel, AnimationPlayer animationPlayer) {
            PlayingAnimations[panel] = animationPlayer;
        }

        public bool IsAnimationPlayingOnPanel(Panel panel) {
            return PlayingAnimations.ContainsKey(panel) &&
                   PlayingAnimations[panel] != null &&
                   PlayingAnimations[panel].IsPlayingAnimation();
        }

        public AnimationPlayer? GetPlayingAnimation(Panel panel) {
            return PlayingAnimations[panel];
        }

        public LedAnimation? GetAnimationByName(string animationName) {
            return LedAnimations.SingleOrDefault(x => x.Name == animationName);
        }

        private async Task UpdateLoop() {
            while (true) {
                // Concat all panel byte arrays together in correct order
                byte[] fullFrameBytes = ConcatArrays(
                    _panelAnimationFrameBytes[Panel.Left],
                    _panelAnimationFrameBytes[Panel.Down],
                    _panelAnimationFrameBytes[Panel.Up],
                    _panelAnimationFrameBytes[Panel.Right]
                );

                // Write the full frame over serial to the teensy
                SerialCommunicator.WriteFullAnimationFrame(fullFrameBytes);

                // Delay according to the update rate
                await Task.Delay(_updateRateMilliseconds);
            }
        }

        private static T[] ConcatArrays<T>(params T[][] arrays) {
            var result = new T[arrays.Sum(a => a.Length)];
            var offset = 0;

            foreach (var t in arrays) {
                t.CopyTo(result, offset);
                offset += t.Length;
            }

            return result;
        }
    }
}
