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

        public void EnqueuePacket(IPacket packet)
        {
            lock (_conditionVariable)
            {
                _packetQueueFromOtherThread.Add(packet);

                if (!_dataIsAvailable)
                {
                    _dataIsAvailable = true;
                }
            }
        }
        public void SendQueuedPackets()
        {
            lock (_conditionVariable)
            {
                Monitor.Pulse(_conditionVariable);
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

        byte[] _pollBytes = new byte[65535];
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

            long pollBytesOffset = 0;

            if (packetsToSend.Count > 0)
            {
                foreach (var item in packetsToSend)
                {
                    var packetBytes = item.GetBytes();

                    if (_pollBytes.Length < pollBytesOffset + packetBytes.Length)
                    {
                        Console.WriteLine($"*** Expanding _pollBytes by {65535 + packetBytes.Length * 2} ***");
                        var newPollbytes = new byte[_pollBytes.Length + 65535 + packetBytes.Length * 2];
                        Array.Copy(_pollBytes, newPollbytes, _pollBytes.Length);
                        _pollBytes = newPollbytes;
                    }

                    _pollBytes[pollBytesOffset++] = (byte)((packetBytes.Length & 0x000000ff) >> 0);
                    _pollBytes[pollBytesOffset++] = (byte)((packetBytes.Length & 0x0000ff00) >> 8);
                    _pollBytes[pollBytesOffset++] = (byte)((packetBytes.Length & 0x00ff0000) >> 16);
                    _pollBytes[pollBytesOffset++] = (byte)((packetBytes.Length & 0xff000000) >> 24);
                    packetBytes.CopyTo(_pollBytes, pollBytesOffset);
                    pollBytesOffset += packetBytes.Length;
                }

                SendRaw(_pollBytes, 0, pollBytesOffset);
            }
        }

        public void SendRaw(byte[] toSend, long toSendOffset, long toSendLength)
        {
            lock (_accountConductorLock)
            {
                var outStream = _accountConductor.GetStream();
                var bytesRemaining = toSendLength;
                var bytesOffset = 0;
                while (bytesRemaining > 0)
                {
                    var bytesToSend = bytesRemaining > 4096 ? 4096 : (int)bytesRemaining;

                    outStream.Write(toSend, bytesOffset, bytesToSend);

                    bytesRemaining -= bytesToSend;
                    bytesOffset += bytesToSend;
                }
            }
        }
    }
}
