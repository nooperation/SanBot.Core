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

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.AgentController.ControlPoint:
                {
                    OnControlPoint?.Invoke(this, (ControlPoint)packet);
                    break;
                }
                case Messages.AgentController.WarpCharacter:
                {
                    OnWarpCharacter?.Invoke(this, (WarpCharacter)packet);
                    break;
                }
                case Messages.AgentController.RequestWarpCharacter:
                {
                    OnRequestWarpCharacter?.Invoke(this, (RequestWarpCharacter)packet);
                    break;
                }
                case Messages.AgentController.CharacterControlPointInput:
                {
                    OnCharacterControlPointInput?.Invoke(this, (CharacterControlPointInput)packet);
                    break;
                }
                case Messages.AgentController.CharacterControlPointInputReliable:
                {
                    OnCharacterControlPointInputReliable?.Invoke(this, (CharacterControlPointInputReliable)packet);
                    break;
                }
                case Messages.AgentController.CharacterControllerInput:
                {
                    OnCharacterControllerInput?.Invoke(this, (CharacterControllerInput)packet);
                    break;
                }
                case Messages.AgentController.CharacterControllerInputReliable:
                {
                    OnCharacterControllerInputReliable?.Invoke(this, (CharacterControllerInputReliable)packet);
                    break;
                }
                case Messages.AgentController.AgentPlayAnimation:
                {
                    OnAgentPlayAnimation?.Invoke(this, (AgentPlayAnimation)packet);
                    break;
                }
                case Messages.AgentController.RequestAgentPlayAnimation:
                {
                    OnRequestAgentPlayAnimation?.Invoke(this, (RequestAgentPlayAnimation)packet);
                    break;
                }
                case Messages.AgentController.RequestBehaviorStateUpdate:
                {
                    OnRequestBehaviorStateUpdate?.Invoke(this, (RequestBehaviorStateUpdate)packet);
                    break;
                }
                case Messages.AgentController.AttachToCharacterNode:
                {
                    OnAttachToCharacterNode?.Invoke(this, (AttachToCharacterNode)packet);
                    break;
                }
                case Messages.AgentController.DetachFromCharacterNode:
                {
                    OnDetachFromCharacterNode?.Invoke(this, (DetachFromCharacterNode)packet);
                    break;
                }
                case Messages.AgentController.RequestDetachFromCharacterNode:
                {
                    OnRequestDetachFromCharacterNode?.Invoke(this, (RequestDetachFromCharacterNode)packet);
                    break;
                }
                case Messages.AgentController.SetCharacterNodePhysics:
                {
                    OnSetCharacterNodePhysics?.Invoke(this, (SetCharacterNodePhysics)packet);
                    break;
                }
                case Messages.AgentController.WarpCharacterNode:
                {
                    OnWarpCharacterNode?.Invoke(this, (WarpCharacterNode)packet);
                    break;
                }
                case Messages.AgentController.CharacterIKBone:
                {
                    OnCharacterIKBone?.Invoke(this, (CharacterIKBone)packet);
                    break;
                }
                case Messages.AgentController.CharacterIKPose:
                {
                    OnCharacterIKPose?.Invoke(this, (CharacterIKPose)packet);
                    break;
                }
                case Messages.AgentController.CharacterIKBoneDelta:
                {
                    OnCharacterIKBoneDelta?.Invoke(this, (CharacterIKBoneDelta)packet);
                    break;
                }
                case Messages.AgentController.CharacterIKPoseDelta:
                {
                    OnCharacterIKPoseDelta?.Invoke(this, (CharacterIKPoseDelta)packet);
                    break;
                }
                case Messages.AgentController.ObjectInteraction:
                {
                    OnObjectInteraction?.Invoke(this, (ObjectInteraction)packet);
                    break;
                }
                case Messages.AgentController.ObjectInteractionUpdate:
                {
                    OnObjectInteractionUpdate?.Invoke(this, (ObjectInteractionUpdate)packet);
                    break;
                }
                case Messages.AgentController.ObjectInteractionPromptUpdate:
                {
                    OnObjectInteractionPromptUpdate?.Invoke(this, (ObjectInteractionPromptUpdate)packet);
                    break;
                }
                case Messages.AgentController.ObjectInteractionCreate:
                {
                    OnObjectInteractionCreate?.Invoke(this, (ObjectInteractionCreate)packet);
                    break;
                }
                case Messages.AgentController.RequestSitOnObject:
                {
                    OnRequestSitOnObject?.Invoke(this, (RequestSitOnObject)packet);
                    break;
                }
                case Messages.AgentController.SitOnObject:
                {
                    OnSitOnObject?.Invoke(this, (SitOnObject)packet);
                    break;
                }
                case Messages.AgentController.ExitSit:
                {
                    OnExitSit?.Invoke(this, (ExitSit)packet);
                    break;
                }
                case Messages.AgentController.SetAgentFiltersBody:
                {
                    OnSetAgentFiltersBody?.Invoke(this, (SetAgentFiltersBody)packet);
                    break;
                }
                case Messages.AgentController.RequestSetAgentFiltersBody:
                {
                    OnRequestSetAgentFiltersBody?.Invoke(this, (RequestSetAgentFiltersBody)packet);
                    break;
                }
                case Messages.AgentController.SetCharacterUserProperty:
                {
                    OnSetCharacterUserProperty?.Invoke(this, (SetCharacterUserProperty)packet);
                    break;
                }
                // REMOVED 40.11.0.1810696  (2020-08-13)
                //case Messages.AgentController.CreateSpeechGraphicsPlayer:
                //{
                //    OnCreateSpeechGraphicsPlayer?.Invoke(this, (CreateSpeechGraphicsPlayer)packet);
                //    break;
                //}
                case Messages.AgentController.RequestSpawnItem:
                {
                    OnRequestSpawnItem?.Invoke(this, (RequestSpawnItem)packet);
                    break;
                }
                case Messages.AgentController.RequestDeleteLatestSpawn:
                {
                    OnRequestDeleteLatestSpawn?.Invoke(this, (RequestDeleteLatestSpawn)packet);
                    break;
                }
                case Messages.AgentController.RequestDeleteAllSpawns:
                {
                    OnRequestDeleteAllSpawns?.Invoke(this, (RequestDeleteAllSpawns)packet);
                    break;
                }
                case Messages.AgentController.UserReaction:
                {
                    // Added 2020-09-09
                    OnUserReaction?.Invoke(this, (UserReaction)packet);
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
