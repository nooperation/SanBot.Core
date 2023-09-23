using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Concentus.Structs;
using NAudio.Wave;
using SanBot.Database;
using SanProtocol;
using SanProtocol.AgentController;
using SanProtocol.ClientKafka;
using SanProtocol.ClientRegion;
using SanProtocol.ClientVoice;
using SanWebApi;
using SanWebApi.Json;
using static SanBot.Database.Services.PersonaService;

namespace SanBot.Core
{
    public class PersonaData
    {
        public uint SessionId { get; set; }
        public string UserName { get; set; } = default!;
        public string Handle { get; set; } = default!;
        public string AvatarType { get; set; } = default!;
        public SanUUID PersonaId { get; set; } = default!;
        public uint? AgentControllerId { get; set; }
        public ulong? AgentComponentId { get; set; }
        public uint? ClusterId { get; set; }
        public uint? CharacterObjectId { get; set; }

        public List<float> Position { get; set; } = new List<float>() { 0.0f, 0.0f, 0.0f };

        public SanProtocol.AnimationComponent.CharacterTransform? LastTransform { get; set; }
        public CharacterControllerInput? LastControllerInput { get; set; }
        public List<float>? LastVoicePosition { get; set; }
    }
    public struct RegionDetails
    {
        public string PersonaHandle { get; set; }
        public string SceneHandle { get; set; }

        public RegionDetails(string personaHandle, string sceneHandle)
        {
            PersonaHandle = personaHandle;
            SceneHandle = sceneHandle;
        }
    }

    public partial class Driver
    {
        public event EventHandler<string>? OnOutput;
        public Action<IPacket>? OnPacket;

        private SanBot.Database.PersonaDatabase Database { get; }

        public KafkaClient KafkaClient { get; set; }
        public RegionClient RegionClient { get; set; }
        public VoiceClient VoiceClient { get; set; }
        public WebApiClient WebApi { get; set; }

        public Dictionary<uint, PersonaData> PersonasBySessionId { get; set; } = new Dictionary<uint, PersonaData>();

        public PersonaPrivateResponse? MyPersonaDetails { get; private set; }
        public AccountConnectorSceneResult? RegionAccountConnectorResponse { get; set; }
        public UserInfoResponse.PayloadClass? MyUserInfo { get; internal set; } = new UserInfoResponse.PayloadClass();

        public SanUUID? CurrentInstanceId { get; set; }

        public PersonaData? MyPersonaData { get; set; }
        public VoiceAudioThread AudioThread { get; set; }
        public bool IsSpeaking => AudioThread.IsSpeaking;
        public uint? MySessionId { get; set; }

        public bool TryToAvoidInterruptingPeople { get; set; } = false;
        public bool UseVoice { get; set; } = true;
        public bool AutomaticallySendClientReady { get; set; } = true;
        public bool IgnoreRegionServer { get; set; }
        public RegionDetails? RegionToJoin { get; set; } = null;

        public Driver()
        {
            Database = new PersonaDatabase();

            KafkaClient = new KafkaClient(this);
            RegionClient = new RegionClient(this);
            VoiceClient = new VoiceClient(this);
            WebApi = new WebApiClient();

            WebApi.OnOutput += WebApi_OnOutput;
            KafkaClient.OnOutput += KafkaClient_OnOutput;
            RegionClient.OnOutput += RegionClient_OnOutput;
            VoiceClient.OnOutput += VoiceClient_OnOutput;

            KafkaClient.OnPacket = OnKafkaPacket;
            RegionClient.OnPacket = OnKafkaPacket;
            VoiceClient.OnPacket = OnKafkaPacket;

            AudioThread = new VoiceAudioThread(VoiceClient.SendRaw, TryToAvoidInterruptingPeople);
        }

