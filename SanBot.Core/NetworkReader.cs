using System.Net.Sockets;
using SanProtocol;
using SanProtocol.AgentController;
using SanProtocol.AnimationComponent;
using SanProtocol.Audio;
using SanProtocol.ClientKafka;
using SanProtocol.ClientRegion;
using SanProtocol.ClientVoice;
using SanProtocol.EditServer;
using SanProtocol.GameWorld;
using SanProtocol.RegionRegion;
using SanProtocol.Render;
using SanProtocol.Simulation;
using SanProtocol.WorldState;

namespace SanBot.Core
{
    internal class NetworkReader
    {
        private readonly object _accountConductorLock;
        private readonly TcpClient _accountConductor;
        private readonly PacketBuffer _packetBuffer = new();

        private readonly string _name = "";

        private Thread? _readerThread;
        private volatile bool _isRunning = false;

        public NetworkReader(TcpClient accountConductor, object accountConductorLock, string name = "")
        {
            _packetBuffer.OnProcessPackets = ProcessPackets;
            _packetBuffer.DecodePacket = DecodePacket;

            _accountConductor = accountConductor;
            _accountConductorLock = accountConductorLock;

            _name = name;
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
                    if (!Poll())
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
            _readerThread?.Join();
        }

        // TODO: Rewrite so we don't have to allocate for each packet processed
        private IPacket DecodePacket(byte[] packet)
        {
            using BinaryReader reader = new(new MemoryStream(packet));
            var id = reader.ReadUInt32();

            switch (id)
            {
                case 0:
                    return new VersionPacket(reader);
                case Messages.AnimationComponentMessages.FloatVariable:
                    return new FloatVariable(reader);
                case Messages.AnimationComponentMessages.FloatNodeVariable:
                    return new FloatNodeVariable(reader);
                case Messages.AnimationComponentMessages.FloatRangeNodeVariable:
                    return new FloatRangeNodeVariable(reader);
                case Messages.AnimationComponentMessages.VectorVariable:
                    return new VectorVariable(reader);
                case Messages.AnimationComponentMessages.QuaternionVariable:
                    return new QuaternionVariable(reader);
                case Messages.AnimationComponentMessages.Int8Variable:
                    return new Int8Variable(reader);
                case Messages.AnimationComponentMessages.BoolVariable:
                    return new BoolVariable(reader);
                case Messages.AnimationComponentMessages.CharacterTransform:
                    return new CharacterTransform(reader);
                case Messages.AnimationComponentMessages.CharacterTransformPersistent:
                    return new CharacterTransformPersistent(reader);
                case Messages.AnimationComponentMessages.CharacterAnimationDestroyed:
                    return new CharacterAnimationDestroyed(reader);
                case Messages.AnimationComponentMessages.AnimationOverride:
                    return new AnimationOverride(reader);
                case Messages.AnimationComponentMessages.BehaviorInternalState:
                    return new BehaviorInternalState(reader);
                case Messages.AnimationComponentMessages.CharacterBehaviorInternalState:
                    return new CharacterBehaviorInternalState(reader);
                case Messages.AnimationComponentMessages.BehaviorStateUpdate:
                    return new BehaviorStateUpdate(reader);
                case Messages.AnimationComponentMessages.BehaviorInitializationData:
                    return new BehaviorInitializationData(reader);
                case Messages.AnimationComponentMessages.CharacterSetPosition:
                    return new CharacterSetPosition(reader);
                case Messages.AnimationComponentMessages.PlayAnimation:
                    return new PlayAnimation(reader);
                // AgentController
                case Messages.AgentControllerMessages.ControlPoint:
                    return new ControlPoint(reader);
                case Messages.AgentControllerMessages.WarpCharacter:
                    return new WarpCharacter(reader);
                case Messages.AgentControllerMessages.RequestWarpCharacter:
                    return new RequestWarpCharacter(reader);
                case Messages.AgentControllerMessages.CharacterControlPointInput:
                    return new CharacterControlPointInput(reader);
                case Messages.AgentControllerMessages.CharacterControlPointInputReliable:
                    return new CharacterControlPointInputReliable(reader);
                case Messages.AgentControllerMessages.CharacterControllerInput:
                    return new CharacterControllerInput(reader);
                case Messages.AgentControllerMessages.CharacterControllerInputReliable:
                    return new CharacterControllerInputReliable(reader);
                case Messages.AgentControllerMessages.AgentPlayAnimation:
                    return new AgentPlayAnimation(reader);
                case Messages.AgentControllerMessages.RequestAgentPlayAnimation:
                    return new RequestAgentPlayAnimation(reader);
                case Messages.AgentControllerMessages.RequestBehaviorStateUpdate:
                    return new RequestBehaviorStateUpdate(reader);
                case Messages.AgentControllerMessages.AttachToCharacterNode:
                    return new AttachToCharacterNode(reader);
                case Messages.AgentControllerMessages.DetachFromCharacterNode:
                    return new DetachFromCharacterNode(reader);
                case Messages.AgentControllerMessages.RequestDetachFromCharacterNode:
                    return new RequestDetachFromCharacterNode(reader);
                case Messages.AgentControllerMessages.SetCharacterNodePhysics:
                    return new SetCharacterNodePhysics(reader);
                case Messages.AgentControllerMessages.WarpCharacterNode:
                    return new WarpCharacterNode(reader);
                case Messages.AgentControllerMessages.CharacterIKBone:
                    return new CharacterIKBone(reader);
                case Messages.AgentControllerMessages.CharacterIKPose:
                    return new CharacterIKPose(reader);
                case Messages.AgentControllerMessages.CharacterIKBoneDelta:
                    return new CharacterIKBoneDelta(reader);
                case Messages.AgentControllerMessages.CharacterIKPoseDelta:
                    return new CharacterIKPoseDelta(reader);
                case Messages.AgentControllerMessages.ObjectInteraction:
                    return new ObjectInteraction(reader);
                case Messages.AgentControllerMessages.ObjectInteractionUpdate:
                    return new ObjectInteractionUpdate(reader);
                case Messages.AgentControllerMessages.ObjectInteractionPromptUpdate:
                    return new ObjectInteractionPromptUpdate(reader);
                case Messages.AgentControllerMessages.ObjectInteractionCreate:
                    return new ObjectInteractionCreate(reader);
                case Messages.AgentControllerMessages.RequestSitOnObject:
                    return new RequestSitOnObject(reader);
                case Messages.AgentControllerMessages.SitOnObject:
                    return new SitOnObject(reader);
                case Messages.AgentControllerMessages.ExitSit:
                    return new ExitSit(reader);
                case Messages.AgentControllerMessages.SetAgentFiltersBody:
                    return new SetAgentFiltersBody(reader);
                case Messages.AgentControllerMessages.RequestSetAgentFiltersBody:
                    return new RequestSetAgentFiltersBody(reader);
                case Messages.AgentControllerMessages.SetCharacterUserProperty:
                    return new SetCharacterUserProperty(reader);
                // REMOVED 40.11.0.1810696  (2020-08-13)
                //case Messages.AgentController.CreateSpeechGraphicsPlayer:
                //{
                //    return new CreateSpeechGraphicsPlayer(reader);
                //    break;
                //}
                case Messages.AgentControllerMessages.RequestSpawnItem:
                    return new RequestSpawnItem(reader);
                case Messages.AgentControllerMessages.RequestDeleteLatestSpawn:
                    return new RequestDeleteLatestSpawn(reader);
                case Messages.AgentControllerMessages.RequestDeleteAllSpawns:
                    return new RequestDeleteAllSpawns(reader);
                case Messages.AgentControllerMessages.UserReaction:
                    // Added 2020-09-09
                    return new UserReaction(reader);
                // Audio
                case Messages.AudioMessages.LoadSound:
                    return new LoadSound(reader);
                case Messages.AudioMessages.PlaySound:
                    return new PlaySound(reader);
                case Messages.AudioMessages.PlayStream:
                    return new PlayStream(reader);
                case Messages.AudioMessages.StopBroadcastingSound:
                    return new StopBroadcastingSound(reader);
                case Messages.AudioMessages.SetAudioStream:
                    return new SetAudioStream(reader);
                case Messages.AudioMessages.SetMediaSource:
                    return new SetMediaSource(reader);
                case Messages.AudioMessages.PerformMediaAction:
                    return new PerformMediaAction(reader);
                case Messages.AudioMessages.StopSound:
                    return new StopSound(reader);
                case Messages.AudioMessages.SetLoudness:
                    return new SetLoudness(reader);
                case Messages.AudioMessages.SetPitch:
                    return new SetPitch(reader);
                // REGION CLIENT
                case Messages.ClientRegionMessages.UserLogin:
                    return new SanProtocol.ClientRegion.UserLogin(reader);
                case Messages.ClientRegionMessages.UserLoginReply:
                    return new SanProtocol.ClientRegion.UserLoginReply(reader);
                case Messages.ClientRegionMessages.AddUser:
                    return new SanProtocol.ClientRegion.AddUser(reader);
                case Messages.ClientRegionMessages.RemoveUser:
                    return new SanProtocol.ClientRegion.RemoveUser(reader);
                case Messages.ClientRegionMessages.RenameUser:
                    return new RenameUser(reader);
                case Messages.ClientRegionMessages.ChatMessageToServer:
                    return new ChatMessageToServer(reader);
                case Messages.ClientRegionMessages.ChatMessageToClient:
                    return new ChatMessageToClient(reader);
                case Messages.ClientRegionMessages.VibrationPulseToClient:
                    return new VibrationPulseToClient(reader);
                case Messages.ClientRegionMessages.SetAgentController:
                    return new SetAgentController(reader);
                case Messages.ClientRegionMessages.TeleportTo:
                    return new TeleportTo(reader);
                case Messages.ClientRegionMessages.TeleportToUri:
                    return new TeleportToUri(reader);
                case Messages.ClientRegionMessages.TeleportToEditMode:
                    return new TeleportToEditMode(reader);
                case Messages.ClientRegionMessages.DebugTimeChangeToServer:
                    return new DebugTimeChangeToServer(reader);
                case Messages.ClientRegionMessages.DebugTimeChangeToClient:
                    return new DebugTimeChangeToClient(reader);
                case Messages.ClientRegionMessages.VisualDebuggerCaptureToServer:
                    return new VisualDebuggerCaptureToServer(reader);
                case Messages.ClientRegionMessages.VisualDebuggerCaptureToClient:
                    return new VisualDebuggerCaptureToClient(reader);
                case Messages.ClientRegionMessages.ScriptModalDialog:
                    return new ScriptModalDialog(reader);
                case Messages.ClientRegionMessages.ScriptModalDialogResponse:
                    return new ScriptModalDialogResponse(reader);
                case Messages.ClientRegionMessages.TwitchEventSubscription:
                    return new TwitchEventSubscription(reader);
                case Messages.ClientRegionMessages.TwitchEvent:
                    return new TwitchEvent(reader);
                case Messages.ClientRegionMessages.ClientStaticReady:
                    return new ClientStaticReady(reader);
                case Messages.ClientRegionMessages.ClientDynamicReady:
                    return new ClientDynamicReady(reader);
                case Messages.ClientRegionMessages.InitialChunkSubscribed:
                    return new InitialChunkSubscribed(reader);
                case Messages.ClientRegionMessages.ClientRegionCommandMessage:
                    return new ClientRegionCommandMessage(reader);
                case Messages.ClientRegionMessages.ClientKickNotification:
                    return new ClientKickNotification(reader);
                case Messages.ClientRegionMessages.ClientSmiteNotification:
                    return new ClientSmiteNotification(reader);
                case Messages.ClientRegionMessages.ClientMuteNotification:
                    return new ClientMuteNotification(reader);
                case Messages.ClientRegionMessages.ClientVoiceBroadcastStartNotification:
                    return new ClientVoiceBroadcastStartNotification(reader);
                case Messages.ClientRegionMessages.ClientVoiceBroadcastStopNotification:
                    return new ClientVoiceBroadcastStopNotification(reader);
                case Messages.ClientRegionMessages.ClientRuntimeInventoryUpdatedNotification:
                    return new ClientRuntimeInventoryUpdatedNotification(reader);
                case Messages.ClientRegionMessages.ClientSetRegionBroadcasted:
                    return new ClientSetRegionBroadcasted(reader);
                case Messages.ClientRegionMessages.SubscribeCommand:
                    return new SubscribeCommand(reader);
                case Messages.ClientRegionMessages.UnsubscribeCommand:
                    return new UnsubscribeCommand(reader);
                case Messages.ClientRegionMessages.ClientCommand:
                    return new ClientCommand(reader);
                case Messages.ClientRegionMessages.RequestDropPortal:
                    return new RequestDropPortal(reader);
                case Messages.ClientRegionMessages.OpenStoreListing:
                    return new OpenStoreListing(reader);
                case Messages.ClientRegionMessages.OpenUserStore:
                    return new OpenUserStore(reader);
                case Messages.ClientRegionMessages.OpenQuestCharacterDialog:
                    return new OpenQuestCharacterDialog(reader);
                case Messages.ClientRegionMessages.UIScriptableBarStart:
                    return new UIScriptableBarStart(reader);
                case Messages.ClientRegionMessages.UIScriptableBarStopped:
                    return new UIScriptableBarStopped(reader);
                case Messages.ClientRegionMessages.UIScriptableBarCancel:
                    return new UIScriptableBarCancel(reader);
                case Messages.ClientRegionMessages.UIHintTextUpdate:
                    return new UIHintTextUpdate(reader);
                case Messages.ClientRegionMessages.QuestOfferResponse:
                    return new QuestOfferResponse(reader);
                case Messages.ClientRegionMessages.QuestCompleted:
                    return new QuestCompleted(reader);
                case Messages.ClientRegionMessages.QuestRemoved:
                    return new QuestRemoved(reader);
                case Messages.ClientRegionMessages.ShowWorldDetail:
                    return new ShowWorldDetail(reader);
                case Messages.ClientRegionMessages.ShowTutorialHint:
                    return new ShowTutorialHint(reader);
                case Messages.ClientRegionMessages.TutorialHintsSetEnabled:
                    return new TutorialHintsSetEnabled(reader);
                case Messages.ClientRegionMessages.ReactionDefinition:
                    return new ReactionDefinition(reader);
                case Messages.ClientRegionMessages.SystemReactionDefinition:
                    return new SystemReactionDefinition(reader);
                case Messages.ClientRegionMessages.UpdateReactions:
                    return new UpdateReactions(reader);
                case Messages.ClientRegionMessages.AddReaction:
                    return new AddReaction(reader);
                case Messages.ClientRegionMessages.RemoveReaction:
                    return new RemoveReaction(reader);
                case Messages.ClientRegionMessages.UIScriptableScoreBoard:
                    return new UIScriptableScoreBoard(reader);
                // EDIT SERVER 
                case Messages.EditServerMessages.UserLogin:
                    return new SanProtocol.EditServer.UserLogin(reader);
                case Messages.EditServerMessages.UserLoginReply:
                    return new SanProtocol.EditServer.UserLoginReply(reader);
                case Messages.EditServerMessages.AddUser:
                    return new SanProtocol.EditServer.AddUser(reader);
                case Messages.EditServerMessages.RemoveUser:
                    return new SanProtocol.EditServer.RemoveUser(reader);
                case Messages.EditServerMessages.OpenWorkspace:
                    return new OpenWorkspace(reader);
                case Messages.EditServerMessages.CloseWorkspace:
                    return new CloseWorkspace(reader);
                case Messages.EditServerMessages.EditWorkspaceCommand:
                    return new EditWorkspaceCommand(reader);
                case Messages.EditServerMessages.SaveWorkspace:
                    return new SaveWorkspace(reader);
                case Messages.EditServerMessages.SaveWorkspaceReply:
                    return new SaveWorkspaceReply(reader);
                case Messages.EditServerMessages.BuildWorkspace:
                    return new BuildWorkspace(reader);
                case Messages.EditServerMessages.UpdateWorkspaceClientBuiltBakeData:
                    return new UpdateWorkspaceClientBuiltBakeData(reader);
                case Messages.EditServerMessages.BuildWorkspaceCompileReply:
                    return new BuildWorkspaceCompileReply(reader);
                case Messages.EditServerMessages.BuildWorkspaceProgressUpdate:
                    return new BuildWorkspaceProgressUpdate(reader);
                case Messages.EditServerMessages.BuildWorkspaceUploadReply:
                    return new BuildWorkspaceUploadReply(reader);
                case Messages.EditServerMessages.WorkspaceReadyReply:
                    return new WorkspaceReadyReply(reader);
                case Messages.EditServerMessages.SaveWorkspaceSelectionToInventory:
                    return new SaveWorkspaceSelectionToInventory(reader);
                case Messages.EditServerMessages.SaveWorkspaceSelectionToInventoryReply:
                    return new SaveWorkspaceSelectionToInventoryReply(reader);
                case Messages.EditServerMessages.InventoryCreateItem:
                    return new InventoryCreateItem(reader);
                case Messages.EditServerMessages.InventoryDeleteItem:
                    return new InventoryDeleteItem(reader);
                case Messages.EditServerMessages.InventoryChangeItemName:
                    return new InventoryChangeItemName(reader);
                case Messages.EditServerMessages.InventoryChangeItemState:
                    return new InventoryChangeItemState(reader);
                case Messages.EditServerMessages.InventoryModifyItemThumbnailAssetId:
                    return new InventoryModifyItemThumbnailAssetId(reader);
                case Messages.EditServerMessages.InventoryModifyItemCapabilities:
                    return new InventoryModifyItemCapabilities(reader);
                case Messages.EditServerMessages.InventorySaveItem:
                    return new InventorySaveItem(reader);
                case Messages.EditServerMessages.InventoryUpdateItemReply:
                    return new InventoryUpdateItemReply(reader);
                case Messages.EditServerMessages.InventoryItemUpload:
                    return new InventoryItemUpload(reader);
                case Messages.EditServerMessages.InventoryItemUploadReply:
                    return new InventoryItemUploadReply(reader);
                case Messages.EditServerMessages.InventoryCreateListing:
                    return new InventoryCreateListing(reader);
                case Messages.EditServerMessages.InventoryCreateListingReply:
                    return new InventoryCreateListingReply(reader);
                case Messages.EditServerMessages.BeginEditServerSpawn:
                    return new BeginEditServerSpawn(reader);
                case Messages.EditServerMessages.EditServerSpawnReady:
                    return new EditServerSpawnReady(reader);
                // GAME WORLD
                case Messages.GameWorldMessages.Timestamp:
                    return new SanProtocol.GameWorld.Timestamp(reader);
                case Messages.GameWorldMessages.MoveEntity:
                    return new MoveEntity(reader);
                case Messages.GameWorldMessages.ChangeMaterialVectorParam:
                    return new ChangeMaterialVectorParam(reader);
                case Messages.GameWorldMessages.ChangeMaterialFloatParam:
                    return new ChangeMaterialFloatParam(reader);
                case Messages.GameWorldMessages.ChangeMaterial:
                    return new ChangeMaterial(reader);
                case Messages.GameWorldMessages.StaticMeshFlagsChanged:
                    return new StaticMeshFlagsChanged(reader);
                case Messages.GameWorldMessages.StaticMeshScaleChanged:
                    return new StaticMeshScaleChanged(reader);
                case Messages.GameWorldMessages.RiggedMeshFlagsChange:
                    return new RiggedMeshFlagsChange(reader);
                case Messages.GameWorldMessages.RiggedMeshScaleChanged:
                    return new RiggedMeshScaleChanged(reader);
                case Messages.GameWorldMessages.ScriptCameraMessage:
                    return new ScriptCameraMessage(reader);
                case Messages.GameWorldMessages.ScriptCameraCapture:
                    return new ScriptCameraCapture(reader);
                case Messages.GameWorldMessages.UpdateRuntimeInventorySettings:
                    return new UpdateRuntimeInventorySettings(reader);
                // REGIONREGION
                case Messages.RegionRegionMessages.DynamicSubscribe:
                    return new DynamicSubscribe(reader);
                case Messages.RegionRegionMessages.DynamicPlayback:
                    return new DynamicPlayback(reader);
                case Messages.RegionRegionMessages.MasterFrameSync:
                    return new MasterFrameSync(reader);
                case Messages.RegionRegionMessages.AgentControllerMapping:
                    return new AgentControllerMapping(reader);
                // RENDER
                case Messages.RenderMessages.LightStateChanged:
                    return new LightStateChanged(reader);
                // SIMULATION
                case Messages.SimulationMessages.InitialTimestamp:
                    return new InitialTimestamp(reader);
                case Messages.SimulationMessages.Timestamp:
                    return new SanProtocol.Simulation.Timestamp(reader);
                case Messages.SimulationMessages.SetWorldGravityMagnitude:
                    return new SetWorldGravityMagnitude(reader);
                case Messages.SimulationMessages.ActiveRigidBodyUpdate:
                    return new ActiveRigidBodyUpdate(reader);
                case Messages.SimulationMessages.RigidBodyDeactivated:
                    return new RigidBodyDeactivated(reader);
                case Messages.SimulationMessages.RigidBodyPropertyChanged:
                    return new RigidBodyPropertyChanged(reader);
                case Messages.SimulationMessages.RigidBodyDestroyed:
                    return new RigidBodyDestroyed(reader);
                //WORLDSTATE
                case Messages.WorldStateMessages.CreateWorld:
                    return new CreateWorld(reader);
                case Messages.WorldStateMessages.DestroyWorld:
                    return new DestroyWorld(reader);
                case Messages.WorldStateMessages.RigidBodyComponentInitialState:
                    return new RigidBodyComponentInitialState(reader);
                case Messages.WorldStateMessages.AnimationComponentInitialState:
                    return new AnimationComponentInitialState(reader);
                case Messages.WorldStateMessages.LoadClusterDefinition:
                    return new LoadClusterDefinition(reader);
                case Messages.WorldStateMessages.ComponentRelativeTransform:
                    return new ComponentRelativeTransform(reader);
                case Messages.WorldStateMessages.InitiateCluster:
                    return new InitiateCluster(reader);
                case Messages.WorldStateMessages.CreateClusterViaDefinition:
                    return new CreateClusterViaDefinition(reader);
                case Messages.WorldStateMessages.DestroyCluster:
                    return new DestroyCluster(reader);
                case Messages.WorldStateMessages.DestroyObject:
                    return new DestroyObject(reader);
                case Messages.WorldStateMessages.DestroySourceIdSpace:
                    return new DestroySourceIdSpace(reader);
                case Messages.WorldStateMessages.CreateCharacterNode:
                    return new CreateCharacterNode(reader);
                case Messages.WorldStateMessages.CreateAgentController:
                    return new CreateAgentController(reader);
                case Messages.WorldStateMessages.DestroyAgentController:
                    return new DestroyAgentController(reader);
                // ClientKafkaMessages
                case Messages.ClientKafkaMessages.RegionChat:
                    return new RegionChat(reader);
                case Messages.ClientKafkaMessages.Login:
                    return new SanProtocol.ClientKafka.Login(reader);
                case Messages.ClientKafkaMessages.LoginReply:
                    return new SanProtocol.ClientKafka.LoginReply(reader);
                case Messages.ClientKafkaMessages.EnterRegion:
                    return new EnterRegion(reader);
                case Messages.ClientKafkaMessages.LeaveRegion:
                    return new LeaveRegion(reader);
                case Messages.ClientKafkaMessages.PrivateChat:
                    return new PrivateChat(reader);
                case Messages.ClientKafkaMessages.PrivateChatStatus:
                    return new PrivateChatStatus(reader);
                case Messages.ClientKafkaMessages.PresenceUpdate:
                    return new PresenceUpdate(reader);
                case Messages.ClientKafkaMessages.FriendRequest:
                    return new FriendRequest(reader);
                case Messages.ClientKafkaMessages.FriendRequestStatus:
                    return new FriendRequestStatus(reader);
                case Messages.ClientKafkaMessages.FriendResponse:
                    return new FriendResponse(reader);
                case Messages.ClientKafkaMessages.FriendResponseStatus:
                    return new FriendResponseStatus(reader);
                case Messages.ClientKafkaMessages.FriendTable:
                    return new FriendTable(reader);
                case Messages.ClientKafkaMessages.RelationshipOperation:
                    return new RelationshipOperation(reader);
                case Messages.ClientKafkaMessages.RelationshipTable:
                    return new RelationshipTable(reader);
                case Messages.ClientKafkaMessages.InventoryItemCapabilities:
                    return new InventoryItemCapabilities(reader);
                case Messages.ClientKafkaMessages.InventoryItemRevision:
                    return new InventoryItemRevision(reader);
                case Messages.ClientKafkaMessages.InventoryItemUpdate:
                    return new InventoryItemUpdate(reader);
                case Messages.ClientKafkaMessages.InventoryItemDelete:
                    return new InventoryItemDelete(reader);
                case Messages.ClientKafkaMessages.InventoryLoaded:
                    return new InventoryLoaded(reader);
                case Messages.ClientKafkaMessages.FriendRequestLoaded:
                    return new FriendRequestLoaded(reader);
                case Messages.ClientKafkaMessages.FriendResponseLoaded:
                    return new FriendResponseLoaded(reader);
                case Messages.ClientKafkaMessages.PresenceUpdateFanoutLoaded:
                    return new PresenceUpdateFanoutLoaded(reader);
                case Messages.ClientKafkaMessages.FriendTableLoaded:
                    return new FriendTableLoaded(reader);
                case Messages.ClientKafkaMessages.RelationshipTableLoaded:
                    return new RelationshipTableLoaded(reader);
                case Messages.ClientKafkaMessages.PrivateChatLoaded:
                    return new PrivateChatLoaded(reader);
                case Messages.ClientKafkaMessages.PrivateChatStatusLoaded:
                    return new PrivateChatStatusLoaded(reader);
                case Messages.ClientKafkaMessages.ScriptRegionConsoleLoaded:
                    return new ScriptRegionConsoleLoaded(reader);
                case Messages.ClientKafkaMessages.ClientMetric:
                    return new ClientMetric(reader);
                case Messages.ClientKafkaMessages.RegionHeartbeatMetric:
                    return new RegionHeartbeatMetric(reader);
                case Messages.ClientKafkaMessages.RegionEventMetric:
                    return new RegionEventMetric(reader);
                case Messages.ClientKafkaMessages.SubscribeScriptRegionConsole:
                    return new SubscribeScriptRegionConsole(reader);
                case Messages.ClientKafkaMessages.UnsubscribeScriptRegionConsole:
                    return new UnsubscribeScriptRegionConsole(reader);
                case Messages.ClientKafkaMessages.ScriptConsoleLog:
                    return new ScriptConsoleLog(reader);
                case Messages.ClientKafkaMessages.LongLivedNotification:
                    return new LongLivedNotification(reader);
                case Messages.ClientKafkaMessages.LongLivedNotificationDelete:
                    return new LongLivedNotificationDelete(reader);
                case Messages.ClientKafkaMessages.LongLivedNotificationsLoaded:
                    return new LongLivedNotificationsLoaded(reader);
                case Messages.ClientKafkaMessages.ShortLivedNotification:
                    return new ShortLivedNotification(reader);
                // ClientVoiceMessages
                case Messages.ClientVoiceMessages.Login:
                    return new SanProtocol.ClientVoice.Login(reader);
                case Messages.ClientVoiceMessages.LoginReply:
                    return new SanProtocol.ClientVoice.LoginReply(reader);
                case Messages.ClientVoiceMessages.AudioData:
                    return new AudioData(reader);
                case Messages.ClientVoiceMessages.SpeechGraphicsData:
                    return new SpeechGraphicsData(reader);
                case Messages.ClientVoiceMessages.LocalAudioData:
                    return new LocalAudioData(reader);
                case Messages.ClientVoiceMessages.LocalAudioStreamState:
                    return new LocalAudioStreamState(reader);
                case Messages.ClientVoiceMessages.LocalAudioPosition:
                    return new LocalAudioPosition(reader);
                case Messages.ClientVoiceMessages.LocalAudioMute:
                    return new LocalAudioMute(reader);
                case Messages.ClientVoiceMessages.LocalSetRegionBroadcasted:
                    return new LocalSetRegionBroadcasted(reader);
                case Messages.ClientVoiceMessages.LocalSetMuteAll:
                    return new LocalSetMuteAll(reader);
                case Messages.ClientVoiceMessages.GroupAudioData:
                    return new GroupAudioData(reader);
                case Messages.ClientVoiceMessages.LocalTextData:
                    return new LocalTextData(reader);
                case Messages.ClientVoiceMessages.MasterInstance:
                    return new MasterInstance(reader);
                case Messages.ClientVoiceMessages.VoiceModerationCommand:
                    return new VoiceModerationCommand(reader);
                case Messages.ClientVoiceMessages.VoiceModerationCommandResponse:
                    return new VoiceModerationCommandResponse(reader);
                case Messages.ClientVoiceMessages.VoiceNotification:
                    return new VoiceNotification(reader);

                default:
                    {
                        Console.WriteLine("NetworkReader: Unhandled Message " + id);
                        throw new Exception("NetworkReader: Unhandled Message " + id);
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

            lock (_accountConductorLock)
            {
                while (_accountConductor.Available > 0)
                {
                    var instream = _accountConductor.GetStream();
                    var numBytesRead = instream.Read(_pollBuffer, 0, _pollBuffer.Length);
                    _packetBuffer.AppendBytes(_pollBuffer, numBytesRead);
                }
            }

            if (_packetBuffer.Packets.Count > 0)
            {
                _packetBuffer.ProcessPacketQueue();
                return true;
            }

            return false;
        }

        private readonly object _availablePacketsLock = new();
        private List<IPacket> _availablePackets { get; set; } = new List<IPacket>();

        private void ProcessPackets(List<IPacket> packets)
        {
            lock (_availablePacketsLock)
            {
                _availablePackets.AddRange(packets);
            }
        }

        public List<IPacket>? GetAvailablePackets()
        {
            lock (_availablePacketsLock)
            {
                if (_availablePackets.Count == 0)
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
