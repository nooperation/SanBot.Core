using SanProtocol;
using SanProtocol.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class Render : IMessageHandler
    {
        public event EventHandler<LightStateChanged>? OnLightStateChanged;

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.Render.LightStateChanged:
                {
                    OnLightStateChanged?.Invoke(this, (LightStateChanged)packet);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
