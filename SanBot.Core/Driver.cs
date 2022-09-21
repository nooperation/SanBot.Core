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

namespace SanBot.Core
{
    public class PersonaData
    {
        public uint SessionId { get; set; }
        public string UserName { get; set; }
        public string Handle { get; set; }
        public string AvatarType { get; set; }
        public SanUUID PersonaId { get; set; }
        public uint? AgentControllerId { get; set; }
        public ulong? AgentComponentId { get; set; }
        public uint? ClusterId { get; set; }
        public uint? CharacterObjectId { get; set; }

        public SanProtocol.AnimationComponent.CharacterTransform? LastTransform { get; set; }
        public CharacterControllerInput? LastControllerInput { get; set; }
        public List<float>? LastVoicePosition { get; set; }
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
                .Where(n => n.Value.AgentControllerId == e.AgentControllerId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (myPersonaData == null)
            {
                Output($"SetAgentController: Attempted to set agent controller to an unknown controller {e.AgentControllerId}");
                return;
            }

            MyPersonaData = myPersonaData;
        }

        private void ClientRegionMessages_OnRemoveUser(object? sender, SanProtocol.ClientRegion.RemoveUser e)
        {
            var personaData = PersonasBySessionId
                .Where(n => n.Value.AgentControllerId == e.SessionId)
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
                .Where(n => n.Value.AgentControllerId == e.SessionId)
                .Select(n => n.Value)
                .FirstOrDefault();
            if (personaData != null)
            {
                Output($"WARNING: Adding a new user who has the session ID of a previous user. Overwriting previous session data");
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

        public async Task StartAsync(ConfigFile config)
        {
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


        private void ClientKafkaMessages_OnLoginReply(object? sender, LoginReply e)
        {
            //KafkaClient_OnOutput(sender, $"Got login reply: {e.Success}");
            //if(e.Message != "")
            //{
            //    KafkaClient_OnOutput(sender, $"  [{e.MessageId}] {e.Message}");
            //}
        }
        
        private void ClientRegionMessages_OnUserLoginReply(object? sender, SanProtocol.ClientRegion.UserLoginReply e)
        {
            //RegionClient_OnOutput(sender, $"Got login reply: {e.Success}");
            //if (e.MessageId != 0)
            //{
            //    RegionClient_OnOutput(sender, $"  MessageID {e.MessageId}");
            //}
            //RegionClient_OnOutput(sender, "Privileges:\n" + String.Join(Environment.NewLine, e.Privileges));
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
                handledData |= KafkaClient.Poll();
            }
            if (RegionClient != null)
            {
                handledData |= RegionClient.Poll();
            }
            if (VoiceClient != null)
            {
                handledData |= VoiceClient.Poll();
            }

            return handledData;
        }
    }
}