        private void OnKafkaPacket(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.ClientKafkaMessages.LoginReply:
                    ClientKafkaMessages_OnLoginReply((SanProtocol.ClientKafka.LoginReply)packet);
                    break;
                case Messages.ClientKafkaMessages.PrivateChat:
                    ClientKafkaMessages_OnPrivateChat((SanProtocol.ClientKafka.PrivateChat)packet);
                    break;
                case Messages.ClientRegionMessages.UserLoginReply:
                    ClientRegionMessages_OnUserLoginReply((SanProtocol.ClientRegion.UserLoginReply)packet);
                    break;
                case Messages.ClientRegionMessages.AddUser:
                    ClientRegionMessages_OnAddUser((SanProtocol.ClientRegion.AddUser)packet);
                    break;
                case Messages.ClientRegionMessages.RemoveUser:
                    ClientRegionMessages_OnRemoveUser((SanProtocol.ClientRegion.RemoveUser)packet);
                    break;
                case Messages.ClientRegionMessages.SetAgentController:
                    ClientRegionMessages_OnSetAgentController((SanProtocol.ClientRegion.SetAgentController)packet);
                    break;
                case Messages.SimulationMessages.Timestamp:
                    SimulationMessages_OnTimestamp((SanProtocol.Simulation.Timestamp)packet);
                    break;
                case Messages.SimulationMessages.InitialTimestamp:
                    SimulationMessages_OnInitialTimestamp((SanProtocol.Simulation.InitialTimestamp)packet);
                    break;
                case Messages.AnimationComponentMessages.CharacterTransform:
                    AnimationComponentMessages_OnCharacterTransform((SanProtocol.AnimationComponent.CharacterTransform)packet);
                    break;
                case Messages.AnimationComponentMessages.CharacterTransformPersistent:
                    AnimationComponentMessages_OnCharacterTransformPersistent((SanProtocol.AnimationComponent.CharacterTransformPersistent)packet);
                    break;
                case Messages.AgentControllerMessages.CharacterControllerInputReliable:
                    AgentControllerMessages_OnCharacterControllerInputReliable((SanProtocol.AgentController.CharacterControllerInputReliable)packet);
                    break;
                case Messages.AgentControllerMessages.CharacterControllerInput:
                    AgentControllerMessages_OnCharacterControllerInput((SanProtocol.AgentController.CharacterControllerInput)packet);
                    break;
                case Messages.WorldStateMessages.CreateAgentController:
                    WorldStateMessages_OnCreateAgentController((SanProtocol.WorldState.CreateAgentController)packet);
                    break;
                case Messages.WorldStateMessages.DestroyAgentController:
                    WorldStateMessages_OnDestroyAgentController((SanProtocol.WorldState.DestroyAgentController)packet);
                    break;
                case Messages.WorldStateMessages.DestroyCluster:
                    WorldStateMessages_OnDestroyCluster((SanProtocol.WorldState.DestroyCluster)packet);
                    break;
                case Messages.WorldStateMessages.CreateClusterViaDefinition:
                    WorldStateMessages_OnCreateClusterViaDefinition((SanProtocol.WorldState.CreateClusterViaDefinition)packet);
                    break;
                case Messages.ClientVoiceMessages.LocalAudioData:
                    ClientVoiceMessages_OnLocalAudioData((SanProtocol.ClientVoice.LocalAudioData)packet);
                    break;
            }

            OnPacket?.Invoke(packet);
        }

        public bool HaveIBeenCreatedYet { get; set; }
        public Dictionary<uint, List<float>> InitialClusterPositions { get; set; } = new Dictionary<uint, List<float>>();

        private void WorldStateMessages_OnCreateClusterViaDefinition(SanProtocol.WorldState.CreateClusterViaDefinition e)
        {
            if (!HaveIBeenCreatedYet)
            {
                InitialClusterPositions[e.ClusterId] = e.SpawnPosition;
                return;
            }
        }

        #region voiceStuff
        private void ClientVoiceMessages_OnLocalAudioData(SanProtocol.ClientVoice.LocalAudioData e)
        {
            if (MyPersonaData == null || MyPersonaData.AgentControllerId == null)
            {
                return;
            }

            if (e.AgentControllerId == MyPersonaData.AgentControllerId)
            {
                return;
            }

            if (e.Data.Volume >= 400)
            {
                //Output($"Someone is speaking [{e.Data.Volume}]: " + e.AgentControllerId);
                AudioThread.LastTimeSomeoneSpoke = DateTime.Now;
            }
        }

        #endregion

        private void AgentControllerMessages_OnCharacterControllerInput(CharacterControllerInput e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentControllerId == e.AgentControllerId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                return;
            }

