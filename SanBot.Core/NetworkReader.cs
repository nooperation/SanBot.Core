using SanBot.Core.MessageHandlers;
using SanProtocol;
using SanProtocol.AgentController;
using SanProtocol.AnimationComponent;
using SanProtocol.Audio;
using SanProtocol.ClientRegion;
using SanProtocol.EditServer;
using SanProtocol.GameWorld;
using SanProtocol.RegionRegion;
using SanProtocol.Render;
using SanProtocol.Simulation;
using SanProtocol.WorldState;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SanBot.Core
{
    internal class NetworkReader
    {
        private readonly object _accountConductorLock;
        private readonly TcpClient _accountConductor;
        private readonly PacketBuffer _packetBuffer = new PacketBuffer();

        Thread? _readerThread;
        private volatile bool _isRunning = false;

        public NetworkReader(TcpClient accountConductor, object accountConductorLock)
        {
            _packetBuffer.OnProcessPackets = ProcessPackets;
            _packetBuffer.DecodePacket = DecodePacket;

            _accountConductor = accountConductor;
            _accountConductorLock = accountConductorLock;
        }

        public void Start()
        {
            Console.WriteLine("NetworkReader::Start()");

            if (_isRunning)
            {
                throw new Exception("It's already running");
            }

            _isRunning = true;
            _readerThread = new Thread(() =>
            {
                while (_isRunning)
                {
                    if(!Poll())
                    {
                        Thread.Sleep(1);
                    }
                }
            });
            _readerThread.Start();
        }
        public void Stop()
        {
            Console.WriteLine("NetworkReader::Stop()");

            if (!_isRunning)
            {
                throw new Exception("It's not running");
            }

            _isRunning = false;
            if (_readerThread != null)
            {
                _readerThread.Join();
            }
        }

        // TODO: Rewrite so we don't have to allocate for each packet processed
        private IPacket DecodePacket(byte[] packet)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(packet)))
            {
                var id = reader.ReadUInt32();

                switch (id)
                {
                    case 0:
                        return new VersionPacket(reader);
                    case Messages.AnimationComponent.FloatVariable:
                        return new FloatVariable(reader);
                    case Messages.AnimationComponent.FloatNodeVariable:
                        return new FloatNodeVariable(reader);
                    case Messages.AnimationComponent.FloatRangeNodeVariable:
                        return new FloatRangeNodeVariable(reader);
                    case Messages.AnimationComponent.VectorVariable:
                        return new VectorVariable(reader);
                    case Messages.AnimationComponent.QuaternionVariable:
                        return new QuaternionVariable(reader);
                    case Messages.AnimationComponent.Int8Variable:
                        return new Int8Variable(reader);
                    case Messages.AnimationComponent.BoolVariable:
                        return new BoolVariable(reader);
                    case Messages.AnimationComponent.CharacterTransform:
                        return new CharacterTransform(reader);
                    case Messages.AnimationComponent.CharacterTransformPersistent:
                        return new CharacterTransformPersistent(reader);
                    case Messages.AnimationComponent.CharacterAnimationDestroyed:
                        return new CharacterAnimationDestroyed(reader);
                    case Messages.AnimationComponent.AnimationOverride:
                        return new AnimationOverride(reader);
                    case Messages.AnimationComponent.BehaviorInternalState:
                        return new BehaviorInternalState(reader);
                    case Messages.AnimationComponent.CharacterBehaviorInternalState:
                        return new CharacterBehaviorInternalState(reader);
                    case Messages.AnimationComponent.BehaviorStateUpdate:
                        return new BehaviorStateUpdate(reader);
                    case Messages.AnimationComponent.BehaviorInitializationData:
                        return new BehaviorInitializationData(reader);
                    case Messages.AnimationComponent.CharacterSetPosition:
                        return new CharacterSetPosition(reader);
                    case Messages.AnimationComponent.PlayAnimation:
                        return new PlayAnimation(reader);
                    // AgentController
                    case Messages.AgentController.ControlPoint:
                        return new ControlPoint(reader);
                    case Messages.AgentController.WarpCharacter:
                        return new WarpCharacter(reader);
                    case Messages.AgentController.RequestWarpCharacter:
                        return new RequestWarpCharacter(reader);
                    case Messages.AgentController.CharacterControlPointInput:
                        return new CharacterControlPointInput(reader);
                    case Messages.AgentController.CharacterControlPointInputReliable:
                        return new CharacterControlPointInputReliable(reader);
                    case Messages.AgentController.CharacterControllerInput:
                        return new CharacterControllerInput(reader);
                    case Messages.AgentController.CharacterControllerInputReliable:
                        return new CharacterControllerInputReliable(reader);
                    case Messages.AgentController.AgentPlayAnimation:
                        return new AgentPlayAnimation(reader);
                    case Messages.AgentController.RequestAgentPlayAnimation:
                        return new RequestAgentPlayAnimation(reader);
                    case Messages.AgentController.RequestBehaviorStateUpdate:
                        return new RequestBehaviorStateUpdate(reader);
                    case Messages.AgentController.AttachToCharacterNode:
                        return new AttachToCharacterNode(reader);
                    case Messages.AgentController.DetachFromCharacterNode:
                        return new DetachFromCharacterNode(reader);
                    case Messages.AgentController.RequestDetachFromCharacterNode:
                        return new RequestDetachFromCharacterNode(reader);
                    case Messages.AgentController.SetCharacterNodePhysics:
                        return new SetCharacterNodePhysics(reader);
                    case Messages.AgentController.WarpCharacterNode:
                        return new WarpCharacterNode(reader);
                    case Messages.AgentController.CharacterIKBone:
                        return new CharacterIKBone(reader);
                    case Messages.AgentController.CharacterIKPose:
                        return new CharacterIKPose(reader);
                    case Messages.AgentController.CharacterIKBoneDelta:
                        return new CharacterIKBoneDelta(reader);
                    case Messages.AgentController.CharacterIKPoseDelta:
                        return new CharacterIKPoseDelta(reader);
                    case Messages.AgentController.ObjectInteraction:
                        return new ObjectInteraction(reader);
                    case Messages.AgentController.ObjectInteractionUpdate:
                        return new ObjectInteractionUpdate(reader);
                    case Messages.AgentController.ObjectInteractionPromptUpdate:
                        return new ObjectInteractionPromptUpdate(reader);
                    case Messages.AgentController.ObjectInteractionCreate:
                        return new ObjectInteractionCreate(reader);
                    case Messages.AgentController.RequestSitOnObject:
                        return new RequestSitOnObject(reader);
                    case Messages.AgentController.SitOnObject:
                        return new SitOnObject(reader);
                    case Messages.AgentController.ExitSit:
                        return new ExitSit(reader);
                    case Messages.AgentController.SetAgentFiltersBody:
                        return new SetAgentFiltersBody(reader);
                    case Messages.AgentController.RequestSetAgentFiltersBody:
                        return new RequestSetAgentFiltersBody(reader);
                    case Messages.AgentController.SetCharacterUserProperty:
                        return new SetCharacterUserProperty(reader);
                    // REMOVED 40.11.0.1810696  (2020-08-13)
                    //case Messages.AgentController.CreateSpeechGraphicsPlayer:
                    //{
                    //    return new CreateSpeechGraphicsPlayer(reader);
                    //    break;
                    //}
                    case Messages.AgentController.RequestSpawnItem:
                        return new RequestSpawnItem(reader);
                    case Messages.AgentController.RequestDeleteLatestSpawn:
                        return new RequestDeleteLatestSpawn(reader);
                    case Messages.AgentController.RequestDeleteAllSpawns:
                        return new RequestDeleteAllSpawns(reader);
                    case Messages.AgentController.UserReaction:
                        // Added 2020-09-09
                        return new UserReaction(reader);
                    // Audio
                    case Messages.Audio.LoadSound:
                        return new LoadSound(reader);
                    case Messages.Audio.PlaySound:
                        return new PlaySound(reader);
                    case Messages.Audio.PlayStream:
                        return new PlayStream(reader);
                    case Messages.Audio.StopBroadcastingSound:
                        return new StopBroadcastingSound(reader);
                    case Messages.Audio.SetAudioStream:
                        return new SetAudioStream(reader);
                    case Messages.Audio.SetMediaSource:
                        return new SetMediaSource(reader);
                    case Messages.Audio.PerformMediaAction:
                        return new PerformMediaAction(reader);
                    case Messages.Audio.StopSound:
                        return new StopSound(reader);
                    case Messages.Audio.SetLoudness:
                        return new SetLoudness(reader);
                    case Messages.Audio.SetPitch:
                        return new SetPitch(reader);
                    // REGION CLIENT
                    case Messages.ClientRegion.UserLogin:
                        return new SanProtocol.ClientRegion.UserLogin(reader);
                    case Messages.ClientRegion.UserLoginReply:
                        return new SanProtocol.ClientRegion.UserLoginReply(reader);
                    case Messages.ClientRegion.AddUser:
                        return new SanProtocol.ClientRegion.AddUser(reader);
                    case Messages.ClientRegion.RemoveUser:
                        return new SanProtocol.ClientRegion.RemoveUser(reader);
                    case Messages.ClientRegion.RenameUser:
                        return new RenameUser(reader);
                    case Messages.ClientRegion.ChatMessageToServer:
                        return new ChatMessageToServer(reader);
                    case Messages.ClientRegion.ChatMessageToClient:
                        return new ChatMessageToClient(reader);
                    case Messages.ClientRegion.VibrationPulseToClient:
                        return new VibrationPulseToClient(reader);
                    case Messages.ClientRegion.SetAgentController:
                        return new SetAgentController(reader);
                    case Messages.ClientRegion.TeleportTo:
                        return new TeleportTo(reader);
                    case Messages.ClientRegion.TeleportToUri:
                        return new TeleportToUri(reader);
                    case Messages.ClientRegion.TeleportToEditMode:
                        return new TeleportToEditMode(reader);
                    case Messages.ClientRegion.DebugTimeChangeToServer:
                        return new DebugTimeChangeToServer(reader);
                    case Messages.ClientRegion.DebugTimeChangeToClient:
                        return new DebugTimeChangeToClient(reader);
                    case Messages.ClientRegion.VisualDebuggerCaptureToServer:
                        return new VisualDebuggerCaptureToServer(reader);
                    case Messages.ClientRegion.VisualDebuggerCaptureToClient:
                        return new VisualDebuggerCaptureToClient(reader);
                    case Messages.ClientRegion.ScriptModalDialog:
                        return new ScriptModalDialog(reader);
                    case Messages.ClientRegion.ScriptModalDialogResponse:
                        return new ScriptModalDialogResponse(reader);
                    case Messages.ClientRegion.TwitchEventSubscription:
                        return new TwitchEventSubscription(reader);
                    case Messages.ClientRegion.TwitchEvent:
                        return new TwitchEvent(reader);
                    case Messages.ClientRegion.ClientStaticReady:
                        return new ClientStaticReady(reader);
                    case Messages.ClientRegion.ClientDynamicReady:
                        return new ClientDynamicReady(reader);
                    case Messages.ClientRegion.InitialChunkSubscribed:
                        return new InitialChunkSubscribed(reader);
                    case Messages.ClientRegion.ClientRegionCommandMessage:
                        return new ClientRegionCommandMessage(reader);
                    case Messages.ClientRegion.ClientKickNotification:
                        return new ClientKickNotification(reader);
                    case Messages.ClientRegion.ClientSmiteNotification:
                        return new ClientSmiteNotification(reader);
                    case Messages.ClientRegion.ClientMuteNotification:
                        return new ClientMuteNotification(reader);
                    case Messages.ClientRegion.ClientVoiceBroadcastStartNotification:
                        return new ClientVoiceBroadcastStartNotification(reader);
                    case Messages.ClientRegion.ClientVoiceBroadcastStopNotification:
                        return new ClientVoiceBroadcastStopNotification(reader);
                    case Messages.ClientRegion.ClientRuntimeInventoryUpdatedNotification:
                        return new ClientRuntimeInventoryUpdatedNotification(reader);
                    case Messages.ClientRegion.ClientSetRegionBroadcasted:
                        return new ClientSetRegionBroadcasted(reader);
                    case Messages.ClientRegion.SubscribeCommand:
                        return new SubscribeCommand(reader);
                    case Messages.ClientRegion.UnsubscribeCommand:
                        return new UnsubscribeCommand(reader);
                    case Messages.ClientRegion.ClientCommand:
                        return new ClientCommand(reader);
                    case Messages.ClientRegion.RequestDropPortal:
                        return new RequestDropPortal(reader);
                    case Messages.ClientRegion.OpenStoreListing:
                        return new OpenStoreListing(reader);
                    case Messages.ClientRegion.OpenUserStore:
                        return new OpenUserStore(reader);
                    case Messages.ClientRegion.OpenQuestCharacterDialog:
                        return new OpenQuestCharacterDialog(reader);
                    case Messages.ClientRegion.UIScriptableBarStart:
                        return new UIScriptableBarStart(reader);
                    case Messages.ClientRegion.UIScriptableBarStopped:
                        return new UIScriptableBarStopped(reader);
                    case Messages.ClientRegion.UIScriptableBarCancel:
                        return new UIScriptableBarCancel(reader);
                    case Messages.ClientRegion.UIHintTextUpdate:
                        return new UIHintTextUpdate(reader);
                    case Messages.ClientRegion.QuestOfferResponse:
                        return new QuestOfferResponse(reader);
                    case Messages.ClientRegion.QuestCompleted:
                        return new QuestCompleted(reader);
                    case Messages.ClientRegion.QuestRemoved:
                        return new QuestRemoved(reader);
                    case Messages.ClientRegion.ShowWorldDetail:
                        return new ShowWorldDetail(reader);
                    case Messages.ClientRegion.ShowTutorialHint:
                        return new ShowTutorialHint(reader);
                    case Messages.ClientRegion.TutorialHintsSetEnabled:
                        return new TutorialHintsSetEnabled(reader);
                    case Messages.ClientRegion.ReactionDefinition:
                        return new ReactionDefinition(reader);
                    case Messages.ClientRegion.SystemReactionDefinition:
                        return new SystemReactionDefinition(reader);
                    case Messages.ClientRegion.UpdateReactions:
                        return new UpdateReactions(reader);
                    case Messages.ClientRegion.AddReaction:
                        return new AddReaction(reader);
                    case Messages.ClientRegion.RemoveReaction:
                        return new RemoveReaction(reader);
                    // EDIT SERVER 
                    case Messages.EditServer.UserLogin:
                        return new SanProtocol.EditServer.UserLogin(reader);
                    case Messages.EditServer.UserLoginReply:
                        return new SanProtocol.EditServer.UserLoginReply(reader);
                    case Messages.EditServer.AddUser:
                        return new SanProtocol.EditServer.AddUser(reader);
                    case Messages.EditServer.RemoveUser:
                        return new SanProtocol.EditServer.RemoveUser(reader);
                    case Messages.EditServer.OpenWorkspace:
                        return new OpenWorkspace(reader);
                    case Messages.EditServer.CloseWorkspace:
                        return new CloseWorkspace(reader);
                    case Messages.EditServer.EditWorkspaceCommand:
                        return new EditWorkspaceCommand(reader);
                    case Messages.EditServer.SaveWorkspace:
                        return new SaveWorkspace(reader);
                    case Messages.EditServer.SaveWorkspaceReply:
                        return new SaveWorkspaceReply(reader);
                    case Messages.EditServer.BuildWorkspace:
                        return new BuildWorkspace(reader);
                    case Messages.EditServer.UpdateWorkspaceClientBuiltBakeData:
                        return new UpdateWorkspaceClientBuiltBakeData(reader);
                    case Messages.EditServer.BuildWorkspaceCompileReply:
                        return new BuildWorkspaceCompileReply(reader);
                    case Messages.EditServer.BuildWorkspaceProgressUpdate:
                        return new BuildWorkspaceProgressUpdate(reader);
                    case Messages.EditServer.BuildWorkspaceUploadReply:
                        return new BuildWorkspaceUploadReply(reader);
                    case Messages.EditServer.WorkspaceReadyReply:
                        return new WorkspaceReadyReply(reader);
                    case Messages.EditServer.SaveWorkspaceSelectionToInventory:
                        return new SaveWorkspaceSelectionToInventory(reader);
                    case Messages.EditServer.SaveWorkspaceSelectionToInventoryReply:
                        return new SaveWorkspaceSelectionToInventoryReply(reader);
                    case Messages.EditServer.InventoryCreateItem:
                        return new InventoryCreateItem(reader);
                    case Messages.EditServer.InventoryDeleteItem:
                        return new InventoryDeleteItem(reader);
                    case Messages.EditServer.InventoryChangeItemName:
                        return new InventoryChangeItemName(reader);
                    case Messages.EditServer.InventoryChangeItemState:
                        return new InventoryChangeItemState(reader);
                    case Messages.EditServer.InventoryModifyItemThumbnailAssetId:
                        return new InventoryModifyItemThumbnailAssetId(reader);
                    case Messages.EditServer.InventoryModifyItemCapabilities:
                        return new InventoryModifyItemCapabilities(reader);
                    case Messages.EditServer.InventorySaveItem:
                        return new InventorySaveItem(reader);
                    case Messages.EditServer.InventoryUpdateItemReply:
                        return new InventoryUpdateItemReply(reader);
                    case Messages.EditServer.InventoryItemUpload:
                        return new InventoryItemUpload(reader);
                    case Messages.EditServer.InventoryItemUploadReply:
                        return new InventoryItemUploadReply(reader);
                    case Messages.EditServer.InventoryCreateListing:
                        return new InventoryCreateListing(reader);
                    case Messages.EditServer.InventoryCreateListingReply:
                        return new InventoryCreateListingReply(reader);
                    case Messages.EditServer.BeginEditServerSpawn:
                        return new BeginEditServerSpawn(reader);
                    case Messages.EditServer.EditServerSpawnReady:
                        return new EditServerSpawnReady(reader);
                    // GAME WORLD
                    case Messages.GameWorld.Timestamp:
                        return new SanProtocol.GameWorld.Timestamp(reader);
                    case Messages.GameWorld.MoveEntity:
                        return new MoveEntity(reader);
                    case Messages.GameWorld.ChangeMaterialVectorParam:
                        return new ChangeMaterialVectorParam(reader);
                    case Messages.GameWorld.ChangeMaterialFloatParam:
                        return new ChangeMaterialFloatParam(reader);
                    case Messages.GameWorld.ChangeMaterial:
                        return new ChangeMaterial(reader);
                    case Messages.GameWorld.StaticMeshFlagsChanged:
                        return new StaticMeshFlagsChanged(reader);
                    case Messages.GameWorld.StaticMeshScaleChanged:
                        return new StaticMeshScaleChanged(reader);
                    case Messages.GameWorld.RiggedMeshFlagsChange:
                        return new RiggedMeshFlagsChange(reader);
                    case Messages.GameWorld.RiggedMeshScaleChanged:
                        return new RiggedMeshScaleChanged(reader);
                    case Messages.GameWorld.ScriptCameraMessage:
                        return new ScriptCameraMessage(reader);
                    case Messages.GameWorld.ScriptCameraCapture:
                        return new ScriptCameraCapture(reader);
                    case Messages.GameWorld.UpdateRuntimeInventorySettings:
                        return new UpdateRuntimeInventorySettings(reader);
                    // REGIONREGION
                    case Messages.RegionRegion.DynamicSubscribe:
                        return new DynamicSubscribe(reader);
                    case Messages.RegionRegion.DynamicPlayback:
                        return new DynamicPlayback(reader);
                    case Messages.RegionRegion.MasterFrameSync:
                        return new MasterFrameSync(reader);
                    case Messages.RegionRegion.AgentControllerMapping:
                        return new AgentControllerMapping(reader);
                    // RENDER
                    case Messages.Render.LightStateChanged:
                        return new LightStateChanged(reader);
                    // SIMULATION
                    case Messages.Simulation.InitialTimestamp:
                        return new InitialTimestamp(reader);
                    case Messages.Simulation.Timestamp:
                        return new SanProtocol.Simulation.Timestamp(reader);
                    case Messages.Simulation.SetWorldGravityMagnitude:
                        return new SetWorldGravityMagnitude(reader);
                    case Messages.Simulation.ActiveRigidBodyUpdate:
                        return new ActiveRigidBodyUpdate(reader);
                    case Messages.Simulation.RigidBodyDeactivated:
                        return new RigidBodyDeactivated(reader);
                    case Messages.Simulation.RigidBodyPropertyChanged:
                        return new RigidBodyPropertyChanged(reader);
                    case Messages.Simulation.RigidBodyDestroyed:
                        return new RigidBodyDestroyed(reader);
                    //WORLDSTATE
                    case Messages.WorldState.CreateWorld:
                        return new CreateWorld(reader);
                    case Messages.WorldState.DestroyWorld:
                        return new DestroyWorld(reader);
                    case Messages.WorldState.RigidBodyComponentInitialState:
                        return new RigidBodyComponentInitialState(reader);
                    case Messages.WorldState.AnimationComponentInitialState:
                        return new AnimationComponentInitialState(reader);
                    case Messages.WorldState.LoadClusterDefinition:
                        return new LoadClusterDefinition(reader);
                    case Messages.WorldState.ComponentRelativeTransform:
                        return new ComponentRelativeTransform(reader);
                    case Messages.WorldState.InitiateCluster:
                        return new InitiateCluster(reader);
                    case Messages.WorldState.CreateClusterViaDefinition:
                        return new CreateClusterViaDefinition(reader);
                    case Messages.WorldState.DestroyCluster:
                        return new DestroyCluster(reader);
                    case Messages.WorldState.DestroyObject:
                        return new DestroyObject(reader);
                    case Messages.WorldState.DestroySourceIdSpace:
                        return new DestroySourceIdSpace(reader);
                    case Messages.WorldState.CreateCharacterNode:
                        return new CreateCharacterNode(reader);
                    case Messages.WorldState.CreateAgentController:
                        return new CreateAgentController(reader);
                    case Messages.WorldState.DestroyAgentController:
                        return new DestroyAgentController(reader);
                    default:
                    {
                        Console.WriteLine("NetworkReader: Unhandled Message " + id);
                        throw new Exception("NetworkReader: Unhandled Message " + id);
                    }
                }
            }
        }

        private readonly byte[] _pollBuffer = new byte[16384];
        public bool Poll()
        {
            if (_accountConductor.Available == 0)
            {
                // Not exactly thread safe, but we'll check it again eventually anyways
                return false;
            }
            
            lock(_accountConductorLock)
            {
                while (_accountConductor.Available > 0)
                {
                    var instream = _accountConductor.GetStream();
                    var numBytesRead = instream.Read(_pollBuffer, 0, _pollBuffer.Length);
                    _packetBuffer.AppendBytes(_pollBuffer, numBytesRead);
                }
            }
 
            if(_packetBuffer.Packets.Count > 0)
            {
                _packetBuffer.ProcessPacketQueue();
                return true;
            }

            return false;
        }

        private object _availablePacketsLock = new object();
        private List<IPacket> _availablePackets { get; set; } = new List<IPacket>();

        private void ProcessPackets(List<IPacket> packets)
        {
            lock(_availablePacketsLock)
            {
                _availablePackets.AddRange(packets);
            }
        }

        public List<IPacket>? GetAvailablePackets()
        {
            lock (_availablePacketsLock)
            {
                if(_availablePackets.Count == 0)
                {
                    return null;
                }

                var newPackets = _availablePackets;
                _availablePackets = new List<IPacket>();

                return newPackets;
            }
        }
    }
}
