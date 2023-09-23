using System.Net.Sockets;
using SanProtocol;

namespace SanBot.Core
{
    public class VoiceClient
    {
        public event EventHandler<string>? OnOutput;
        public Action<IPacket>? OnPacket;

        public string? Hostname { get; set; }
        public int Port { get; set; }
        public uint Secret { get; set; }
        public SanUUID InstanceId { get; set; } = SanUUID.Zero;
        public bool GotVersionPacket { get; set; }

        public Driver Driver { get; }

        private readonly NetworkWriter _networkWriter;
        private readonly NetworkReader _networkReader;

        private readonly object _accountConductorLock = new();
        private readonly TcpClient _accountConductor = new();

        public uint CurrentSequence { get; set; }

        public VoiceClient(Driver driver)
        {
            Driver = driver;

            _networkWriter = new NetworkWriter(_accountConductor, _accountConductorLock, nameof(VoiceClient));
            _networkWriter.Start();

            _networkReader = new NetworkReader(_accountConductor, _accountConductorLock, nameof(VoiceClient));
            _networkReader.Start();
        }

        public void Start(string hostname, int port, uint secret, SanUUID instanceId)
        {
            Hostname = hostname;
            Port = port;
            Secret = secret;
            InstanceId = instanceId;

            Output("Connecting...");
            _accountConductor.Connect(Hostname, Port);
            Output("OK");

            Output("Sending version packet...");
            var versionPacket = new VersionPacket(VersionPacket.VersionType.ClientVoiceChannel);
            SendPacket(versionPacket);
            Output("OK");
        }

        public void Disconnect()
        {
            if (_accountConductor.Connected)
            {
                _accountConductor.Close();
            }
        }

        public bool Poll()
        {
            var packets = _networkReader.GetAvailablePackets();
            if (packets == null)
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

                OnPacket?.Invoke(packet);
            }
            _networkWriter.SendQueuedPackets();

            return true;
        }

        public void SendPacket(IPacket packet)
        {
            SendRaw(packet.GetBytes());
        }

        public void SendRaw(byte[] bytes)
        {
            _networkWriter.SendRaw(bytes, 0, bytes.Length);
        }

        private void HandleVersionPacket(VersionPacket packet)
        {
            Output("Got version packet from server, sending login packet");
            var loginPacket = new SanProtocol.ClientVoice.Login(
                InstanceId,
                Secret,
                SanUUID.Zero,
                0
            );
            SendPacket(loginPacket);
            GotVersionPacket = true;
        }

        private void Output(string message)
        {
            OnOutput?.Invoke(this, message);
        }
    }
}
