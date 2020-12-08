using System.Text.Json.Serialization;

namespace PhoenixLeds.DTO
{
    public class GlobalSettingsDto
    {
        [JsonPropertyName("ledGridSize")]
        public int LedGridSize { get; set; }
    }
}
