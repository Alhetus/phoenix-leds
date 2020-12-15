using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PhoenixLeds.DTO;

namespace PhoenixLeds
{
    public class AnimationEventModel
    {
        public static Dictionary<string, AnimationEvent> AnimationEvents { get; set; } = new Dictionary<string, AnimationEvent>();

        public static async Task LoadAsync() {
            var animationEventsFilePath = Path.Combine(".", "animationEvents.json");

            if (!File.Exists(animationEventsFilePath)) {
                Console.WriteLine("Error: Could not find animationEvents.json!");
                return;
            }

            var json = await File.ReadAllTextAsync(animationEventsFilePath, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json)) {
                Console.WriteLine("Error: animationEvents.json is empty!");
                return;
            }

            AnimationEventsDto animationEventsDto;

            try {
                animationEventsDto = JsonSerializer.Deserialize<AnimationEventsDto>(json);
            }
            catch (JsonException e) {
                Console.WriteLine($"Error: could not parse animationEvents.json: {e.Message}");
                return;
            }

            AnimationEvents = animationEventsDto.AnimationEvents.Select(ae => new AnimationEvent(ae))
                .ToDictionary(x => x.EventName, x => x);

            Console.WriteLine($"Loaded {AnimationEvents.Count} animation events.");
        }
    }
}