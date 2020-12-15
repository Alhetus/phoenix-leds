using System.Threading.Tasks;

namespace PhoenixLeds
{
    class Program
    {
        private static async Task Main(string[] args) {
            // Load settings
            await GlobalSettings.LoadAsync();

            // Load animation events
            await AnimationEventModel.LoadAsync();

            // Open serial port
            SerialCommunicator.InitSerialConnection();

            // Load animations from .anim files
            var ledAnimationController = new LedAnimationController();
            await ledAnimationController.LoadAnimationsAsync();

            // Start server
            var server = new Server(ledAnimationController);
            // TODO: Move server configuration to settings.json
            server.StartServer(6263, 20);

            // Poll for received animation events on the ENet server
            while (true) {
                server.Update();
            }
        }
    }
}
