using SanProtocol;
using SanProtocol.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class Simulation : IMessageHandler
    {
        public event EventHandler<InitialTimestamp>? OnInitialTimestamp;
        public event EventHandler<Timestamp>? OnTimestamp;
        public event EventHandler<SetWorldGravityMagnitude>? OnSetWorldGravityMagnitude;
        public event EventHandler<ActiveRigidBodyUpdate>? OnActiveRigidBodyUpdate;
        public event EventHandler<RigidBodyDeactivated>? OnRigidBodyDeactivated;
        public event EventHandler<RigidBodyPropertyChanged>? OnRigidBodyPropertyChanged;
        public event EventHandler<RigidBodyDestroyed>? OnRigidBodyDestroyed;

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.Simulation.InitialTimestamp:
                {
                    OnInitialTimestamp?.Invoke(this, (InitialTimestamp)packet);
                    break;
                }
                case Messages.Simulation.Timestamp:
                {
                    OnTimestamp?.Invoke(this, (Timestamp)packet);
                    break;
                }
                case Messages.Simulation.SetWorldGravityMagnitude:
                {
                    OnSetWorldGravityMagnitude?.Invoke(this, (SetWorldGravityMagnitude)packet);
                    break;
                }
                case Messages.Simulation.ActiveRigidBodyUpdate:
                {
                    OnActiveRigidBodyUpdate?.Invoke(this, (ActiveRigidBodyUpdate)packet);
                    break;
                }
                case Messages.Simulation.RigidBodyDeactivated:
                {
                    OnRigidBodyDeactivated?.Invoke(this, (RigidBodyDeactivated)packet);
                    break;
                }
                case Messages.Simulation.RigidBodyPropertyChanged:
                {
                    OnRigidBodyPropertyChanged?.Invoke(this, (RigidBodyPropertyChanged)packet);
                    break;
                }
                case Messages.Simulation.RigidBodyDestroyed:
                {
                    OnRigidBodyDestroyed?.Invoke(this, (RigidBodyDestroyed)packet);
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
