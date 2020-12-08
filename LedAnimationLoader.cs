using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PhoenixLeds.DTO;

namespace PhoenixLeds
{
    public class LedAnimationLoader
    {
        public static async Task LoadAnimationsAsync() {
            var animationsPath = Path.Combine(".", "Animations");

            if (!Directory.Exists(animationsPath)) {
                Console.WriteLine($"Error: Animations directory does not exist at '{animationsPath}'");
                return;
            }

            var animFiles = Directory.GetFiles(animationsPath, "*.anim");

            if (!animFiles.Any()) {
                Console.WriteLine("Error: Could not find any .anim files in the Animations directory.");
                return;
            }

            Console.WriteLine($"Found {animFiles.Length} animation files.");

            var ledAnimationDtoTasks = animFiles.Select(LoadAnimationAsync);
            var ledAnimationDtos = (await Task.WhenAll(ledAnimationDtoTasks))
                .Where(x => x?.AnimationTexturePath != null);

            Console.WriteLine($"Loaded {ledAnimationDtos.Count()} valid animations.");
        }

        private static async Task<LedAnimationDto?> LoadAnimationAsync(string path) {
            var json = await File.ReadAllTextAsync(path, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json)) {
                Console.WriteLine($"Error: .anim file '{path}' is empty! Skipping.");
                return null;
            }
            
            var options = new JsonSerializerOptions {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            LedAnimationDto ledAnimationDto;

            try {
                ledAnimationDto = JsonSerializer.Deserialize<LedAnimationDto>(json, options);
            }
            catch (JsonException e) {
                Console.WriteLine($"Error: Could not parse .anim file '{path}': {e.Message}");
                return null;
            }

            return ledAnimationDto;
        }
    }
}
