using SanProtocol;
using SanProtocol.AgentController;
using SanProtocol.AnimationComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class AgentController : IMessageHandler
    {
        public event EventHandler<AgentPlayAnimation>? OnAgentPlayAnimation;
        public event EventHandler<ExitSit>? OnExitSit;
        public event EventHandler<ObjectInteractionPromptUpdate>? OnObjectInteractionPromptUpdate;
        public event EventHandler<ObjectInteractionCreate>? OnObjectInteractionCreate;
        public event EventHandler<RequestSitOnObject>? OnRequestSitOnObject;
        public event EventHandler<SitOnObject>? OnSitOnObject;
        public event EventHandler<SetAgentFiltersBody>? OnSetAgentFiltersBody;
        public event EventHandler<RequestSetAgentFiltersBody>? OnRequestSetAgentFiltersBody;
        public event EventHandler<SetCharacterUserProperty>? OnSetCharacterUserProperty;
        // public event EventHandler<CreateSpeechGraphicsPlayer>? OnCreateSpeechGraphicsPlayer; /* REMOVED 2020-08-13 */
        public event EventHandler<RequestSpawnItem>? OnRequestSpawnItem;
        public event EventHandler<RequestDeleteLatestSpawn>? OnRequestDeleteLatestSpawn;
        public event EventHandler<RequestDeleteAllSpawns>? OnRequestDeleteAllSpawns;
        public event EventHandler<ControlPoint>? OnControlPoint;
        public event EventHandler<WarpCharacter>? OnWarpCharacter;
        public event EventHandler<RequestWarpCharacter>? OnRequestWarpCharacter;
        public event EventHandler<CharacterControlPointInput>? OnCharacterControlPointInput;
        public event EventHandler<CharacterControlPointInputReliable>? OnCharacterControlPointInputReliable;
        public event EventHandler<CharacterControllerInput>? OnCharacterControllerInput;
        public event EventHandler<CharacterControllerInputReliable>? OnCharacterControllerInputReliable;
        public event EventHandler<RequestAgentPlayAnimation>? OnRequestAgentPlayAnimation;
        public event EventHandler<RequestBehaviorStateUpdate>? OnRequestBehaviorStateUpdate;
        public event EventHandler<AttachToCharacterNode>? OnAttachToCharacterNode;
        public event EventHandler<DetachFromCharacterNode>? OnDetachFromCharacterNode;
        public event EventHandler<RequestDetachFromCharacterNode>? OnRequestDetachFromCharacterNode;
        public event EventHandler<SetCharacterNodePhysics>? OnSetCharacterNodePhysics;
        public event EventHandler<WarpCharacterNode>? OnWarpCharacterNode;
        public event EventHandler<CharacterIKBone>? OnCharacterIKBone;
        public event EventHandler<CharacterIKPose>? OnCharacterIKPose;
        public event EventHandler<CharacterIKBoneDelta>? OnCharacterIKBoneDelta;
        public event EventHandler<CharacterIKPoseDelta>? OnCharacterIKPoseDelta;
        public event EventHandler<ObjectInteraction>? OnObjectInteraction;
        public event EventHandler<ObjectInteractionUpdate>? OnObjectInteractionUpdate;
        public event EventHandler<UserReaction>? OnUserReaction; /* ADDED 2020-09-10 ? */

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.AgentController.ControlPoint:
                {
                    this.HandleControlPoint(reader);
                    break;
                }
                case Messages.AgentController.WarpCharacter:
                {
                    this.HandleWarpCharacter(reader);
                    break;
                }
                case Messages.AgentController.RequestWarpCharacter:
                {
                    this.HandleRequestWarpCharacter(reader);
                    break;
                }
                case Messages.AgentController.CharacterControlPointInput:
                {
                    this.HandleCharacterControlPointInput(reader);
                    break;
                }
                case Messages.AgentController.CharacterControlPointInputReliable:
                {
                    this.HandleCharacterControlPointInputReliable(reader);
                    break;
                }
                case Messages.AgentController.CharacterControllerInput:
                {
                    this.HandleCharacterControllerInput(reader);
                    break;
                }
                case Messages.AgentController.CharacterControllerInputReliable:
                {
                    this.HandleCharacterControllerInputReliable(reader);
                    break;
                }
                case Messages.AgentController.AgentPlayAnimation:
                {
                    this.HandleAgentPlayAnimation(reader);
                    break;
                }
                case Messages.AgentController.RequestAgentPlayAnimation:
                {
                    this.HandleRequestAgentPlayAnimation(reader);
                    break;
                }
                case Messages.AgentController.RequestBehaviorStateUpdate:
                {
                    this.HandleRequestBehaviorStateUpdate(reader);
                    break;
                }
                case Messages.AgentController.AttachToCharacterNode:
                {
                    this.HandleAttachToCharacterNode(reader);
                    break;
                }
                case Messages.AgentController.DetachFromCharacterNode:
                {
                    this.HandleDetachFromCharacterNode(reader);
                    break;
                }
                case Messages.AgentController.RequestDetachFromCharacterNode:
                {
                    this.HandleRequestDetachFromCharacterNode(reader);
                    break;
                }
                case Messages.AgentController.SetCharacterNodePhysics:
                {
                    this.HandleSetCharacterNodePhysics(reader);
                    break;
                }
                case Messages.AgentController.WarpCharacterNode:
                {
                    this.HandleWarpCharacterNode(reader);
                    break;
                }
                case Messages.AgentController.CharacterIKBone:
                {
                    this.HandleCharacterIKBone(reader);
                    break;
                }
                case Messages.AgentController.CharacterIKPose:
                {
                    this.HandleCharacterIKPose(reader);
                    break;
                }
                case Messages.AgentController.CharacterIKBoneDelta:
                {
                    this.HandleCharacterIKBoneDelta(reader);
                    break;
                }
                case Messages.AgentController.CharacterIKPoseDelta:
                {
                    this.HandleCharacterIKPoseDelta(reader);
                    break;
                }
                case Messages.AgentController.ObjectInteraction:
                {
                    this.HandleObjectInteraction(reader);
                    break;
                }
                case Messages.AgentController.ObjectInteractionUpdate:
                {
                    this.HandleObjectInteractionUpdate(reader);
                    break;
                }
                case Messages.AgentController.ObjectInteractionPromptUpdate:
                {
                    this.HandleObjectInteractionPromptUpdate(reader);
                    break;
                }
                case Messages.AgentController.ObjectInteractionCreate:
                {
                    this.HandleObjectInteractionCreate(reader);
                    break;
                }
                case Messages.AgentController.RequestSitOnObject:
                {
                    this.HandleRequestSitOnObject(reader);
                    break;
                }
                case Messages.AgentController.SitOnObject:
                {
                    this.HandleSitOnObject(reader);
                    break;
                }
                case Messages.AgentController.ExitSit:
                {
                    this.HandleExitSit(reader);
                    break;
                }
                case Messages.AgentController.SetAgentFiltersBody:
                {
                    this.HandleSetAgentFiltersBody(reader);
                    break;
                }
                case Messages.AgentController.RequestSetAgentFiltersBody:
                {
                    this.HandleRequestSetAgentFiltersBody(reader);
                    break;
                }
                case Messages.AgentController.SetCharacterUserProperty:
                {
                    this.HandleSetCharacterUserProperty(reader);
                    break;
                }
                // REMOVED 40.11.0.1810696  (2020-08-13)
                //case Messages.AgentController.CreateSpeechGraphicsPlayer:
                //{
                //    this.HandleCreateSpeechGraphicsPlayer(reader);
                //    break;
                //}
                case Messages.AgentController.RequestSpawnItem:
                {
                    this.HandleRequestSpawnItem(reader);
                    break;
                }
                case Messages.AgentController.RequestDeleteLatestSpawn:
                {
                    this.HandleRequestDeleteLatestSpawn(reader);
                    break;
                }
                case Messages.AgentController.RequestDeleteAllSpawns:
                {
                    this.HandleRequestDeleteAllSpawns(reader);
                    break;
                }
                case Messages.AgentController.UserReaction:
                {
                    // Added 2020-09-09
                    this.HandleUserReaction(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }
            
            return true;
        }

        void HandleCharacterControllerInput(BinaryReader reader)
        {
            var packet = new CharacterControllerInput(reader);
            OnCharacterControllerInput?.Invoke(this, packet);
        }

        void HandleObjectInteractionUpdate(BinaryReader reader)
        {
            var packet = new ObjectInteractionUpdate(reader);
            OnObjectInteractionUpdate?.Invoke(this, packet);
        }

        void HandleCharacterIKPoseDelta(BinaryReader reader)
        {
            var packet = new CharacterIKPoseDelta(reader);
            OnCharacterIKPoseDelta?.Invoke(this, packet);
        }

        void HandleCharacterIKPose(BinaryReader reader)
        {
            var packet = new CharacterIKPose(reader);
            OnCharacterIKPose?.Invoke(this, packet);
        }

        void HandleCharacterControlPointInputReliable(BinaryReader reader)
        {
            var packet = new CharacterControlPointInputReliable(reader);
            OnCharacterControlPointInputReliable?.Invoke(this, packet);
        }

        void HandleCharacterControllerInputReliable(BinaryReader reader)
        {
            var packet = new CharacterControllerInputReliable(reader);
            OnCharacterControllerInputReliable?.Invoke(this, packet);
        }

        void HandleCharacterControlPointInput(BinaryReader reader)
        {
            var packet = new CharacterControlPointInput(reader);
            OnCharacterControlPointInput?.Invoke(this, packet);
        }

        void HandleWarpCharacter(BinaryReader reader)
        {
            var packet = new WarpCharacter(reader);
            OnWarpCharacter?.Invoke(this, packet);
        }

        void HandleControlPoint(BinaryReader reader)
        {
            var packet = new ControlPoint(reader);
            OnControlPoint?.Invoke(this, packet);
        }

        void HandleRequestWarpCharacter(BinaryReader reader)
        {
            var packet = new RequestWarpCharacter(reader);
            OnRequestWarpCharacter?.Invoke(this, packet);
        }

        void HandleAgentPlayAnimation(BinaryReader reader)
        {
            var packet = new AgentPlayAnimation(reader);
            OnAgentPlayAnimation?.Invoke(this, packet);
        }

        void HandleRequestAgentPlayAnimation(BinaryReader reader)
        {
            var packet = new RequestAgentPlayAnimation(reader);
            OnRequestAgentPlayAnimation?.Invoke(this, packet);
        }

        void HandleRequestBehaviorStateUpdate(BinaryReader reader)
        {
            var packet = new RequestBehaviorStateUpdate(reader);
            OnRequestBehaviorStateUpdate?.Invoke(this, packet);
        }

        void HandleAttachToCharacterNode(BinaryReader reader)
        {
            var packet = new AttachToCharacterNode(reader);
            OnAttachToCharacterNode?.Invoke(this, packet);
        }

        void HandleDetachFromCharacterNode(BinaryReader reader)
        {
            var packet = new DetachFromCharacterNode(reader);
            OnDetachFromCharacterNode?.Invoke(this, packet);
        }

        void HandleRequestDetachFromCharacterNode(BinaryReader reader)
        {
            var packet = new RequestDetachFromCharacterNode(reader);
            OnRequestDetachFromCharacterNode?.Invoke(this, packet);
        }

        void HandleSetCharacterNodePhysics(BinaryReader reader)
        {
            var packet = new SetCharacterNodePhysics(reader);
            OnSetCharacterNodePhysics?.Invoke(this, packet);
        }

        void HandleWarpCharacterNode(BinaryReader reader)
        {
            var packet = new WarpCharacterNode(reader);
            OnWarpCharacterNode?.Invoke(this, packet);
        }

        void HandleCharacterIKBone(BinaryReader reader)
        {
            var packet = new CharacterIKBone(reader);
            OnCharacterIKBone?.Invoke(this, packet);
        }

        void HandleCharacterIKBoneDelta(BinaryReader reader)
        {
            var packet = new CharacterIKBoneDelta(reader);
            OnCharacterIKBoneDelta?.Invoke(this, packet);
        }

        void HandleObjectInteraction(BinaryReader reader)
        {
            var packet = new ObjectInteraction(reader);
            OnObjectInteraction?.Invoke(this, packet);
        }

        void HandleObjectInteractionPromptUpdate(BinaryReader reader)
        {
            var packet = new ObjectInteractionPromptUpdate(reader);
            OnObjectInteractionPromptUpdate?.Invoke(this, packet);
        }

        void HandleObjectInteractionCreate(BinaryReader reader)
        {
            var packet = new ObjectInteractionCreate(reader);
            OnObjectInteractionCreate?.Invoke(this, packet);
        }

        void HandleRequestSitOnObject(BinaryReader reader)
        {
            var packet = new RequestSitOnObject(reader);
            OnRequestSitOnObject?.Invoke(this, packet);
        }

        void HandleSitOnObject(BinaryReader reader)
        {
            var packet = new SitOnObject(reader);
            OnSitOnObject?.Invoke(this, packet);
        }

        void HandleExitSit(BinaryReader reader)
        {
            var packet = new ExitSit(reader);
            OnExitSit?.Invoke(this, packet);
        }

        void HandleSetAgentFiltersBody(BinaryReader reader)
        {
            var packet = new SetAgentFiltersBody(reader);
            OnSetAgentFiltersBody?.Invoke(this, packet);
        }

        void HandleRequestSetAgentFiltersBody(BinaryReader reader)
        {
            var packet = new RequestSetAgentFiltersBody(reader);
            OnRequestSetAgentFiltersBody?.Invoke(this, packet);
        }

        void HandleSetCharacterUserProperty(BinaryReader reader)
        {
            var packet = new SetCharacterUserProperty(reader);
            OnSetCharacterUserProperty?.Invoke(this, packet);
        }

        void HandleRequestSpawnItem(BinaryReader reader)
        {
            var packet = new RequestSpawnItem(reader);
            OnRequestSpawnItem?.Invoke(this, packet);
        }

        void HandleRequestDeleteLatestSpawn(BinaryReader reader)
        {
            var packet = new RequestDeleteLatestSpawn(reader);
            OnRequestDeleteLatestSpawn?.Invoke(this, packet);
        }

        void HandleRequestDeleteAllSpawns(BinaryReader reader)
        {
            var packet = new RequestDeleteAllSpawns(reader);
            OnRequestDeleteAllSpawns?.Invoke(this, packet);
        }

        void HandleUserReaction(BinaryReader reader)
        {
            var packet = new UserReaction(reader);
            OnUserReaction?.Invoke(this, packet);
        }
    }
}
