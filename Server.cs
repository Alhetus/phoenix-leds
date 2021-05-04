using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENet;
using PhoenixLeds.DTO;

namespace PhoenixLeds
{
    public class Server
    {
        private Host? _server;
        private readonly LedAnimationController _ledAnimationController;

        public Server(LedAnimationController ledAnimationController) {
            _ledAnimationController = ledAnimationController;
        }

        public void StartServer(ushort port, int maxClients) {
            Library.Initialize();

            _server = new Host();
            var address = new Address { Port = port };
            var success = address.SetHost("localhost");

            if (!success)
                throw new Exception("Could not set host to localhost!");

            _server.Create(address, maxClients, 2);
            Console.WriteLine($"Server started on port {port}");
        }

        public void Update() {
            if (_server == null)
                return;

            if (_server.Service(1, out var netEvent) <= 0)
                return;

            switch (netEvent.Type) {
                case EventType.None:
                    break;

                case EventType.Connect:
                    Console.WriteLine("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                    break;

                case EventType.Disconnect:
                    Console.WriteLine("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                    break;

                case EventType.Timeout:
                    Console.WriteLine("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                    break;

                case EventType.Receive:
                    Console.WriteLine("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " +
                                      netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " +
                                      netEvent.Packet.Length);
                    HandlePacket(netEvent.Packet);
                    netEvent.Packet.Dispose();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _server.Flush();
        }

        private void HandlePacket(Packet packet) {
            byte[] buffer = new byte[packet.Length];
            packet.CopyTo(buffer);

            var message = Encoding.ASCII.GetString(buffer);
            var animationEventDto = new AnimationEventDto(message);

            if (animationEventDto.PadIndex > 0) {
                Console.WriteLine("TODO: support more pads!");
                return;
            }

            if (AnimationEventModel.AnimationEvents.ContainsKey(animationEventDto.EventName)) {
                var animationEvent = AnimationEventModel.AnimationEvents[animationEventDto.EventName];

                // If column that we got from animationEventDto is supported play animation on that panel
                // Otherwise 
                var panelsToPlayOn = animationEventDto.ColumnIndex >= 0 && animationEventDto.ColumnIndex <= 3
                    ? new List<Panel> { (Panel) animationEventDto.ColumnIndex }
                    : animationEvent.Panels;

                _ledAnimationController.PlayLedAnimation(animationEvent.AnimationToPlay, panelsToPlayOn.ToHashSet());
            }
            else
                Console.WriteLine($"Error: No animation event with name '{animationEventDto.EventName}' exists!");
        }
    }
}
