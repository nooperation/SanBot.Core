using SanBot.Core.MessageHandlers;
using SanProtocol;
using SanWebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SanBot.Core;

namespace SanBot.Core
{
    public class VoiceClient
    {
        TcpClient accountConductor = new TcpClient();
        public event EventHandler<string>? OnOutput;
        
        public string? Hostname { get; set; }
        public int Port { get; set; }
        public uint Secret { get; set; }
        public SanUUID InstanceId { get; set; } = SanUUID.Zero;
        
        private List<IMessageHandler> MessageHandlers { get; }
        public PacketBuffer PacketBuffer { get; set; } = new PacketBuffer();

        public ClientVoice ClientVoiceMessages { get; set; }
        public Driver Driver { get; }

        public VoiceClient(Driver driver)
        {
            this.Driver = driver;

            this.ClientVoiceMessages = new ClientVoice();

            this.MessageHandlers = new List<IMessageHandler>()
            {
                ClientVoiceMessages
            };
        }

        public void Start(string hostname, int port, uint secret, SanUUID instanceId)
        {
            Hostname = hostname;
            Port = port;
            Secret = secret;
            InstanceId = instanceId;
            // InstanceId = new SanUUID(sceneUri.Substring(1 + sceneUri.LastIndexOf('/')));

            Output("Connecting...");
            accountConductor.Connect(Hostname, Port);
            Output("OK");

            Output("Sending version packet...");
            var versionPacket = new VersionPacket(VersionPacket.VersionType.ClientVoiceChannel);
            SendPacket(versionPacket);
            Output("OK");
        }


        public bool Poll()
        {
            if (accountConductor.Available == 0)
            {
                return false;
            }

            var instream = accountConductor.GetStream();

            var buffer = new byte[4096];
            var numBytesRead = instream.Read(buffer, 0, buffer.Length);

            PacketBuffer.AppendBytes(buffer, numBytesRead);
            foreach (var packet in PacketBuffer.Packets)
            {
                HandlePacket(packet);
            }
            PacketBuffer.Packets.Clear();

            return true;
        }

        public void SendPacket(IPacket packet)
        {
           // Output("SendPacket: " + packet.GetType() + "\n" + packet);
            SendRaw(packet.GetBytes());
        }

        public void SendRaw(byte[] bytes)
        {
            //Output("SendRaw: " + Utils.DumpPacket(bytes, true));

            BinaryWriter bw = new BinaryWriter(accountConductor.GetStream());
            bw.Write(bytes.Length);

            var bytesRemaining = bytes.Length;
            var bytesOffset = 0;
            while (bytesRemaining > 0)
            {
                var bytesToSend = bytesRemaining > 4096 ? 4096 : bytesRemaining;

                bw.Write(bytes, bytesOffset, bytesToSend);

                bytesRemaining -= bytesToSend;
                bytesOffset += bytesToSend;
            }
        }

        private void HandlePacket(byte[] packet)
        {
           // Output("HandlePacket " + Utils.DumpPacket(packet, true));

            using (BinaryReader br = new BinaryReader(new MemoryStream(packet)))
            {
                var id = br.ReadUInt32();

                switch (id)
                {
                    case 0:
                        HandleVersionPacket(br);
                        break;
                    default:
                    {
                        bool handledMessage = false;
                        foreach (var item in MessageHandlers)
                        {
                            if (item.OnMessage(id, br))
                            {
                                handledMessage = true;
                                break;
                            }
                        }

                        if (handledMessage == false)
                        {
                            Output("Unhandled Message");
                        }
                    }
                    break;
                }
            }
        }

        private void HandleVersionPacket(BinaryReader br)
        {
            Output("Got version packet from server");
            var versionPacket = new VersionPacket(br);

            Output("Sending login packet...");
            var loginPacket = new SanProtocol.ClientVoice.Login(
                InstanceId,
                Secret,
                SanUUID.Zero,
                0
            );
            SendPacket(loginPacket);
        }

        private void Output(string message)
        {
            OnOutput?.Invoke(this, message);
        }
    }
}
