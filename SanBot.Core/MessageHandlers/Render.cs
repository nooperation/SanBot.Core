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

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.Render.LightStateChanged:
                {
                    this.HandleLightStateChanged(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleLightStateChanged(BinaryReader reader)
        {
            var packet = new LightStateChanged(reader);
            OnLightStateChanged?.Invoke(this, packet);
        }
    }
}
