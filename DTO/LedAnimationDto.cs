using System.Text.Json.Serialization;

namespace PhoenixLeds.DTO
{
    public class LedAnimationDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("animationType")]
        public AnimationType AnimationType { get; set; } = AnimationType.PlayOnce;

        [JsonPropertyName("rotateToFaceCenterPanel")]
        public bool RotateToFaceCenterPanel { get; set; } = true;

        [JsonPropertyName("animationFrameWidth")]
        public int AnimationFrameWidth { get; set; } = 12;

        [JsonPropertyName("animationFrameHeight")]
        public int AnimationFrameHeight { get; set; } = 12;

        [JsonPropertyName("animationTexturePath")]
        public string? AnimationTexturePath { get; set; }

        [JsonPropertyName("framesPerSecond")]
        public int FramesPerSecond { get; set; } = 24;
    }
}
