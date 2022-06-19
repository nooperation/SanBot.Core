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

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.Simulation.InitialTimestamp:
                {
                    this.HandleInitialTimestamp(reader);
                    break;
                }
                case Messages.Simulation.Timestamp:
                {
                    this.HandleTimestamp(reader);
                    break;
                }
                case Messages.Simulation.SetWorldGravityMagnitude:
                {
                    this.HandleSetWorldGravityMagnitude(reader);
                    break;
                }
                case Messages.Simulation.ActiveRigidBodyUpdate:
                {
                    this.HandleActiveRigidBodyUpdate(reader);
                    break;
                }
                case Messages.Simulation.RigidBodyDeactivated:
                {
                    this.HandleRigidBodyDeactivated(reader);
                    break;
                }
                case Messages.Simulation.RigidBodyPropertyChanged:
                {
                    this.HandleRigidBodyPropertyChanged(reader);
                    break;
                }
                case Messages.Simulation.RigidBodyDestroyed:
                {
                    this.HandleRigidBodyDestroyed(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleInitialTimestamp(BinaryReader reader)
        {
            var packet = new InitialTimestamp(reader);
            OnInitialTimestamp?.Invoke(this, packet);
        }

        void HandleTimestamp(BinaryReader reader)
        {
            var packet = new Timestamp(reader);
            OnTimestamp?.Invoke(this, packet);
        }

        void HandleSetWorldGravityMagnitude(BinaryReader reader)
        {
            var packet = new SetWorldGravityMagnitude(reader);
            OnSetWorldGravityMagnitude?.Invoke(this, packet);
        }

        void HandleActiveRigidBodyUpdate(BinaryReader reader)
        {
            var packet = new ActiveRigidBodyUpdate(reader);
            OnActiveRigidBodyUpdate?.Invoke(this, packet);
        }

        void HandleRigidBodyDeactivated(BinaryReader reader)
        {
            var packet = new RigidBodyDeactivated(reader);
            OnRigidBodyDeactivated?.Invoke(this, packet);
        }

        void HandleRigidBodyPropertyChanged(BinaryReader reader)
        {
            var packet = new RigidBodyPropertyChanged(reader);
            OnRigidBodyPropertyChanged?.Invoke(this, packet);
        }

        void HandleRigidBodyDestroyed(BinaryReader reader)
        {
            var packet = new RigidBodyDestroyed(reader);
            OnRigidBodyDestroyed?.Invoke(this, packet);
        }

    }
}
