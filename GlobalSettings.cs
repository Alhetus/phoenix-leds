using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PhoenixLeds.DTO;

namespace PhoenixLeds
{
    public static class GlobalSettings
    {
        /// <summary>How many pixels exist in the led grid, for example in 12x12 grid this would be 12.</summary>
        public static int LedGridSize { get; private set; } = 12;
        public static string SerialPort { get; private set; } = "";
        public static int BaudRate { get; private set; } = 12582912;

        public static async Task LoadAsync() {
            var settingsFilePath = Path.Combine(".", "settings.json");

            if (!File.Exists(settingsFilePath)) {
                Console.WriteLine("Error: Could not find settings.json! Using default values.");
                return;
            }

            var json = await File.ReadAllTextAsync(settingsFilePath, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json)) {
                Console.WriteLine("Error: settings.json is empty! Using default values.");
                return;
            }

            GlobalSettingsDto globalSettingsDto;

            try {
                globalSettingsDto = JsonSerializer.Deserialize<GlobalSettingsDto>(json);
            }
            catch (JsonException e) {
                Console.WriteLine($"Could not parse settings.json: {e.Message}");
                return;
            }

            LedGridSize = globalSettingsDto.LedGridSize;
            SerialPort = globalSettingsDto.SerialPort;
            BaudRate = globalSettingsDto.BaudRate;

            Console.WriteLine("Loaded settings.");
        }
    }
}
