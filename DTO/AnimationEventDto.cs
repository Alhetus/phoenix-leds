using System;

namespace PhoenixLeds.DTO
{
    public class AnimationEventDto
    {
        public int PadIndex { get; }
        public int ColumnIndex { get; }
        public string EventName { get; } = "";

        public AnimationEventDto(string message) {
            var parts = message.Split(';');

            if (parts.Length != 3) {
                Console.WriteLine($"Got a packet message in wrong format, must contain exactly two ';'. Message: '{message}'");
                return;
            }

            // Parse pad index
            var padIndexSuccess = int.TryParse(parts[0], out var padIndex);

            if (padIndexSuccess)
                PadIndex = padIndex;
            else
                Console.WriteLine($"Could not parse padIndex from message part '{parts[0]}'");

            // Parse column index
            var columnIndexSuccess = int.TryParse(parts[1], out var columnIndex);

            if (columnIndexSuccess)
                ColumnIndex = columnIndex;
            else
                Console.WriteLine($"Could not parse columnIndex from message part '{parts[1]}'");

            EventName = parts[2].Trim();
        }
    }
}
