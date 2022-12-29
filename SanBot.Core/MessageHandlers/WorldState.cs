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

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.WorldState.CreateWorld:
                {
                    OnCreateWorld?.Invoke(this, (CreateWorld)packet);
                    break;
                }
                case Messages.WorldState.DestroyWorld:
                {
                    OnDestroyWorld?.Invoke(this, (DestroyWorld)packet);
                    break;
                }
                case Messages.WorldState.RigidBodyComponentInitialState:
                {
                    OnRigidBodyComponentInitialState?.Invoke(this, (RigidBodyComponentInitialState)packet);
                    break;
                }
                case Messages.WorldState.AnimationComponentInitialState:
                {
                    OnAnimationComponentInitialState?.Invoke(this, (AnimationComponentInitialState)packet);
                    break;
                }
                case Messages.WorldState.LoadClusterDefinition:
                {
                    OnLoadClusterDefinition?.Invoke(this, (LoadClusterDefinition)packet);
                    break;
                }
                case Messages.WorldState.ComponentRelativeTransform:
                {
                    OnComponentRelativeTransform?.Invoke(this, (ComponentRelativeTransform)packet);
                    break;
                }
                case Messages.WorldState.InitiateCluster:
                {
                    OnInitiateCluster?.Invoke(this, (InitiateCluster)packet);
                    break;
                }
                case Messages.WorldState.CreateClusterViaDefinition:
                {
                    OnCreateClusterViaDefinition?.Invoke(this, (CreateClusterViaDefinition)packet);
                    break;
                }
                case Messages.WorldState.DestroyCluster:
                {
                    OnDestroyCluster?.Invoke(this, (DestroyCluster)packet);
                    break;
                }
                case Messages.WorldState.DestroyObject:
                {
                    OnDestroyObject?.Invoke(this, (DestroyObject)packet);
                    break;
                }
                case Messages.WorldState.DestroySourceIdSpace:
                {
                    OnDestroySourceIdSpace?.Invoke(this, (DestroySourceIdSpace)packet);
                    break;
                }
                case Messages.WorldState.CreateCharacterNode:
                {
                    OnCreateCharacterNode?.Invoke(this, (CreateCharacterNode)packet);
                    break;
                }
                case Messages.WorldState.CreateAgentController:
                {
                    OnCreateAgentController?.Invoke(this, (CreateAgentController)packet);
                    break;
                }
                case Messages.WorldState.DestroyAgentController:
                {
                    OnDestroyAgentController?.Invoke(this, (DestroyAgentController)packet);
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
