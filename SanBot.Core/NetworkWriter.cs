using SanProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static SanWebApi.Json.MarketplaceApi_ProductsResponse;

namespace SanBot.Core
{
    internal class NetworkWriter
    {
        private volatile List<IPacket> _packetQueueFromOtherThread = new List<IPacket>();

        private object _accountConductorLock;
        private TcpClient _accountConductor;

        private object _conditionVariable = new object();
        private bool _dataIsAvailable = false;
        private volatile bool _isRunning = false;

        Thread? writerThread;

        public NetworkWriter(TcpClient accountConductor, object accountConductorLock)
        {
            _accountConductor = accountConductor;
            _accountConductorLock = accountConductorLock;
        }

        public void Start()
        {
            Console.WriteLine("NetworkWriter::Start()");

            if (_isRunning)
            {
                throw new Exception("It's already running");
            }

            _isRunning = true;
            writerThread = new Thread(() =>
            {
                while (_isRunning)
                {
                    Poll();

                    lock (_conditionVariable)
                    {
                        if (!_dataIsAvailable)
                        {
                            Monitor.Wait(_conditionVariable);
                        }
                    }
                }
            });
            writerThread.Start();
        }

        public void Stop()
        {
            Console.WriteLine("NetworkWriter::Stop()");

            if (!_isRunning)
            {
                throw new Exception("It's not running");
            }

            _isRunning = false;
            lock (_conditionVariable)
            {
                Monitor.Pulse(_conditionVariable);
            }

            if(writerThread != null)
            {
                writerThread.Join();
            }
        }

        public void SendPacket(IPacket packet)
        {
            lock (_conditionVariable)
            {
                _packetQueueFromOtherThread.Add(packet);

                if(!_dataIsAvailable)
                {
                    _dataIsAvailable = true;
                    Monitor.Pulse(_conditionVariable);
                }
            }
        }

        public void Poll()
        {
            List<IPacket> packetsToSend = new List<IPacket>();

            lock (_conditionVariable)
            {
                packetsToSend = Interlocked.Exchange(ref _packetQueueFromOtherThread, packetsToSend);
                _dataIsAvailable = false;
            }

            if (packetsToSend.Count == 0)
            {
                return;
            }

            if (packetsToSend.Count > 0)
            {
                using (var ms = new MemoryStream())
                {
                    using (var bw2 = new BinaryWriter(ms))
                    {
                        foreach (var item in packetsToSend)
                        {
                            var packetBytes = item.GetBytes();
                            bw2.Write(packetBytes.Length);
                            bw2.Write(packetBytes);
                        }

                        SendRaw(ms.ToArray());
                    }
                }
            }
        }

        public void SendRaw(byte[] toSend)
        {
            lock (_accountConductorLock)
            {
                var outStream = _accountConductor.GetStream();
                var bytesRemaining = toSend.Length;
                var bytesOffset = 0;
                while (bytesRemaining > 0)
                {
                    var bytesToSend = bytesRemaining > 4096 ? 4096 : bytesRemaining;

                    outStream.Write(toSend, bytesOffset, bytesToSend);

                    bytesRemaining -= bytesToSend;
                    bytesOffset += bytesToSend;
                }
            }
        }
    }
}
