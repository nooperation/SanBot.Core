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
        private NetworkReader _networkReader;

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

            _networkReader = new NetworkReader(accountConductor, accountConductorLock);
            _networkReader.Start();
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

        public bool Poll()
        {
            var packets = _networkReader.GetAvailablePackets();
            if(packets == null)
            {
                return false;
            }

            foreach (var packet in packets)
            {
                if (packet.MessageId == 0)
                {
                    HandleVersionPacket((VersionPacket)packet);
                    continue;
                }

                foreach (var item in MessageHandlers)
                {
                    if(item.OnMessage(packet))
                    {
                        break;
                    }
                }
            }
            _networkWriter.SendQueuedPackets();

            return true;
        }

        public void SendPacket(IPacket packet)
        {
            _networkWriter.SendPacket(packet);
        }

        public void EnqueuePacket(IPacket packet)
        {
            _networkWriter.EnqueuePacket(packet);
        }

        private void HandleVersionPacket(VersionPacket packet)
        {
            Output("Got version packet from server, sending login packet");
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
