using System;

namespace PhoenixLeds.DTO
{
    public class AnimationEventDto
    {
        public int PadIndex { get; }
        public string EventName { get; } = "";

        public AnimationEventDto(string message) {
            var parts = message.Split(';');

            if (parts.Length != 2) {
                Console.WriteLine($"Got a packet message in wrong format, must contain exactly one ';'. Message: '{message}'");
                return;
            }

            var padIndexSuccess = int.TryParse(parts[0], out var padIndex);

            if (padIndexSuccess)
                PadIndex = padIndex;
            else
                Console.WriteLine($"Could not parse padIndex from message part '{parts[0]}'");

            EventName = parts[1];
        }
    }
}
