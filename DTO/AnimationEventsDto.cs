using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhoenixLeds.DTO
{
    public class AnimationEventsDto
    {
        [JsonPropertyName("animationEvents")]
        public List<EventDto> AnimationEvents { get; set; } = new List<EventDto>();
    }

    public class EventDto
    {
        [JsonPropertyName("eventName")]
        public string? EventName { get; set; }

        [JsonPropertyName("panels")]
        public List<string> Panels { get; set; } = new List<string>();

        [JsonPropertyName("animationToPlay")]
        public string? AnimationToPlay { get; set; }
    }
}
