using SanProtocol;

namespace SanBot.Core
{
    public class PacketBuffer
    {
        private byte[] _recvBuffer = new byte[0];
        public List<byte[]> Packets { get; set; } = new List<byte[]>();

        public Action<List<IPacket>> OnProcessPackets { get; internal set; } = delegate { };
        public Func<byte[], IPacket> DecodePacket { get; set; } = (bytes) => throw new NotImplementedException();

        public void AppendBytes(byte[] bytes)
        {
            AppendBytes(bytes, bytes.Length);
        }

        public void AppendBytes(byte[] bytes, int numBytes)
        {
            if (numBytes > bytes.Length)
            {
                throw new Exception("numBytes > bytes.length");
            }

            var newRecvBuffer = new byte[_recvBuffer.Length + numBytes];
            _recvBuffer.CopyTo(newRecvBuffer, 0);
            Array.Copy(bytes, 0, newRecvBuffer, _recvBuffer.Length, numBytes);
            _recvBuffer = newRecvBuffer;

            using (MemoryStream ms = new MemoryStream(_recvBuffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    var totalBytesConsumed = 0;

                    while (br.BaseStream.Length - br.BaseStream.Position >= 4)
                    {
                        var packetLength = br.ReadInt32();
                        var remainingBytes = br.BaseStream.Length - br.BaseStream.Position;
                        if (packetLength > remainingBytes)
                        {
                            break;
                        }

                        var packetBytes = br.ReadBytes(packetLength);
                        Packets.Add(packetBytes);
                        totalBytesConsumed += packetLength + 4;

                        //var signature = BitConverter.ToUInt32(packetBytes);
                    }

                    if (totalBytesConsumed > 0)
                    {
                        newRecvBuffer = new byte[_recvBuffer.Length - totalBytesConsumed];
                        Array.Copy(_recvBuffer, totalBytesConsumed, newRecvBuffer, 0, _recvBuffer.Length - totalBytesConsumed);
                        _recvBuffer = newRecvBuffer;
                    }
                }
            }
        }

        public void ProcessPacketQueue()
        {
            var decodedPackets = Packets
                .Select(n => DecodePacket(n)).ToList();

            OnProcessPackets(decodedPackets);
            Packets.Clear();
        }
    }
}
