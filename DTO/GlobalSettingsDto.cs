using System.Text.Json.Serialization;

namespace PhoenixLeds.DTO
{
    public class GlobalSettingsDto
    {
        [JsonPropertyName("ledGridWidth")]
        public int LedGridWidth { get; set; }
        
        [JsonPropertyName("ledGridHeight")]
        public int LedGridHeight { get; set; }
        
        [JsonPropertyName("serialPort")]
        public string? SerialPort { get; set; }
        
        [JsonPropertyName("baudRate")]
        public int BaudRate { get; set; }
    }
}
