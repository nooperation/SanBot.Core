﻿using SanBot.Core.MessageHandlers;
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

        public RegionClient(Driver driver)
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
            var versionPacket = new VersionPacket(VersionPacket.VersionType.ClientRegionChannel);
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

                OnPacket(packet);
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
