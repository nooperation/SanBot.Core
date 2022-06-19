using SanProtocol;
using SanProtocol.WorldState;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class WorldState : IMessageHandler
    {
        public event EventHandler<CreateWorld>? OnCreateWorld;
        public event EventHandler<DestroyWorld>? OnDestroyWorld;
        public event EventHandler<RigidBodyComponentInitialState>? OnRigidBodyComponentInitialState;
        public event EventHandler<AnimationComponentInitialState>? OnAnimationComponentInitialState;
        public event EventHandler<LoadClusterDefinition>? OnLoadClusterDefinition;
        public event EventHandler<ComponentRelativeTransform>? OnComponentRelativeTransform;
        public event EventHandler<InitiateCluster>? OnInitiateCluster;
        public event EventHandler<CreateClusterViaDefinition>? OnCreateClusterViaDefinition;
        public event EventHandler<DestroyCluster>? OnDestroyCluster;
        public event EventHandler<DestroyObject>? OnDestroyObject;
        public event EventHandler<DestroySourceIdSpace>? OnDestroySourceIdSpace;
        public event EventHandler<CreateCharacterNode>? OnCreateCharacterNode;
        public event EventHandler<CreateAgentController>? OnCreateAgentController;
        public event EventHandler<DestroyAgentController>? OnDestroyAgentController;

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.WorldState.CreateWorld:
                {
                    this.HandleCreateWorld(reader);
                    break;
                }
                case Messages.WorldState.DestroyWorld:
                {
                    this.HandleDestroyWorld(reader);
                    break;
                }
                case Messages.WorldState.RigidBodyComponentInitialState:
                {
                    this.HandleRigidBodyComponentInitialState(reader);
                    break;
                }
                case Messages.WorldState.AnimationComponentInitialState:
                {
                    this.HandleAnimationComponentInitialState(reader);
                    break;
                }
                case Messages.WorldState.LoadClusterDefinition:
                {
                    this.HandleLoadClusterDefinition(reader);
                    break;
                }
                case Messages.WorldState.ComponentRelativeTransform:
                {
                    this.HandleComponentRelativeTransform(reader);
                    break;
                }
                case Messages.WorldState.InitiateCluster:
                {
                    this.HandleInitiateCluster(reader);
                    break;
                }
                case Messages.WorldState.CreateClusterViaDefinition:
                {
                    this.HandleCreateClusterViaDefinition(reader);
                    break;
                }
                case Messages.WorldState.DestroyCluster:
                {
                    this.HandleDestroyCluster(reader);
                    break;
                }
                case Messages.WorldState.DestroyObject:
                {
                    this.HandleDestroyObject(reader);
                    break;
                }
                case Messages.WorldState.DestroySourceIdSpace:
                {
                    this.HandleDestroySourceIdSpace(reader);
                    break;
                }
                case Messages.WorldState.CreateCharacterNode:
                {
                    this.HandleCreateCharacterNode(reader);
                    break;
                }
                case Messages.WorldState.CreateAgentController:
                {
                    this.HandleCreateAgentController(reader);
                    break;
                }
                case Messages.WorldState.DestroyAgentController:
                {
                    this.HandleDestroyAgentController(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleCreateWorld(BinaryReader reader)
        {
            var packet = new CreateWorld(reader);
            OnCreateWorld?.Invoke(this, packet);

            /*
            // We will appear in world with this packet. Call it multiple times to appear multiple times : (
            var newPacket = new SanProtocol.ClientRegion.ClientDynamicReady(
                new List<float>() { 0, 0, 0 },
                new List<float>() { 0, 0, 0, 0 },
                new SanUUID(Client.Instance.PersonaDetails.Id),
                "",
                0,
                1
            );
            Client.Instance.RegionClient.SendPacket(newPacket);

            // Subscribe to static stuff. We have to call this or our avatar will sit in this instance forever~
            var newPacket = new SanProtocol.ClientRegion.ClientStaticReady(1);
            Client.Instance.RegionClient.SendPacket(newPacket);
            */

            //Client.Instance.WorldHasBeenCreated = true;
        }

        void HandleDestroyWorld(BinaryReader reader)
        {
            var packet = new DestroyWorld(reader);
            OnDestroyWorld?.Invoke(this, packet);
        }

        void HandleRigidBodyComponentInitialState(BinaryReader reader)
        {
            var packet = new RigidBodyComponentInitialState(reader);
            OnRigidBodyComponentInitialState?.Invoke(this, packet);
        }

        void HandleAnimationComponentInitialState(BinaryReader reader)
        {
            var packet = new AnimationComponentInitialState(reader);
            OnAnimationComponentInitialState?.Invoke(this, packet);
        }

        void HandleLoadClusterDefinition(BinaryReader reader)
        {
            var packet = new LoadClusterDefinition(reader);
            OnLoadClusterDefinition?.Invoke(this, packet);
        }

        void HandleComponentRelativeTransform(BinaryReader reader)
        {
            var packet = new ComponentRelativeTransform(reader);
            OnComponentRelativeTransform?.Invoke(this, packet);
        }

        void HandleInitiateCluster(BinaryReader reader)
        {
            var packet = new InitiateCluster(reader);
            OnInitiateCluster?.Invoke(this, packet);
        }

        void HandleCreateClusterViaDefinition(BinaryReader reader)
        {
            var packet = new CreateClusterViaDefinition(reader);
            OnCreateClusterViaDefinition?.Invoke(this, packet);
        }

        void HandleDestroyCluster(BinaryReader reader)
        {
            var packet = new DestroyCluster(reader);
            OnDestroyCluster?.Invoke(this, packet);
        }

        void HandleDestroyObject(BinaryReader reader)
        {
            var packet = new DestroyObject(reader);
            OnDestroyObject?.Invoke(this, packet);
        }

        void HandleDestroySourceIdSpace(BinaryReader reader)
        {
            var packet = new DestroySourceIdSpace(reader);
            OnDestroySourceIdSpace?.Invoke(this, packet);
        }

        void HandleCreateCharacterNode(BinaryReader reader)
        {
            var packet = new CreateCharacterNode(reader);
            OnCreateCharacterNode?.Invoke(this, packet);
        }

        void HandleCreateAgentController(BinaryReader reader)
        {
            var packet = new CreateAgentController(reader);
            OnCreateAgentController?.Invoke(this, packet);
        }

        void HandleDestroyAgentController(BinaryReader reader)
        {
            var packet = new DestroyAgentController(reader);
            OnDestroyAgentController?.Invoke(this, packet);
        }
    }
}
