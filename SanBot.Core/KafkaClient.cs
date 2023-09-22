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
        public event EventHandler<string>? OnOutput;
        public Action<IPacket>? OnPacket;

        public string? Hostname { get; set; }
        public int Port { get; set; }
        public uint Secret { get; set; }

        public Driver Driver { get; }

        private readonly NetworkWriter _networkWriter;
        private readonly NetworkReader _networkReader;
        private readonly object _accountConductorLock = new object();
        private readonly TcpClient _accountConductor = new TcpClient();

        public KafkaClient(Driver driver)
        {
            this.Driver = driver;

            _networkWriter = new NetworkWriter(_accountConductor, _accountConductorLock);
            _networkWriter.Start();

            _networkReader = new NetworkReader(_accountConductor, _accountConductorLock);
            _networkReader.Start();
        }

        public void Start(string hostname, int port, uint secret)
        {
            Hostname = hostname;
            Port = port;
            Secret = secret;

            Output("Connecting...");
            _accountConductor.Connect(Hostname, Port);
            Output("OK");

            Output("Sending version packet...");
            var versionPacket = new VersionPacket(VersionPacket.VersionType.ClientKafkaChannel);
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
            _networkWriter.SendPacket(packet);
        }

        public void SendRaw(byte[] bytes)
        {
            _networkWriter.SendRaw(bytes, 0, bytes.LongLength);
        }

        private void HandleVersionPacket(VersionPacket packet)
        {
            Output("Got version packet from server");

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
