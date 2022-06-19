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

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.RegionRegion.DynamicSubscribe:
                {
                    this.HandleDynamicSubscribe(reader);
                    break;
                }
                case Messages.RegionRegion.DynamicPlayback:
                {
                    this.HandleDynamicPlayback(reader);
                    break;
                }
                case Messages.RegionRegion.MasterFrameSync:
                {
                    this.HandleMasterFrameSync(reader);
                    break;
                }
                case Messages.RegionRegion.AgentControllerMapping:
                {
                    this.HandleAgentControllerMapping(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleDynamicSubscribe(BinaryReader reader)
        {
            var packet = new DynamicSubscribe(reader);
            OnDynamicSubscribe?.Invoke(this, packet);
        }

        void HandleDynamicPlayback(BinaryReader reader)
        {
            var packet = new DynamicPlayback(reader);
            OnDynamicPlayback?.Invoke(this, packet);
        }

        void HandleMasterFrameSync(BinaryReader reader)
        {
            var packet = new MasterFrameSync(reader);
            OnMasterFrameSync?.Invoke(this, packet);
        }

        void HandleAgentControllerMapping(BinaryReader reader)
        {
            var packet = new AgentControllerMapping(reader);
            OnAgentControllerMapping?.Invoke(this, packet);
        }

    }
}
