using SanProtocol;
using SanProtocol.RegionRegion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class RegionRegion : IMessageHandler
    {
        public event EventHandler<DynamicSubscribe>? OnDynamicSubscribe;
        public event EventHandler<DynamicPlayback>? OnDynamicPlayback;
        public event EventHandler<MasterFrameSync>? OnMasterFrameSync;
        public event EventHandler<AgentControllerMapping>? OnAgentControllerMapping;

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.RegionRegion.DynamicSubscribe:
                {
                    OnDynamicSubscribe?.Invoke(this, (DynamicSubscribe)packet);
                    break;
                }
                case Messages.RegionRegion.DynamicPlayback:
                {
                    OnDynamicPlayback?.Invoke(this, (DynamicPlayback)packet);
                    break;
                }
                case Messages.RegionRegion.MasterFrameSync:
                {
                    OnMasterFrameSync?.Invoke(this, (MasterFrameSync)packet);
                    break;
                }
                case Messages.RegionRegion.AgentControllerMapping:
                {
                    OnAgentControllerMapping?.Invoke(this, (AgentControllerMapping)packet);
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
