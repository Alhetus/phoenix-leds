using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace PhoenixLeds
{
    public static class SerialCommunicator
    {
        private static SerialPort? Serial { get; set; }

        // Message control bytes teensy -> computer
        private const byte STX = 0x02; // Start of Text
        private const byte ETX = 0x03; // End of Text

        // Message control bytes computer -> teensy
        // When textures are loaded they are slightly darkened so they don't contain any 254 (FE) or 255 (FF) values, those are reserved for communication
        private static readonly byte[] FrameStartHeader = { 0xfe }; // Send one FE byte to mark frame start
        private static readonly byte[] FrameEndHeader = { 0xff }; // Send one FF byte to mark frame end

        private static readonly object EventLock = new object();
        private static byte[] _buffer = new byte[4096];
        private static readonly List<byte> MessageBuffer = new List<byte>();
        private static bool _isReceivingMessage;

        public static void InitSerialConnection() {
            var ports = SerialPort.GetPortNames();
            Console.WriteLine($"Found {ports.Length} serial " + (ports.Length == 1 ? "port." : "ports."));

            foreach (var port in ports) {
                Console.WriteLine($"Found serial port: {port}");
            }

            if (!ports.Any())
                throw new Exception("Error: No serial ports available!");

            if (ports.All(x => x != GlobalSettings.SerialPort))
                throw new Exception($"Error: Port '{GlobalSettings.SerialPort}' specified in settings.json not found!");

            Serial = new SerialPort {
                BaudRate = GlobalSettings.BaudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                PortName = GlobalSettings.SerialPort,
                RtsEnable = true,
                Encoding = Encoding.UTF8,
                WriteBufferSize = 8192
            };

            Serial.DataReceived += DataReceivedHandler;
            Serial.ErrorReceived += ErrorReceivedHandler;

            try {
                Serial.Open();
            }
            catch (Exception e) {
                Console.WriteLine($"Error: Could not open serial port '{GlobalSettings.SerialPort}': " + e.Message);
                throw;
            }

            if (Serial.IsOpen)
                Console.WriteLine($"Serial connection to port '{GlobalSettings.SerialPort}' is open. Baud rate: {Serial.BaudRate}");
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
            lock (EventLock) {
                switch (e.EventType) {
                    case SerialData.Chars:
                        var port = (SerialPort)sender;
                        var bytesToRead = port.BytesToRead;

                        if (bytesToRead > _buffer.Length)
                            Array.Resize(ref _buffer, bytesToRead);

                        var bytesRead = port.Read(_buffer, 0, bytesToRead);

                        ProcessBuffer(_buffer, bytesRead);
                        break;
                    case SerialData.Eof:
                        Console.WriteLine("Received Eof from serial port, closing connection.");
                        var serialPort = (SerialPort) sender;
                        serialPort.Close();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e) {
            lock (EventLock) {
                switch (e.EventType) {
                    case SerialError.RXOver:
                        Console.WriteLine("Serial error: An input buffer overflow has occurred. There is either no room in the input buffer, or a character was received after the end-of-file (EOF) character. Resetting everything.");
                        ResetSerialPort((SerialPort) sender);
                        break;
                    case SerialError.Overrun:
                        Console.WriteLine("Serial error: A character-buffer overrun has occurred. The next character is lost.. Resetting everything.");
                        ResetSerialPort((SerialPort) sender);
                        break;
                    case SerialError.RXParity:
                        Console.WriteLine("Serial error: The hardware detected a parity error. Resetting everything.");
                        ResetSerialPort((SerialPort) sender);
                        break;
                    case SerialError.Frame:
                        Console.WriteLine("Serial error: The hardware detected a framing error. Resetting everything.");
                        ResetSerialPort((SerialPort) sender);
                        break;
                    case SerialError.TXFull:
                        throw new Exception("Serial error: Output buffer is full. Can't handle this!");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void ResetSerialPort(SerialPort serialPort) {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        private static void ProcessBuffer(IReadOnlyList<byte> buffer, int length) {
            for (var i = 0; i < length; i++) {
                switch (buffer[i]) {
                    // End of text received -> get the message
                    case ETX:
                        var message = Encoding.UTF8.GetString(MessageBuffer.ToArray());
                        Console.WriteLine($"Serial message received: {message}");
                        MessageBuffer.Clear();
                        _isReceivingMessage = false;
                        break;
                    // Start of text received -> start accumulating message
                    case STX:
                        MessageBuffer.Clear();
                        _isReceivingMessage = true;
                        break;
                    default:
                        // If we are currently receiving a message -> write the bytes to the message buffer
                        if (_isReceivingMessage)
                            MessageBuffer.Add(buffer[i]);
                        break;
                }
            }
        }

        public static void WriteFullAnimationFrame(byte[] fullFrameBytes) {
            if (Serial == null || !Serial.IsOpen) {
                Console.WriteLine("Error: Can't send animation frame. Serial port is not open!");
                return;
            }

            // Write full animation frame with all panels
            Serial.Write(FrameStartHeader, 0, FrameStartHeader.Length);
            Serial.Write(fullFrameBytes, 0, fullFrameBytes.Length);
            Serial.Write(FrameEndHeader, 0, FrameEndHeader.Length);
        }
    }
}
