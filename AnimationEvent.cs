using System;
using System.Collections.Generic;
using System.Linq;
using PhoenixLeds.DTO;

namespace PhoenixLeds
{
    public class AnimationEvent
    {
        public string EventName { get; }
        public List<Panel> Panels { get; }
        public string AnimationToPlay { get; }

        public AnimationEvent(EventDto dto) {
            EventName = dto.EventName ?? "Unknown";
            AnimationToPlay = dto.AnimationToPlay ?? "";
            Panels = dto.Panels.Select(p => {
                var isSuccess = Enum.TryParse(p, true, out Panel panel);
                return isSuccess ? Panel.Down : panel;
            }).ToList();
        }
    }
}
