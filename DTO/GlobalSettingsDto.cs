using System.Text.Json.Serialization;

namespace PhoenixLeds.DTO
{
    public class GlobalSettingsDto
    {
        [JsonPropertyName("ledGridSize")]
        public int LedGridSize { get; set; }
        
        [JsonPropertyName("serialPort")]
        public string SerialPort { get; set; }
        
        [JsonPropertyName("baudRate")]
        public int BaudRate { get; set; }
    }
}
