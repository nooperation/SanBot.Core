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

namespace SanBot.Core
{
    public class Driver
    {
        public event EventHandler<string>? OnOutput;

        public KafkaClient KafkaClient { get; set; }
        public RegionClient RegionClient { get; set; }
        public VoiceClient VoiceClient { get; set; }
        public WebApiClient WebApi { get; set; }

        public PersonaPrivateResponse? MyPersonaDetails { get; private set; }
        public AccountConnectorSceneResult? RegionAccountConnectorResponse { get; set; }
        public UserInfoResponse.PayloadClass? MyUserInfo { get; internal set; } = new UserInfoResponse.PayloadClass();

        public SanUUID? CurrentInstanceId { get; set; }

        public Driver()
        {
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
        }

        private void ClientRegionMessages_OnAddUser(object? sender, SanProtocol.ClientRegion.AddUser e)
        {
        }

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
