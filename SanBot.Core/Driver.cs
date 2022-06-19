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

namespace SanBot.Core
{
    public class Driver
    {
        public KafkaClient KafkaClient { get; set; }
        public RegionClient RegionClient { get; set; }
        public VoiceClient VoiceClient { get; set; }
        public WebApi WebApi { get; set; }

        public PersonaPrivateResponse? MyPersonaDetails { get; private set; }
        public AccountConnectorSceneResult? RegionAccountConnectorResponse { get; set; }
        public UserInfoResponse.PayloadClass? MyUserInfo { get; internal set; } = new UserInfoResponse.PayloadClass();

        public SanUUID? CurrentInstanceId { get; set; }

        public Driver()
        {
            KafkaClient = new KafkaClient(this);
            RegionClient = new RegionClient(this);
            VoiceClient = new VoiceClient(this);
            WebApi = new WebApi();

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

        internal void SendPrivateMessage(SanUUID other, string message)
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

        private static string Clusterbutt(string text)
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

        public void Output(string str)
        {
            Console.WriteLine(str);
        }

        public async Task StartAsync(ConfigFile config)
        {
            await WebApi.Login(config.Username, config.Password);

            var tosStatus = await WebApi.RequestTos();
            Output($"Signed TOS = {tosStatus.Payload?.SignedTos}");

            var userInfoResult = await WebApi.RequestUserInfo();
            MyUserInfo = userInfoResult.Payload;
            Output($"AccountId = {MyUserInfo.AccountId}");

            var personas = await WebApi.RequestPersonaByAccount(MyUserInfo.AccountId);
            Output("Personas:");
            foreach (var item in personas.Payload)
            {
                Output($"{item.Id} | {item.Handle} | {item.Name}");

                if (item.IsDefault)
                {
                    this.MyPersonaDetails = WebApi.GetPersonaPrivate(item.Id).Result;
                    break;
                }
            }

            var accountConnectorResponse = WebApi.GetAccountConnector().Result;

            KafkaClient.Start(
                accountConnectorResponse.ConnectorResponse.Host,
                accountConnectorResponse.ConnectorResponse.TcpPort,
                accountConnectorResponse.ConnectorResponse.Secret
            );
        }

        public async Task JoinRegion(string personaHandle, string sceneHandle)
        {
            RegionAccountConnectorResponse = await WebApi.GetAccountConnectorScene(personaHandle, sceneHandle);
            CurrentInstanceId = new SanUUID(RegionAccountConnectorResponse.SceneUri.Substring(1 + RegionAccountConnectorResponse.SceneUri.LastIndexOf('/')));

            Output("Starting region thread");
            RegionClient.Start(
                RegionAccountConnectorResponse.RegionResponse.Host,
                RegionAccountConnectorResponse.RegionResponse.UdpPort,
                RegionAccountConnectorResponse.RegionResponse.Secret
            );
        }

        private void ClientKafkaMessages_OnLoginReply(object? sender, LoginReply e)
        {
            Output("Kafka logged in!");
        }
        
        private void ClientRegionMessages_OnUserLoginReply(object? sender, SanProtocol.ClientRegion.UserLoginReply e)
        {
            Output("Logged into region");
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
