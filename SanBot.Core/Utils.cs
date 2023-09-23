using System.Runtime.Intrinsics.X86;
using System.Text;

namespace SanBot.Core
{
    public class Utils
    {
        public static ushort ShortCRC(byte[] data, long dataLength, ulong initialChecksum)
        {
            var checksum = ~(uint)initialChecksum;

            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                var num_8byte_chunks = (ulong)(dataLength / 8);
                for (ulong i = 0; i < num_8byte_chunks; ++i)
                {
                    var current = br.ReadUInt64();
                    checksum = (uint)Sse42.X64.Crc32(checksum, current);
                }

                var remainingBytes = dataLength - br.BaseStream.Position;
                if (remainingBytes >= 4)
                {
                    var current = br.ReadUInt32();
                    checksum = Sse42.Crc32(checksum, current);
                    remainingBytes -= 4;
                }

                if (remainingBytes >= 2)
                {
                    var current = br.ReadUInt16();
                    checksum = Sse42.Crc32(checksum, current);
                    remainingBytes -= 2;
                }

                if (remainingBytes > 0)
                {
                    var current = br.ReadByte();
                    checksum = Sse42.Crc32(checksum, current);
                }
            }

            checksum = ~checksum;
            return (ushort)checksum;
        }


        public static string DumpPacket(byte[] packet, bool isSending)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{(isSending ? "-->" : "<--")} [{packet.Length}]");

            foreach (var item in packet)
            {
                sb.Append($"{item:X2} ");
            }

            return sb.ToString();
        }
    }
}
