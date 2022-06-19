using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core
{
    public class PacketBuffer
    {
        public byte[] RecvBuffer = new byte[0];
        public List<byte[]> Packets { get; set; } = new List<byte[]>();

        public void AppendBytes(byte[] bytes)
        {
            AppendBytes(bytes, bytes.Length);
        }

        public void AppendBytes(byte[] bytes, int numBytes)
        {
            if(numBytes > bytes.Length)
            {
                throw new Exception("numBytes > bytes.length");
            }

            var newRecvBuffer = new byte[RecvBuffer.Length + numBytes];
            RecvBuffer.CopyTo(newRecvBuffer, 0);
            Array.Copy(bytes, 0, newRecvBuffer, RecvBuffer.Length, numBytes);
            RecvBuffer = newRecvBuffer;

            using (MemoryStream ms = new MemoryStream(RecvBuffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    var totalBytesConsumed = 0;

                    while(br.BaseStream.Length - br.BaseStream.Position >= 4)
                    {
                        var packetLength = br.ReadInt32();
                        var remainingBytes = br.BaseStream.Length - br.BaseStream.Position;
                        if(packetLength > remainingBytes)
                        {
                            break;
                        }

                        var packet = br.ReadBytes(packetLength);
                        Packets.Add(packet);
                        totalBytesConsumed += packetLength + 4;

                        var signature = BitConverter.ToUInt32(packet);
                    }

                    if(totalBytesConsumed > 0)
                    {
                        newRecvBuffer = new byte[RecvBuffer.Length - totalBytesConsumed];
                        Array.Copy(RecvBuffer, totalBytesConsumed, newRecvBuffer, 0, RecvBuffer.Length - totalBytesConsumed);
                        RecvBuffer = newRecvBuffer;
                    }
                }
            }
        }
    }
}
