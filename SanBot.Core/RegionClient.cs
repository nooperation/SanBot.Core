using SanBot.Core.MessageHandlers;
using SanProtocol;
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
    public class RegionClient
    {
        private object accountConductorLock = new object();
        TcpClient accountConductor = new TcpClient();
        public event EventHandler<string>? OnOutput;

        public string? Hostname { get; set; }
        public int Port { get; set; }
        public uint Secret { get; set; }

        private List<IMessageHandler> MessageHandlers { get; }
        public PacketBuffer PacketBuffer { get; set; } = new PacketBuffer();
        public Driver Driver { get; }

        public AgentController AgentControllerMessages { get; set; }
        public AnimationComponent AnimationComponentMessages { get; set; }
        public Audio AudioMessages { get; set; }
        public ClientRegion ClientRegionMessages { get; set; }
        public EditServer EditServerMessages { get; set; }
        public GameWorld GameWorldMessages { get; set; }
        public RegionRegion RegionRegionMessages { get; set; }
        public Render RenderMessages { get; set; }
        public Simulation SimulationMessages { get; set; }
        public WorldState WorldStateMessages { get; set; }

        private NetworkWriter _networkWriter;

        public RegionClient(Driver driver)
        {
            this.Driver = driver;

            this.AgentControllerMessages = new MessageHandlers.AgentController();
            this.AnimationComponentMessages = new MessageHandlers.AnimationComponent();
            this.AudioMessages = new MessageHandlers.Audio();
            this.ClientRegionMessages = new MessageHandlers.ClientRegion();
            this.EditServerMessages = new MessageHandlers.EditServer();
            this.GameWorldMessages = new MessageHandlers.GameWorld();
            this.RegionRegionMessages = new MessageHandlers.RegionRegion();
            this.RenderMessages = new MessageHandlers.Render();
            this.SimulationMessages = new MessageHandlers.Simulation();
            this.WorldStateMessages = new MessageHandlers.WorldState();

            this.MessageHandlers = new List<IMessageHandler>()
            {
                AgentControllerMessages,
                AnimationComponentMessages,
                AudioMessages,
                ClientRegionMessages,
                EditServerMessages,
                GameWorldMessages,
                RegionRegionMessages,
                RenderMessages,
                SimulationMessages,
                WorldStateMessages,
            };

            _networkWriter = new NetworkWriter(accountConductor, accountConductorLock);
            _networkWriter.Start();
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
            var versionPacket = new VersionPacket(VersionPacket.VersionType.ClientRegionChannel);
            SendPacket(versionPacket);
            Output("OK");
        }

        public void Disconnect()
        {
            if (accountConductor.Connected)
            {
                accountConductor.Close();
            }
        }

        byte[] PollBuffer = new byte[16384];
        public bool Poll()
        {
            if (accountConductor.Available == 0)
            {
                return false;
            }

            var instream = accountConductor.GetStream();

            while (accountConductor.Available > 0)
            {
                var numBytesRead = instream.Read(PollBuffer, 0, PollBuffer.Length);
                PacketBuffer.AppendBytes(PollBuffer, numBytesRead);
            }

            foreach (var packet in PacketBuffer.Packets)
            {
                HandlePacket(packet);
            }
            PacketBuffer.Packets.Clear();

            return true;
        }

        public void SendPacket(IPacket packet)
        {
            _networkWriter.SendPacket(packet);
        }

        private void HandlePacket(byte[] packet)
        {
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
            var loginPacket = new SanProtocol.ClientRegion.UserLogin(
                Secret
            );
            SendPacket(loginPacket);
        }

        private void Output(string message)
        {
            OnOutput?.Invoke(this, message);
        }
    }
}
