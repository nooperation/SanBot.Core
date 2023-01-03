using SanWebApi.Json;
using SanProtocol;
using SanProtocol.ClientKafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SanWebApi;
using Newtonsoft.Json;
using SanProtocol.AgentController;
using SanProtocol.ClientRegion;
using static SanBot.Database.Services.PersonaService;
using SanBot.Database;
using Concentus.Structs;
using SanProtocol.ClientVoice;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using Google.Cloud.TextToSpeech.V1;
using NAudio.Wave;

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



    public class Driver
    {
        public event EventHandler<string>? OnOutput;

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

        public bool TryToAvoidInterruptingPeople { get; set; } = false;
        public bool UseVoice { get; set; } = true;
        public bool AutomaticallySendClientReady { get; set; } = true;
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

            KafkaClient.ClientKafkaMessages.OnLoginReply += ClientKafkaMessages_OnLoginReply;
            KafkaClient.ClientKafkaMessages.OnPrivateChat += ClientKafkaMessages_OnPrivateChat;

            RegionClient.ClientRegionMessages.OnUserLoginReply += ClientRegionMessages_OnUserLoginReply;
            RegionClient.ClientRegionMessages.OnAddUser += ClientRegionMessages_OnAddUser;
            RegionClient.ClientRegionMessages.OnRemoveUser += ClientRegionMessages_OnRemoveUser;
            RegionClient.ClientRegionMessages.OnSetAgentController += ClientRegionMessages_OnSetAgentController;

            RegionClient.SimulationMessages.OnTimestamp += SimulationMessages_OnTimestamp;
            RegionClient.SimulationMessages.OnInitialTimestamp += SimulationMessages_OnInitialTimestamp;

            RegionClient.AnimationComponentMessages.OnCharacterTransform += AnimationComponentMessages_OnCharacterTransform;
            RegionClient.AnimationComponentMessages.OnCharacterTransformPersistent += AnimationComponentMessages_OnCharacterTransformPersistent;
            RegionClient.AgentControllerMessages.OnCharacterControllerInputReliable += AgentControllerMessages_OnCharacterControllerInputReliable;
            RegionClient.AgentControllerMessages.OnCharacterControllerInput += AgentControllerMessages_OnCharacterControllerInput;

            RegionClient.WorldStateMessages.OnCreateAgentController += WorldStateMessages_OnCreateAgentController;
            RegionClient.WorldStateMessages.OnDestroyAgentController += WorldStateMessages_OnDestroyAgentController;
            RegionClient.WorldStateMessages.OnDestroyCluster += WorldStateMessages_OnDestroyCluster;
            RegionClient.WorldStateMessages.OnCreateClusterViaDefinition += WorldStateMessages_OnCreateClusterViaDefinition;

            VoiceClient.ClientVoiceMessages.OnLocalAudioData += ClientVoiceMessages_OnLocalAudioData;


            AudioThread = new VoiceAudioThread(VoiceClient.SendRaw, TryToAvoidInterruptingPeople);
        }


        public bool HaveIBeenCreatedYet { get; set; }
        public Dictionary<uint, List<float>> InitialClusterPositions { get; set; } = new Dictionary<uint, List<float>>();

        private void WorldStateMessages_OnCreateClusterViaDefinition(object? sender, SanProtocol.WorldState.CreateClusterViaDefinition e)
        {
            if (!HaveIBeenCreatedYet)
            {
                InitialClusterPositions[e.ClusterId] = e.SpawnPosition;
                return;
            }
        }

        private void ClientVoiceMessages_OnLocalAudioData(object? sender, SanProtocol.ClientVoice.LocalAudioData e)
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

        private void AgentControllerMessages_OnCharacterControllerInput(object? sender, CharacterControllerInput e)
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

        private void AgentControllerMessages_OnCharacterControllerInputReliable(object? sender, CharacterControllerInputReliable e)
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
        private void WorldStateMessages_OnDestroyCluster(object? sender, SanProtocol.WorldState.DestroyCluster e)
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

        private void WorldStateMessages_OnCreateAgentController(object? sender, SanProtocol.WorldState.CreateAgentController e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Key == e.SessionId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if(personaData == null)
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

        private void WorldStateMessages_OnDestroyAgentController(object? sender, SanProtocol.WorldState.DestroyAgentController e)
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

        private void ClientRegionMessages_OnSetAgentController(object? sender, SanProtocol.ClientRegion.SetAgentController e)
        {
            var myPersonaData = this.PersonasBySessionId
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

        private void ClientRegionMessages_OnRemoveUser(object? sender, SanProtocol.ClientRegion.RemoveUser e)
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

            PersonasBySessionId.Remove(e.SessionId);
        }

        private void ClientRegionMessages_OnAddUser(object? sender, SanProtocol.ClientRegion.AddUser e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.SessionId == e.SessionId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData != null)
            {
                if(e.SessionId != MySessionId)
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

        private void AnimationComponentMessages_OnCharacterTransformPersistent(object? sender, SanProtocol.AnimationComponent.CharacterTransformPersistent e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentComponentId == e.ComponentId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if(personaData == null)
            {
                return;
            }

            personaData.LastTransform = e;
            personaData.Position[0] = e.Position[0];
            personaData.Position[1] = e.Position[1];
            personaData.Position[2] = e.Position[2];
        }
        private void AnimationComponentMessages_OnCharacterTransform(object? sender, SanProtocol.AnimationComponent.CharacterTransform e)
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

            var millisecondsSinceLastTimestamp = ((DateTime.Now.Ticks - LastTimestampTicks)) / 10000;
            var totalFramesSinceLastTimestamp = millisecondsSinceLastTimestamp / kFrameFrequency;

            return LastTimestampFrame + (ulong)totalFramesSinceLastTimestamp;
        }

        private void SimulationMessages_OnInitialTimestamp(object? sender, SanProtocol.Simulation.InitialTimestamp e)
        {
            Output($"InitialTimestamp {e.Frame} | {e.Nanoseconds}");

            LastTimestampFrame = e.Frame;
            LastTimestampTicks = DateTime.Now.Ticks;
            InitialTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private void SimulationMessages_OnTimestamp(object? sender, SanProtocol.Simulation.Timestamp e)
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
                var newChatPacket = new RegionChat(
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
            if(MyPersonaDetails == null)
            {
                Output("Cannot send private message because we don't have our own persona details yet...");
                return;
            }

            var privateMessagePacket = new PrivateChat(
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
            var packet = new RequestSpawnItem(
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
            var packet = new RequestDropPortal(sansarUri, sansarUriDescription);
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
                var sb = new StringBuilder();
                sb.Append(match.Groups[1 + 7]);
                sb.Append(match.Groups[1 + 6]);
                sb.Append(match.Groups[1 + 5]);
                sb.Append(match.Groups[1 + 4]);
                sb.Append(match.Groups[1 + 3]);
                sb.Append(match.Groups[1 + 2]);
                sb.Append(match.Groups[1 + 1]);
                sb.Append(match.Groups[1 + 0]);
                sb.Append(match.Groups[1 + 8 + 7]);
                sb.Append(match.Groups[1 + 8 + 6]);
                sb.Append(match.Groups[1 + 8 + 5]);
                sb.Append(match.Groups[1 + 8 + 4]);
                sb.Append(match.Groups[1 + 8 + 3]);
                sb.Append(match.Groups[1 + 8 + 2]);
                sb.Append(match.Groups[1 + 8 + 1]);
                sb.Append(match.Groups[1 + 8 + 0]);

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
            if (persona != null)
            {
                return $"{persona.Name} ({persona.Handle})";
            }

            return personaId.Format();
        }

        public async Task<PersonaDto?> ResolvePersonaId(SanUUID personaId)
        {
            var personaGuid = new Guid(personaId.Format());

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



        public class GoogleConfigPayload
        {
            public string key { get; set; } = default!;
        }
        public class AzureConfigPayload
        {
            public string key1 { get; set; } = default!;
            public string region { get; set; } = default!;
        }
        public AzureConfigPayload? AzureConfig { get; set; }
        private GoogleConfigPayload? GoogleConfig { get; set; }

        public async Task StartAsync(ConfigFile config)
        {
            AzureConfig = null;
            GoogleConfig = null;

            try
            {
                var azureConfigPath = Path.Join(GetSanbotConfigPath(), "azure.json");
                var configFileContents = File.ReadAllText(azureConfigPath);
                var result = System.Text.Json.JsonSerializer.Deserialize<AzureConfigPayload>(configFileContents);
                if (result == null || result.key1.Length == 0 || result.region.Length == 0)
                {
                    throw new Exception("Invalid azure config");
                }

                AzureConfig = result;
            }
            catch (Exception ex)
            {
                throw new Exception("Missing or invalid azure config", ex);
            }

            await WebApi.Login(config.Username, config.Password);

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
                    this.MyPersonaDetails = WebApi.GetPersonaPrivate(item.Id).Result;
                }
            }
            if(this.MyPersonaDetails == null)
            {
                throw new Exception("Failed to find a default persona");
            }
            Output("OK");

            Output("Posting to account connector...");
            var accountConnectorResponse = WebApi.GetAccountConnectorAsync().Result;
            Output("OK");


         //   WebApi.SetAvatarIdAsync(MyPersonaDetails.Id, "43668ab727c00fd7d33a5af1085493dd").Wait();

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
            CurrentInstanceId = new SanUUID(RegionAccountConnectorResponse.SceneUri.Substring(1 + RegionAccountConnectorResponse.SceneUri.LastIndexOf('/')));

            var regionAddress = CurrentInstanceId!.Format();
            KafkaClient.SendPacket(new SanProtocol.ClientKafka.EnterRegion(
                regionAddress
            ));

            Output("Starting region client");
            RegionClient.Start(
                RegionAccountConnectorResponse.RegionResponse.Host,
                RegionAccountConnectorResponse.RegionResponse.UdpPort,
                RegionAccountConnectorResponse.RegionResponse.Secret
            );

            Output("Starting voice client");
            VoiceClient.Start(
                RegionAccountConnectorResponse.VoiceResponse.Host,
                RegionAccountConnectorResponse.VoiceResponse.UdpPort,
                RegionAccountConnectorResponse.VoiceResponse.Secret,
                CurrentInstanceId
            );
        }

        public uint? MySessionId { get; set; }
        private void ClientRegionMessages_OnUserLoginReply(object? sender, SanProtocol.ClientRegion.UserLoginReply e)
        {
            if (!e.Success)
            {
                throw new Exception("Failed to enter region");
            }

            Output("Logged into region: " + e.ToString());

            if (!AutomaticallySendClientReady)
            {
                return;
            }

            MySessionId = e.SessionId;
            Output("My session ID is " + e.SessionId);
            if(this.PersonasBySessionId.ContainsKey(e.SessionId))
            {
                Output("*** Oh no, we were assigned session ID " + e.SessionId + ", but session ID " + e.SessionId + " already belongs to: " + PersonasBySessionId[e.SessionId].UserName ?? "UNKNOWN");
            }
            MyPersonaData = new PersonaData()
            {
                SessionId = e.SessionId,
            };
            this.PersonasBySessionId[e.SessionId] = MyPersonaData;

            var regionAddress = CurrentInstanceId!.Format();
            KafkaClient.SendPacket(new SanProtocol.ClientKafka.EnterRegion(
                regionAddress
            ));

            RegionClient.SendPacket(new SanProtocol.ClientRegion.ClientDynamicReady(
                new List<float>() { 0, 0, 0 },
                new List<float>() { 1, 0, 0, 0 },
                new SanUUID(MyPersonaDetails!.Id),
                "",
                0,
                1
            ));

            RegionClient.SendPacket(new SanProtocol.ClientRegion.ClientStaticReady(
                1
            ));
        }

        public class AudioStreamHandler : PushAudioOutputStreamCallback
        {
            public List<byte[]> CollectedBytes { get; set; } = new List<byte[]>();

            public Driver Driver { get; set; }
            public AudioStreamHandler(Driver driver)
            {
                this.Driver = driver;
            }

            public override uint Write(byte[] dataBuffer)
            {
                Driver.Output($"Write() - Added {dataBuffer.Length} bytes to the buffer");
                CollectedBytes.Add(dataBuffer);

                return (uint)dataBuffer.Length;
            }

            public override void Close()
            {
                Driver.Output("Audio data is ready to be consumed");

                long totalSize = 0;
                foreach (var item in CollectedBytes)
                {
                    totalSize += item.Length;
                }

                var buffer = new byte[totalSize];
                var bufferOffset = 0;

                foreach (var item in CollectedBytes)
                {
                    item.CopyTo(buffer, bufferOffset);
                    bufferOffset += item.Length;
                }

                Driver.Speak(buffer);
                base.Close();
            }
        }

        static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
                default:
                    break;
            }
        }

        DateTime LastSpoke = DateTime.Now;
        public HashSet<string> PreviousMessages { get; set; } = new HashSet<string>();
        public string TextToSpeechVoice { get; set; } = $"<speak xmlns='http://www.w3.org/2001/10/synthesis' xmlns:mstts='http://www.w3.org/2001/mstts' xmlns:emo='http://www.w3.org/2009/10/emotionml' version='1.0' xml:lang='en-US'><voice name=\"en-US-JennyNeural\"><prosody volume='40'  rate=\'20%\' pitch=\'0%\'>#MESSAGE#</prosody></voice></speak>";
        public string GoogleTTSName { get; set; } = "";
        public float GoogleTTSRate { get; set; } = 1.0f;
        public float GoogleTTSPitch { get; set; } = 0;

        public void Speak(string message, bool allowRepeating = false)
        {
            if (message.Length >= 256)
            {
                Output($"Ignored, too long ${message.Length}");
                return;
            }

            //if ((DateTime.Now - LastSpoke).TotalSeconds <= 1)
            //{
            //    Output($"Ignored, only {(DateTime.Now - LastSpoke).TotalSeconds} since last speaking");
            //    return;
            //}

            if (!allowRepeating && PreviousMessages.Contains(message))
            {
                return;
            }
            PreviousMessages.Add(message);


            var client = TextToSpeechClient.Create();

            var input = new SynthesisInput
            {
                Text = message,
                
            };
            var voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                Name = GoogleTTSName,
                SsmlGender = SsmlVoiceGender.Neutral
            };
            var audioConfig = new Google.Cloud.TextToSpeech.V1.AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3,
                SampleRateHertz = 48000,
                SpeakingRate = GoogleTTSRate,
                Pitch = GoogleTTSPitch
            };
            var response = client.SynthesizeSpeech(input, voiceSelection, audioConfig);

            using (MemoryStream mp3Stream = new MemoryStream())
            {
                response.AudioContent.WriteTo(mp3Stream);
                mp3Stream.Position = 0;

                WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(mp3Stream));
                byte[] bytes = new byte[pcm.Length];
                pcm.Position = 0;
                pcm.Read(bytes, 0, (int)pcm.Length);
                //File.WriteAllBytes("Bot.pcm", bytes);
                Speak(bytes);
            }

            LastSpoke = DateTime.Now;
        }


        public void SpeakAzure(string message, bool allowRepeating = false)
        {
            if (message.Length >= 256)
            {
                Output($"Ignored, too long ${message.Length}");
                return;
            }

            //if ((DateTime.Now - LastSpoke).TotalSeconds <= 1)
            //{
            //    Output($"Ignored, only {(DateTime.Now - LastSpoke).TotalSeconds} since last speaking");
            //    return;
            //}

            if (!allowRepeating && PreviousMessages.Contains(message))
            {
                return;
            }
            PreviousMessages.Add(message);

            var speechConfig = SpeechConfig.FromSubscription(AzureConfig.key1, AzureConfig.region);
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw48Khz16BitMonoPcm);
            speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
            //speechConfig.SpeechSynthesisVoiceName = "en-US-AnaNeural";

            var audioCallbackHandler = new AudioStreamHandler(this);
            using (var audioConfig = Microsoft.CognitiveServices.Speech.Audio.AudioConfig.FromStreamOutput(audioCallbackHandler))
            {
                using (var speechSynthesizer = new SpeechSynthesizer(speechConfig, audioConfig))
                {
                    var ssml = TextToSpeechVoice.Replace("#MESSAGE#", message);
                    var speechSynthesisResult = speechSynthesizer.SpeakSsmlAsync(ssml).Result;
                    OutputSpeechSynthesisResult(speechSynthesisResult);
                }
            }

            LastSpoke = DateTime.Now;
        }

        public void Speak(byte[] rawPcmBytes)
        {
            const int kFrameSize = 960;
            const int kFrequency = 48000;

            var pcmSamples = new short[rawPcmBytes.Length / 2];
            Buffer.BlockCopy(rawPcmBytes, 0, pcmSamples, 0, rawPcmBytes.Length);

            OpusEncoder encoder = OpusEncoder.Create(kFrequency, 1, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP);

            var totalFrames = pcmSamples.Length / 960;

            List<byte[]> messages = new List<byte[]>();
            for (int i = 0; i < totalFrames; i++)
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

        private void ClientKafkaMessages_OnPrivateChat(object? sender, PrivateChat e)
        {
            Output($"(PRIVMSG) {e.FromPersonaId}: {e.Message}");
        }

        private void ClientKafkaMessages_OnLoginReply(object? sender, SanProtocol.ClientKafka.LoginReply e)
        {
            if (!e.Success)
            {
                throw new Exception($"KafkaClient failed to login: {e.Message}");
            }

            Output("Kafka client logged in successfully");
            //    https://atlas.sansar.com/experiences/mijekamunro/bingo-oracle
            // default bot
            // Driver.WebApi.SetAvatarIdAsync(Driver.MyPersonaDetails.Id, "43668ab727c00fd7d33a5af1085493dd").Wait();

            // Driver.JoinRegion("djm3n4c3-9174", "dj-s-outside-fun2").Wait();
            //   Driver.JoinRegion("sansar-studios", "social-hub").Wait();
            //  Driver.JoinRegion("sansar-studios", "social-hub").Wait();
            //  Driver.JoinRegion("lozhyde", "sxc").Wait();
            // Driver.JoinRegion("mijekamunro", "bingo-oracle").Wait();
            //  Driver.JoinRegion("nopnopnop", "owo").Wait();
            //Driver.JoinRegion("nop", "rce-poc").Wait();
            // Driver.JoinRegion("princesspea-0197", "wanderlust").Wait();
            if(RegionToJoin != null)
            {
                JoinRegion(RegionToJoin.Value.PersonaHandle, RegionToJoin.Value.SceneHandle).Wait();
            }
        }

        public void Disconnect()
        {
            RegionClient?.Disconnect();
            VoiceClient?.Disconnect();
            KafkaClient?.Disconnect();

            RegionClient = null;
            VoiceClient = null;
            KafkaClient = null;
        }

        public bool Poll()
        {
            bool handledData = false;

            if (KafkaClient != null)
            {
                var clientHadData = true;

                while(clientHadData)
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
