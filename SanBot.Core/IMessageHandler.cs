using SanProtocol;

namespace SanBot.Core
{
    internal interface IMessageHandler
    {
        public bool OnMessage(uint messageId, BinaryReader reader);
        public bool OnMessage(IPacket packet);
    }
}
