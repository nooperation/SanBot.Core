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
    public class KafkaClient
    {
        TcpClient accountConductor = new TcpClient();
        public event EventHandler<string>? OnOutput;

        public string? Hostname { get; set; }
        public int Port { get; set; }
        public uint Secret { get; set; }

        private List<IMessageHandler> MessageHandlers { get; }
        public ClientKafka ClientKafkaMessages { get; set; }
        public PacketBuffer PacketBuffer { get; set; } = new PacketBuffer();
        public Driver Driver { get; }

        public KafkaClient(Driver driver)
        {
            this.Driver = driver;

            ClientKafkaMessages = new ClientKafka();

            this.MessageHandlers = new List<IMessageHandler>()
            {
                ClientKafkaMessages
            };
        }

        public void Start(string hostname, int port, uint secret)
        {
            Hostname = hostname;
            Port = port;
            Secret = secret;

            Output("Connecting...");
            accountConductor.Connect(Hostname, Port);
            Output("OK");

            Output("Sending version packet...");
            var versionPacket = new VersionPacket(VersionPacket.VersionType.ClientKafkaChannel);
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

            if (Driver.MyUserInfo?.AccountId == null)
            {
                throw new Exception("Cannot login: Missing UserInfo");
            }

            if (Driver.MyPersonaDetails?.Id == null)
            {
                throw new Exception("Cannot login: Missing PersonaDetails for default persona");
            }

            Output("Sending login packet...");
            var loginPacket = new SanProtocol.ClientKafka.Login(
                Driver.MyUserInfo.AccountId,
                Driver.MyPersonaDetails.Id,
                Secret,
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