            personaData.LastControllerInput = e;
        }

        private void AgentControllerMessages_OnCharacterControllerInputReliable(CharacterControllerInputReliable e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentControllerId == e.AgentControllerId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                return;
            }

            personaData.LastControllerInput = e;
        }

        #region PersonaTracking
        private void WorldStateMessages_OnDestroyCluster(SanProtocol.WorldState.DestroyCluster e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.ClusterId == e.ClusterId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                return;
            }

            Output($"Destroyed player's cluster for {personaData.UserName} ({personaData.Handle})");
        }

        private void WorldStateMessages_OnCreateAgentController(SanProtocol.WorldState.CreateAgentController e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Key == e.SessionId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                Output($"Got CreateAgentController message for unknown session {e.SessionId}");
                return;
            }

            personaData.AgentControllerId = e.AgentControllerId;
            personaData.CharacterObjectId = e.CharacterObjectId;
            personaData.ClusterId = e.ClusterId;
            personaData.AgentComponentId = e.CharacterObjectId * 0x100000000ul;

            if (InitialClusterPositions.ContainsKey(e.ClusterId))
            {
                Output($"Found my initial cluster position for agent session {e.SessionId}");
                var initialPosition = InitialClusterPositions[e.ClusterId];
                personaData.Position[0] = initialPosition[0];
                personaData.Position[1] = initialPosition[1];
                personaData.Position[2] = initialPosition[2];
            }
        }

        private void WorldStateMessages_OnDestroyAgentController(SanProtocol.WorldState.DestroyAgentController e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentControllerId == e.AgentControllerId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                Output($"Got DestroyAgentController message for unknown AgentControllerId {e.AgentControllerId}");
                return;
            }

            Output($"Destroy agent controller for user {personaData.UserName} ({personaData.Handle})");
        }

        private void ClientRegionMessages_OnSetAgentController(SanProtocol.ClientRegion.SetAgentController e)
        {
            if (HaveIBeenCreatedYet)
            {
                Output("Got a secondary OnSetAgentController? this is new...");
                return;
            }

            var myPersonaData = PersonasBySessionId
                .Where(n => n.Value.SessionId == MySessionId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (myPersonaData == null)
            {
                Output($"SetAgentController: I got OnSetAgentController, but I can't find my Id in PersonasBySessionId? AgentController={e.AgentControllerId}");
                return;
            }

            myPersonaData.AgentControllerId = e.AgentControllerId;
            HaveIBeenCreatedYet = true;

            Output("Sending to voice server: LocalAudioStreamState(1)...");
            VoiceClient.SendPacket(new SanProtocol.ClientVoice.LocalAudioStreamState(VoiceClient.InstanceId, e.AgentControllerId, 1, 1));

            Output("Sending to voice server: LocalAudioPosition(0,0,0)...");
            SetVoicePosition(new List<float>() { 0, 0, 0 }, true);

            if (UseVoice)
            {
                AudioThread.Start();
            }
        }

        private void ClientRegionMessages_OnRemoveUser(SanProtocol.ClientRegion.RemoveUser e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.SessionId == e.SessionId)
                .Select(n => n.Value)
                .FirstOrDefault();

            if (personaData == null)
            {
                Output($"Unknown user has left (SessionId = {e.SessionId})");
                return;
            }

            Output($"{personaData.UserName} ({personaData.Handle}) Left the region");

            var unused = PersonasBySessionId.Remove(e.SessionId);
        }

        private void ClientRegionMessages_OnAddUser(SanProtocol.ClientRegion.AddUser e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.SessionId == e.SessionId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData != null)
            {
                if (e.SessionId != MySessionId)
                {
                    Output($"WARNING: Adding a new user who has the session ID of a previous user. Overwriting previous session data");
                }
                else
                {
                    personaData.SessionId = e.SessionId;
                    personaData.AvatarType = e.AvatarType;
                    personaData.Handle = e.Handle;
                    personaData.PersonaId = e.PersonaId;
                    personaData.UserName = e.UserName;
                    Output($"I have entered the region as session {e.SessionId}");
                    return;
                }
            }

            PersonasBySessionId[e.SessionId] = new PersonaData()
            {
                SessionId = e.SessionId,
                AvatarType = e.AvatarType,
                Handle = e.Handle,
                PersonaId = e.PersonaId,
                UserName = e.UserName,
                AgentComponentId = null,
                AgentControllerId = null,
                CharacterObjectId = null,
                ClusterId = null,
            };

            Output($"{e.UserName} ({e.Handle}) Entered the region as session {e.SessionId}");
        }

        private void AnimationComponentMessages_OnCharacterTransformPersistent(SanProtocol.AnimationComponent.CharacterTransformPersistent e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentComponentId == e.ComponentId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                return;
            }

            personaData.LastTransform = e;
            personaData.Position[0] = e.Position[0];
            personaData.Position[1] = e.Position[1];
            personaData.Position[2] = e.Position[2];
        }
        private void AnimationComponentMessages_OnCharacterTransform(SanProtocol.AnimationComponent.CharacterTransform e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentComponentId == e.ComponentId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData == null)
            {
                return;
            }

            personaData.LastTransform = e;
            personaData.Position[0] = e.Position[0];
            personaData.Position[1] = e.Position[1];
            personaData.Position[2] = e.Position[2];
        }
        #endregion


        #region Framerate
        public long LastTimestampTicks { get; set; } = 0;
        public ulong LastTimestampFrame { get; set; } = 0;
        public long InitialTimestamp { get; set; } = 0;

        public ulong GetCurrentFrame()
        {
            if (LastTimestampTicks == 0)
            {
                return LastTimestampFrame;
            }

            const float kFrameFrequency = 1000.0f / 90.0f;

            var millisecondsSinceLastTimestamp = (DateTime.Now.Ticks - LastTimestampTicks) / 10000;
            var totalFramesSinceLastTimestamp = millisecondsSinceLastTimestamp / kFrameFrequency;

            return LastTimestampFrame + (ulong)totalFramesSinceLastTimestamp;
        }

        private void SimulationMessages_OnInitialTimestamp(SanProtocol.Simulation.InitialTimestamp e)
        {
            Output($"InitialTimestamp {e.Frame} | {e.Nanoseconds}");

            LastTimestampFrame = e.Frame;
            LastTimestampTicks = DateTime.Now.Ticks;
            InitialTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private void SimulationMessages_OnTimestamp(SanProtocol.Simulation.Timestamp e)
        {
            //Output($"Server frame: {e.Frame} Client frame: {GetCurrentFrame()} | Diff={(long)e.Frame - (long)GetCurrentFrame()}");
            LastTimestampFrame = e.Frame;
            LastTimestampTicks = DateTime.Now.Ticks;
        }
        #endregion

        #region CommonNetworkActions
        public void SendChatMessage(string message)
        {
            if (KafkaClient != null)
            {
                RegionChat newChatPacket = new(
                    new SanUUID(),
                    new SanUUID(),
                    "",
                    0,
                    message,
                    0,
                    0,
                    0,
                    0
                );
                KafkaClient.SendPacket(newChatPacket);
            }
        }

        public void SendPrivateMessage(SanUUID other, string message)
        {
            if (MyPersonaDetails == null)
            {
                Output("Cannot send private message because we don't have our own persona details yet...");
                return;
            }

            PrivateChat privateMessagePacket = new(
                0,
                MyPersonaDetails.Id,
                other,
                message,
                0
            );
            KafkaClient.SendPacket(privateMessagePacket);
        }

        public void RequestSpawnItem(ulong frame, SanUUID itemClusterResourceId, List<float> spawnPosition, Quaternion spawnOrientation, uint agentControllerId)
        {
            RequestSpawnItem packet = new(
                frame,
                agentControllerId,
                itemClusterResourceId,
                255,
                spawnPosition,
                spawnOrientation
            );

            RegionClient.SendPacket(packet);
        }

        public void RequestPortalAt(string sansarUri, string sansarUriDescription)
        {
            RequestDropPortal packet = new(sansarUri, sansarUriDescription);
            RegionClient.SendPacket(packet);
        }

        public void SetPosition(List<float> position, Quaternion quat, ulong groundComponentId, bool isPersistent)
        {
            if (MyPersonaData == null || MyPersonaData.AgentComponentId == null || MyPersonaData.AgentControllerId == null)
            {
                return;
            }

            if (position.Count != 3)
            {
                throw new Exception($"{nameof(position)} Expected float3 position, got float{position.Count}");
            }

            if (isPersistent)
            {
                RegionClient.SendPacket(new SanProtocol.AnimationComponent.CharacterTransformPersistent(
                    MyPersonaData.AgentComponentId.Value,
                    GetCurrentFrame(),
                    groundComponentId,
                    new List<float>()
                    {
                      position[0],
                      position[1],
                      position[2],
                    },
                    quat
                ));
            }
            else
            {
                RegionClient.SendPacket(new SanProtocol.AnimationComponent.CharacterTransform(
                    MyPersonaData.AgentComponentId.Value,
                    GetCurrentFrame(),
                    groundComponentId,
                    new List<float>()
                    {
                        position[0],
                        position[1],
                        position[2],
                    },
                    quat
                ));
            }
        }

        public void SetVoicePosition(List<float> position, bool forceUpdate)
        {
            if (MyPersonaData == null || MyPersonaData.AgentControllerId == null)
            {
                return;
            }

            if (position.Count != 3)
            {
                throw new Exception($"{nameof(position)} Expected float3 position, got float{position.Count}");
            }

            if (MyPersonaData.LastVoicePosition == null || forceUpdate)
            {
                MyPersonaData.LastVoicePosition = position;
            }
            else
            {
                var distanceSinceFromLastVoicePosition =
                    Math.Sqrt(
                        Math.Pow(position[0] - MyPersonaData.LastVoicePosition[0], 2) +
                        Math.Pow(position[1] - MyPersonaData.LastVoicePosition[1], 2) +
                        Math.Pow(position[2] - MyPersonaData.LastVoicePosition[2], 2)
                    );

                if (distanceSinceFromLastVoicePosition <= 2)
                {
                    return;
                }
            }

            MyPersonaData.LastVoicePosition = position;
            VoiceClient.SendPacket(new SanProtocol.ClientVoice.LocalAudioPosition(
                VoiceClient.CurrentSequence++,
                VoiceClient.InstanceId,
                new List<float>()
                {
                    position[0],
                    position[1],
                    position[2],
                },
                MyPersonaData.AgentControllerId.Value
            ));
        }

        public void WarpToPosition(List<float> position3, List<float> rotation4)
        {
            if (MyPersonaData == null || MyPersonaData.AgentControllerId == null)
            {
                return;
            }

            if (position3.Count != 3)
            {
                throw new Exception($"{nameof(position3)} Expected float3 position, got float{position3.Count}");
            }
            if (rotation4.Count != 4)
            {
                throw new Exception($"{nameof(rotation4)} Expected float4 rotation, got float{rotation4.Count}");
            }

            RegionClient.SendPacket(new SanProtocol.AgentController.WarpCharacter(
                GetCurrentFrame(),
                MyPersonaData.AgentControllerId.Value,
                position3[0],
                position3[1],
                position3[2],
                rotation4[0],
                rotation4[1],
                rotation4[2],
                rotation4[3]
            ));
        }

        public static string Clusterbutt(string text)
        {
            text = text.Replace("-", "");
            var match = Regex.Match(text, @".*([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2}).*", RegexOptions.Singleline);
            if (match.Success)
            {
                StringBuilder sb = new();
                _ = sb.Append(match.Groups[1 + 7]);
                _ = sb.Append(match.Groups[1 + 6]);
                _ = sb.Append(match.Groups[1 + 5]);
                _ = sb.Append(match.Groups[1 + 4]);
                _ = sb.Append(match.Groups[1 + 3]);
                _ = sb.Append(match.Groups[1 + 2]);
                _ = sb.Append(match.Groups[1 + 1]);
                _ = sb.Append(match.Groups[1 + 0]);
                _ = sb.Append(match.Groups[1 + 8 + 7]);
                _ = sb.Append(match.Groups[1 + 8 + 6]);
                _ = sb.Append(match.Groups[1 + 8 + 5]);
                _ = sb.Append(match.Groups[1 + 8 + 4]);
                _ = sb.Append(match.Groups[1 + 8 + 3]);
                _ = sb.Append(match.Groups[1 + 8 + 2]);
                _ = sb.Append(match.Groups[1 + 8 + 1]);
                _ = sb.Append(match.Groups[1 + 8 + 0]);

                return sb.ToString();
            }
            else
            {
                return "ERROR";
            }
        }
        #endregion

        #region Database 
        public async Task<string?> GetPersonaName(SanUUID personaId)
        {
            var persona = await ResolvePersonaId(personaId);
            return persona != null ? $"{persona.Name} ({persona.Handle})" : personaId.Format();
        }

        public async Task<PersonaDto?> ResolvePersonaId(SanUUID personaId)
        {
            Guid personaGuid = new(personaId.Format());

            var persona = await Database.PersonaService.GetPersona(personaGuid);
            if (persona != null)
            {
                return persona;
            }

            var profiles = await WebApi.GetProfiles(new List<string>() {
                personaId.Format(),
            });

            PersonaDto? foundPersona = null;
            foreach (var item in profiles.Data)
            {
                if (new Guid(item.AvatarId) == personaGuid)
                {
                    foundPersona = new PersonaDto
                    {
                        Id = personaGuid,
                        Handle = item.AvatarHandle,
                        Name = item.AvatarName
                    };
                }

                await Database.PersonaService.UpdatePersonaAsync(new Guid(item.AvatarId), item.AvatarHandle, item.AvatarName);
            }

            return foundPersona;
        }
        #endregion


        private void VoiceClient_OnOutput(object? sender, string message)
        {
            OnOutput?.Invoke(sender, message);
        }
        private void RegionClient_OnOutput(object? sender, string message)
        {
            OnOutput?.Invoke(sender, message);
        }
        private void KafkaClient_OnOutput(object? sender, string message)
        {
            OnOutput?.Invoke(sender, message);
        }
        private void WebApi_OnOutput(object? sender, string message)
        {
            OnOutput?.Invoke(sender, message);
        }

        public void Output(string str)
        {
            OnOutput?.Invoke(this, str);
        }

        public static string GetSanbotConfigPath()
        {
            return Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SanBot"
            );
        }

        public async Task StartAsync(SecureString username, SecureString password)
        {
            await WebApi.Login(username, password);

            Output("Getting TOS status...");
            var tosStatus = await WebApi.RequestTos();
            Output($"  Signed TOS = {tosStatus.Payload?.SignedTos}");
            Output($"OK");

            Output("Getting user info...");
            var userInfoResult = await WebApi.RequestUserInfo();
            MyUserInfo = userInfoResult.Payload;
            Output($"  AccountId = {MyUserInfo.AccountId}");
            Output("OK");

            Output("Getting personas...");
            var personas = await WebApi.RequestPersonaByAccount(MyUserInfo.AccountId);
            foreach (var item in personas.Payload)
            {
                Output($"  {item.Id} | {item.Handle} | {item.Name}");

                if (item.IsDefault)
                {
                    MyPersonaDetails = WebApi.GetPersonaPrivate(item.Id).Result;
                }
            }
            if (MyPersonaDetails == null)
            {
                throw new Exception("Failed to find a default persona");
            }
            Output("OK");

            Output("Posting to account connector...");
            var accountConnectorResponse = WebApi.GetAccountConnectorAsync().Result;
            Output("OK");

            Output("Driver intialized. Starting KafkaClient");
            KafkaClient.Start(
                accountConnectorResponse.ConnectorResponse.Host,
                accountConnectorResponse.ConnectorResponse.TcpPort,
                accountConnectorResponse.ConnectorResponse.Secret
            );
        }

        public async Task JoinRegion(string personaHandle, string sceneHandle)
        {
            RegionAccountConnectorResponse = await WebApi.GetAccountConnectorSceneAsync(personaHandle, sceneHandle);
            CurrentInstanceId = new SanUUID(RegionAccountConnectorResponse.SceneUri[(1 + RegionAccountConnectorResponse.SceneUri.LastIndexOf('/'))..]);

            var regionAddress = CurrentInstanceId.Format();
            KafkaClient.SendPacket(new SanProtocol.ClientKafka.EnterRegion(
                regionAddress
            ));

            if (!IgnoreRegionServer)
            {
                Output("Starting region client");
                RegionClient.Start(
                    RegionAccountConnectorResponse.RegionResponse.Host,
                    RegionAccountConnectorResponse.RegionResponse.TcpPort,
                    RegionAccountConnectorResponse.RegionResponse.Secret
                );
            }

            Output("Starting voice client");
            VoiceClient.Start(
                RegionAccountConnectorResponse.VoiceResponse.Host,
                RegionAccountConnectorResponse.VoiceResponse.TcpPort,
                RegionAccountConnectorResponse.VoiceResponse.Secret,
                CurrentInstanceId
            );
        }

        private void ClientRegionMessages_OnUserLoginReply(SanProtocol.ClientRegion.UserLoginReply e)
        {
            if (!e.Success)
            {
                throw new Exception("Failed to enter region");
            }
            if (CurrentInstanceId == null)
            {
                throw new Exception($"{nameof(ClientRegionMessages_OnUserLoginReply)} - {nameof(CurrentInstanceId)} is null");
            }

            Output("Logged into region: " + e.ToString());

            if (!AutomaticallySendClientReady)
            {
                return;
            }

            MySessionId = e.SessionId;
            Output("My session ID is " + e.SessionId);
            if (PersonasBySessionId.ContainsKey(e.SessionId))
            {
                Output("*** Oh no, we were assigned session ID " + e.SessionId + ", but session ID " + e.SessionId + " already belongs to: " + PersonasBySessionId[e.SessionId].UserName ?? "UNKNOWN");
            }
            MyPersonaData = new PersonaData()
            {
                SessionId = e.SessionId,
            };
            PersonasBySessionId[e.SessionId] = MyPersonaData;

            var regionAddress = CurrentInstanceId.Format();
            KafkaClient.SendPacket(new SanProtocol.ClientKafka.EnterRegion(
                regionAddress
            ));

            RegionClient.SendPacket(new SanProtocol.ClientRegion.ClientDynamicReady(
                new List<float>() { 0, 0, 0 },
                new List<float>() { 0, 0, 0, 0 },
                new SanUUID(MyPersonaDetails!.Id),
                "",
                0,
                1
            ));

            RegionClient.SendPacket(new SanProtocol.ClientRegion.ClientStaticReady(
                1
            ));
        }

        public void Speak(byte[] rawPcmBytes)
        {
            const int kFrameSize = 960;
            const int kFrequency = 48000;

            if (CurrentInstanceId == null)
            {
                throw new Exception($"{nameof(Speak)} - {nameof(CurrentInstanceId)} is null");
            }
            if (MyPersonaData?.AgentControllerId == null)
            {
                throw new Exception($"{nameof(Speak)} - {nameof(MyPersonaData.AgentControllerId)} is null");
            }

            var pcmSamples = new short[rawPcmBytes.Length / 2];
            Buffer.BlockCopy(rawPcmBytes, 0, pcmSamples, 0, rawPcmBytes.Length);

            OpusEncoder encoder = OpusEncoder.Create(kFrequency, 1, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP);

            var totalFrames = pcmSamples.Length / 960;

            List<byte[]> messages = new();
            for (var i = 0; i < totalFrames; i++)
            {
                var compressedBytes = new byte[1276];
                var written = encoder.Encode(pcmSamples, kFrameSize * i, kFrameSize, compressedBytes, 0, compressedBytes.Length);

                var packetBytes = new SanProtocol.ClientVoice.LocalAudioData(
                    CurrentInstanceId,
                    MyPersonaData.AgentControllerId.Value,
                    new AudioData(VoiceClient.CurrentSequence, 1000, compressedBytes.Take(written).ToArray()),
                    new SpeechGraphicsData(VoiceClient.CurrentSequence, new byte[] { }),
                    0
                ).GetBytes();
                VoiceClient.CurrentSequence++;

                messages.Add(packetBytes);
            }

            AudioThread.EnqueueData(messages);
        }

        private void ClientKafkaMessages_OnPrivateChat(PrivateChat e)
        {
            Output($"(PRIVMSG) {e.FromPersonaId}: {e.Message}");
        }

        private void ClientKafkaMessages_OnLoginReply(SanProtocol.ClientKafka.LoginReply e)
        {
            if (!e.Success)
            {
                throw new Exception($"KafkaClient failed to login: {e.Message}");
            }

            Output("Kafka client logged in successfully");

            if (RegionToJoin != null)
            {
                JoinRegion(RegionToJoin.Value.PersonaHandle, RegionToJoin.Value.SceneHandle).Wait();
            }
        }

        public void Disconnect()
        {
            RegionClient.Disconnect();
            VoiceClient.Disconnect();
            KafkaClient.Disconnect();
        }

        public bool Poll()
        {
            var handledData = false;

            if (KafkaClient != null)
            {
                var clientHadData = true;

                while (clientHadData)
                {
                    clientHadData = KafkaClient.Poll();
                    handledData |= clientHadData;
                }
            }
            if (RegionClient != null)
            {
                var clientHadData = true;

                while (clientHadData)
                {
                    clientHadData = RegionClient.Poll();
                    handledData |= clientHadData;
                }
            }
            if (VoiceClient != null)
            {
                var clientHadData = true;

                while (clientHadData)
                {
                    clientHadData = VoiceClient.Poll();
                    handledData |= clientHadData;
                }
            }

            return handledData;
        }
    }
}
