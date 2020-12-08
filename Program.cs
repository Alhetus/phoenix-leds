using System;
using System.Threading.Tasks;

namespace PhoenixLeds
{
    class Program
    {
        private static async Task Main(string[] args) {
            // Load settings
            await GlobalSettings.LoadAsync();

            // Load animations from .anim files
            await LedAnimationLoader.LoadAnimationsAsync();

            Console.WriteLine("Hello World!");
        }
    }
}
