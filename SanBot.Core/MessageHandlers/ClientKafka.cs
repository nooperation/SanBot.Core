using SanWebApi.Json;
using SanProtocol;
using SanProtocol.ClientKafka;
using SanWebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SanBot.Core.MessageHandlers
{
    public class ClientKafka : IMessageHandler
    {
        public event EventHandler<FriendResponseLoaded>? OnFriendResponseLoaded;
        public event EventHandler<PresenceUpdateFanoutLoaded>? OnPresenceUpdateFanoutLoaded;
        public event EventHandler<FriendTableLoaded>? OnFriendTableLoaded;
        public event EventHandler<RelationshipTableLoaded>? OnRelationshipTableLoaded;
        public event EventHandler<PrivateChatLoaded>? OnPrivateChatLoaded;
        public event EventHandler<PrivateChatStatusLoaded>? OnPrivateChatStatusLoaded;
        public event EventHandler<ScriptRegionConsoleLoaded>? OnScriptRegionConsoleLoaded;
        public event EventHandler<ClientMetric>? OnClientMetric;
        public event EventHandler<RegionHeartbeatMetric>? OnRegionHeartbeatMetric;
        public event EventHandler<RegionEventMetric>? OnRegionEventMetric;
        public event EventHandler<SubscribeScriptRegionConsole>? OnSubscribeScriptRegionConsole;
        public event EventHandler<UnsubscribeScriptRegionConsole>? OnUnsubscribeScriptRegionConsole;
        public event EventHandler<ScriptConsoleLog>? OnScriptConsoleLog;
        public event EventHandler<LongLivedNotification>? OnLongLivedNotification;
        public event EventHandler<LongLivedNotificationDelete>? OnLongLivedNotificationDelete;
        public event EventHandler<LongLivedNotificationsLoaded>? OnLongLivedNotificationsLoaded;
        public event EventHandler<ShortLivedNotification>? OnShortLivedNotification;
        public event EventHandler<Login>? OnLogin;
        public event EventHandler<LoginReply>? OnLoginReply;
        public event EventHandler<EnterRegion>? OnEnterRegion;
        public event EventHandler<LeaveRegion>? OnLeaveRegion;
        public event EventHandler<RegionChat>? OnRegionChat;
        public event EventHandler<PrivateChat>? OnPrivateChat;
        public event EventHandler<PrivateChatStatus>? OnPrivateChatStatus;
        public event EventHandler<PresenceUpdate>? OnPresenceUpdate;
        public event EventHandler<FriendRequest>? OnFriendRequest;
        public event EventHandler<FriendRequestStatus>? OnFriendRequestStatus;
        public event EventHandler<FriendResponse>? OnFriendResponse;
        public event EventHandler<FriendResponseStatus>? OnFriendResponseStatus;
        public event EventHandler<FriendTable>? OnFriendTable;
        public event EventHandler<RelationshipOperation>? OnRelationshipOperation;
        public event EventHandler<RelationshipTable>? OnRelationshipTable;
        public event EventHandler<InventoryItemCapabilities>? OnInventoryItemCapabilities;
        public event EventHandler<InventoryItemRevision>? OnInventoryItemRevision;
        public event EventHandler<InventoryItemUpdate>? OnInventoryItemUpdate;
        public event EventHandler<InventoryItemDelete>? OnInventoryItemDelete;
        public event EventHandler<InventoryLoaded>? OnInventoryLoaded;
        public event EventHandler<FriendRequestLoaded>? OnFriendRequestLoaded;

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.ClientKafka.RegionChat:
                {
                    this.HandleRegionChat(reader);
                    break;
                }
                case Messages.ClientKafka.Login:
                {
                    this.HandleLogin(reader);
                    break;
                }
                case Messages.ClientKafka.LoginReply:
                {
                    this.HandleLoginReply(reader);
                    break;
                }
                case Messages.ClientKafka.EnterRegion:
                {
                    this.HandleEnterRegion(reader);
                    break;
                }
                case Messages.ClientKafka.LeaveRegion:
                {
                    this.HandleLeaveRegion(reader);
                    break;
                }
                case Messages.ClientKafka.PrivateChat:
                {
                    this.HandlePrivateChat(reader);
                    break;
                }
                case Messages.ClientKafka.PrivateChatStatus:
                {
                    this.HandlePrivateChatStatus(reader);
                    break;
                }
                case Messages.ClientKafka.PresenceUpdate:
                {
                    this.HandlePresenceUpdate(reader);
                    break;
                }
                case Messages.ClientKafka.FriendRequest:
                {
                    this.HandleFriendRequest(reader);
                    break;
                }
                case Messages.ClientKafka.FriendRequestStatus:
                {
                    this.HandleFriendRequestStatus(reader);
                    break;
                }
                case Messages.ClientKafka.FriendResponse:
                {
                    this.HandleFriendResponse(reader);
                    break;
                }
                case Messages.ClientKafka.FriendResponseStatus:
                {
                    this.HandleFriendResponseStatus(reader);
                    break;
                }
                case Messages.ClientKafka.FriendTable:
                {
                    this.HandleFriendTable(reader);
                    break;
                }
                case Messages.ClientKafka.RelationshipOperation:
                {
                    this.HandleRelationshipOperation(reader);
                    break;
                }
                case Messages.ClientKafka.RelationshipTable:
                {
                    this.HandleRelationshipTable(reader);
                    break;
                }
                case Messages.ClientKafka.InventoryItemCapabilities:
                {
                    this.HandleInventoryItemCapabilities(reader);
                    break;
                }
                case Messages.ClientKafka.InventoryItemRevision:
                {
                    this.HandleInventoryItemRevision(reader);
                    break;
                }
                case Messages.ClientKafka.InventoryItemUpdate:
                {
                    this.HandleInventoryItemUpdate(reader);
                    break;
                }
                case Messages.ClientKafka.InventoryItemDelete:
                {
                    this.HandleInventoryItemDelete(reader);
                    break;
                }
                case Messages.ClientKafka.InventoryLoaded:
                {
                    this.HandleInventoryLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.FriendRequestLoaded:
                {
                    this.HandleFriendRequestLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.FriendResponseLoaded:
                {
                    this.HandleFriendResponseLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.PresenceUpdateFanoutLoaded:
                {
                    this.HandlePresenceUpdateFanoutLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.FriendTableLoaded:
                {
                    this.HandleFriendTableLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.RelationshipTableLoaded:
                {
                    this.HandleRelationshipTableLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.PrivateChatLoaded:
                {
                    this.HandlePrivateChatLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.PrivateChatStatusLoaded:
                {
                    this.HandlePrivateChatStatusLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.ScriptRegionConsoleLoaded:
                {
                    this.HandleScriptRegionConsoleLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.ClientMetric:
                {
                    this.HandleClientMetric(reader);
                    break;
                }
                case Messages.ClientKafka.RegionHeartbeatMetric:
                {
                    this.HandleRegionHeartbeatMetric(reader);
                    break;
                }
                case Messages.ClientKafka.RegionEventMetric:
                {
                    this.HandleRegionEventMetric(reader);
                    break;
                }
                case Messages.ClientKafka.SubscribeScriptRegionConsole:
                {
                    this.HandleSubscribeScriptRegionConsole(reader);
                    break;
                }
                case Messages.ClientKafka.UnsubscribeScriptRegionConsole:
                {
                    this.HandleUnsubscribeScriptRegionConsole(reader);
                    break;
                }
                case Messages.ClientKafka.ScriptConsoleLog:
                {
                    this.HandleScriptConsoleLog(reader);
                    break;
                }
                case Messages.ClientKafka.LongLivedNotification:
                {
                    this.HandleLongLivedNotification(reader);
                    break;
                }
                case Messages.ClientKafka.LongLivedNotificationDelete:
                {
                    this.HandleLongLivedNotificationDelete(reader);
                    break;
                }
                case Messages.ClientKafka.LongLivedNotificationsLoaded:
                {
                    this.HandleLongLivedNotificationsLoaded(reader);
                    break;
                }
                case Messages.ClientKafka.ShortLivedNotification:
                {
                    this.HandleShortLivedNotification(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }



        // TODO: get rid of this
        //Thread voiceClientStarterThread = null;
        //Thread regionClientStarterThread = null;
        //long loginTime = long.MaxValue;

        //static void PopulateAvatars(string path, Dictionary<string, string> avatars)
        //{
        //    var lines = File.ReadAllLines(path);

        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            continue;
        //        }

        //        var parts = lines[i].Split(',');
        //        var username = parts[1].Substring(1, parts[1].Length - 2);
        //        if (username == "Video1 - Sansar")
        //        {
        //            continue;
        //        }

        //        var assetId = parts[4].Substring(1, parts[4].Length - 2);

        //        avatars[assetId] = username;
        //    }
        //}

        //private static string Clusterbutt(string text)
        //{
        //    text = text.Replace("-", "");
        //    var match = Regex.Match(text, @".*([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{2}).*", RegexOptions.Singleline);
        //    if (match.Success)
        //    {
        //        var sb = new StringBuilder();
        //        sb.Append(match.Groups[1 + 7]);
        //        sb.Append(match.Groups[1 + 6]);
        //        sb.Append(match.Groups[1 + 5]);
        //        sb.Append(match.Groups[1 + 4]);
        //        sb.Append(match.Groups[1 + 3]);
        //        sb.Append(match.Groups[1 + 2]);
        //        sb.Append(match.Groups[1 + 1]);
        //        sb.Append(match.Groups[1 + 0]);
        //        sb.Append(match.Groups[1 + 8 + 7]);
        //        sb.Append(match.Groups[1 + 8 + 6]);
        //        sb.Append(match.Groups[1 + 8 + 5]);
        //        sb.Append(match.Groups[1 + 8 + 4]);
        //        sb.Append(match.Groups[1 + 8 + 3]);
        //        sb.Append(match.Groups[1 + 8 + 2]);
        //        sb.Append(match.Groups[1 + 8 + 1]);
        //        sb.Append(match.Groups[1 + 8 + 0]);

        //        return sb.ToString();
        //    }
        //    else
        //    {
        //        return "ERROR";
        //    }
        //}
        void HandleScriptConsoleLog(BinaryReader reader)
        {
            var packet = new ScriptConsoleLog(reader);
            OnScriptConsoleLog?.Invoke(this, packet);
        }

        void HandleLoginReply(BinaryReader reader)
        {
            //loginTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var packet = new LoginReply(reader);
            OnLoginReply?.Invoke(this, packet);

            // if(packet.Success)
            // {
            //    LoggedIn = true;
            //     TimeStarted = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // lol
            /*
             //Client.Instance.SendCatFact(new SanUUID("9f06bc78-6eef-40da-862a-18d3e03f1714"));
             //Client.Instance.SendCatFact(new SanUUID("1c3aad2b-0258-4c90-a604-0da35a9743f9")); // nop
             //Client.Instance.SendCatFact(new SanUUID("1a9f7777-8172-46a7-989c-1d8059118bb9")); // Terje-9294
             //Client.Instance.SendCatFact(new SanUUID("89b307d6-53c3-4711-ac65-7216e3af4a4c")); // zerocheese-4281
             //Client.Instance.SendCatFact(new SanUUID("9c2e15d1-3e1e-4872-8797-efca0261edb9")); // smurfbox
            */
            //// Client.Instance.SendCatFact(new SanUUID("08b6b976-760e-4280-90a9-d80b5fdc6390")); // lacie-sansar

            //Dictionary<string, string> avatars = new Dictionary<string, string>();
            //PopulateAvatars(@"r:\dec\new_sansar_dec\userdump_2020-09-30.csv", avatars);
            //PopulateAvatars(@"r:\dec\new_sansar_dec\userdump-2021-12-12.csv", avatars);
            //
            //var assetIds = avatars.Select(k => Clusterbutt(k.Key)).ToList();
            //assetIds.Shuffle();

            //int maxBots = 1;
            //int currentAvatarAssetIdIndex = 0;

            //for (int i = 0; i < maxBots; i++)
            //{
            //  Backend.Instance.SetAvatarId(Backend.Instance.MyPersona.Id, "9ccb5474edb7bce353dc52c6db4ff955").Wait();
            //  var result = Backend.Instance.GetAccountConnectorScene("sansar-studios", "nexus").Result;
            //var result = Backend.Instance.GetAccountConnectorScene("nop", "flat").Result;

            //https://atlas.sansar.com/experiences/1234-5678/familiar-room
            // var result = Backend.Instance.GetAccountConnectorScene("nop", "flat2").Result;
            // var result = Backend.Instance.GetAccountConnectorScene("1234-5678", "familiar-room").Result;


            /*
            voiceClientStarterThread = new Thread(new ThreadStart(() =>
            {
                Output("Starting voice thread");
                var client = new Client(this.Client.Instance, ClientType.Voice, result.VoiceResponse.Host, result.VoiceResponse.UdpPort, result.VoiceResponse.Secret, result.SceneUri);
                client.Start();
                while (true)
                {
                    if(!client.Poll())
                    {
                        Thread.Sleep(100);
                    }
                }

                //Client.Instance.VoiceClient = client;
            }));
            */


            //regionClientStarterThread = new Thread(new ThreadStart(() =>
            //{
            //    Output("Starting region thread");
            //    var client = new Client(this.Client.Instance, ClientType.Region, result.RegionResponse.Host, result.RegionResponse.UdpPort, result.RegionResponse.Secret, result.SceneUri);
            //    client.BotIndex = i;
            //    client.MaxBots = maxBots;

            //    //Client.Instance.RegionClient = client;
            //    client.Start();
            //    while(true)
            //    {
            //        if (!client.Poll())
            //        {
            //            Thread.Sleep(100);
            //        }
            //    }
            //}));

            // voiceClientStarterThread.Start();
            //regionClientStarterThread.Start();
            //  }

            // }
        }

        void HandlePresenceUpdate(BinaryReader reader)
        {
            var packet = new PresenceUpdate(reader);
            OnPresenceUpdate?.Invoke(this, packet);
            //var sourceName = Client.Instance.GetPersonaName(packet.PersonaId);
            //Output($"{sourceName} -> {packet.State} {packet.SansarUri}");
        }

        void HandleRelationshipTable(BinaryReader reader)
        {
            var packet = new RelationshipTable(reader);
            OnRelationshipTable?.Invoke(this, packet);

            //if(!RelationshipTableLoaded)
            //{
            //    return;
            //}

            //// 0 = friend request / friend accept
            //// 1 = ignore / remove friend
            //// 2 = block
            //// 3 = unblock ... remove friend??
            //if(packet.FromOther == 1 && packet.Status == 0)
            //{
            //    var acceptRequestPacket = new RelationshipOperation(packet.Other, 0);
            //    Client.Instance.KafkaClient.SendPacket(acceptRequestPacket);

            //   // Client.Instance.SendPrivateMessage(packet.Other, "Thank you for subscribing to cat facts!");
            //   // Client.Instance.SendCatFact(packet.Other);
            //}
            //if (packet.FromOther == 1 && packet.Status == 3) // lol refriend
            //{
            //    //var acceptRequestPacket = new RelationshipOperation(packet.Other, 0);
            //    //Client.Instance.KafkaClient.SendPacket(acceptRequestPacket);
            //}
        }

        void HandleInventoryItemUpdate(BinaryReader reader)
        {
            var packet = new InventoryItemUpdate(reader);
            OnInventoryItemUpdate?.Invoke(this, packet);
        }

        void HandleRegionChat(BinaryReader reader)
        {
            var packet = new RegionChat(reader);
            OnRegionChat?.Invoke(this, packet);


            // if(packet.Timestamp < TimeStarted)
            // {
            //     return;
            // }

            // if(packet.Message == "")
            // {
            //     return;
            // }

            // if(packet.Message == "!catfact")
            // {
            //     var newFact = Client.Instance.GetCatFact().Result;
            //     Client.Instance.SendChatMessage(newFact);
            // }

            // var personaData = Client.Instance.ResolvePersonaId(packet.FromPersonaId).Result;
            // var personaName = "";
            // if(personaData.Valid)
            // {
            //     personaName = $"{personaData.Name} ({personaData.Handle})";
            // }
            // else
            // {
            //     personaName = packet.FromPersonaId.Format();
            // }

            // var logLine = $"[{DateTime.Now}] {personaName}: {packet.Message}";
            // Output(logLine);

            // try
            // {
            //     //File.AppendAllText("u:\\sanbot_chat.log", $"{logLine}\r\n");
            // }
            // catch (Exception)
            // {
            //     Thread.Sleep(200);
            //     try
            //     {
            //        // File.AppendAllText("u:\\sanbot_chat.log", $"{logLine}\r\n");
            //     }
            //     catch (Exception)
            //     {
            //         Random rand = new Random();
            //        // File.AppendAllText($"u:\\sanbot_chat_{rand.Next().ToString()}.log",$"{logLine}\r\n");
            //     }
            // }

            //// if (packet.Message.Trim().ToLower() == "Hello" || packet.Message.Trim().ToLower() == "hi")
            //// {
            //    // Client.Instance.SendChatMessage($"Hi {personaData.Name}!");
            //// }

        }

        //public bool LoggedIn { get; set; } = false;
        //Regex patternJoin = new Regex("join (?<personaHandle>[a-zA-Z0-9\\-_]+) (?<sceneHandle>.*)", RegexOptions.IgnoreCase);
        //Regex patternSnoop = new Regex("snoop (?<personaHandle>[a-zA-Z0-9\\-_]+) (?<sceneHandle>.*)", RegexOptions.IgnoreCase);
        //Regex patternMessage = new Regex("msg (?<personaHandle>[a-zA-Z0-9\\-_]+) (?<message>.*)", RegexOptions.IgnoreCase);

        void HandlePrivateChat(BinaryReader reader)
        {
            var packet = new PrivateChat(reader);
            OnPrivateChat?.Invoke(this, packet);

            //if (packet.Timestamp > loginTime)
            //{
            //    var personaData = Client.Instance.ResolvePersonaId(packet.FromPersonaId).Result;
            //    var personaName = "";
            //    if (personaData.Valid)
            //    {
            //        personaName = $"{personaData.Name} ({personaData.Handle})";
            //    }
            //    else
            //    {
            //        personaName = packet.FromPersonaId.Format();
            //    }

            //    var logFormat = $"{{PM}} [{DateTime.Now}] {personaName}: {packet.Message}";
            //    Output(logFormat);

            //    try
            //    {
            //     //   File.AppendAllText("u:\\sanbot.log", $"{logFormat}\r\n");
            //    }
            //    catch (Exception)
            //    {
            //        Thread.Sleep(200);
            //        try
            //        {
            //       //     File.AppendAllText("u:\\sanbot.log", $"{logFormat}\r\n");
            //        }
            //        catch (Exception)
            //        {
            //            Random rand = new Random();
            //        //    File.AppendAllText($"u:\\sanbot_{rand.Next().ToString()}.log", $"{logFormat}\r\n");
            //        }
            //    }

            //    if (packet.Message.StartsWith("msg "))
            //    {
            //        var match = patternMessage.Match(packet.Message);
            //        if (!match.Success)
            //        {
            //            return;
            //        }

            //        var personaHandle = match.Groups["personaHandle"].Value;
            //        var message = match.Groups["message"].Value;
            //        var user = Client.Instance.ResolvePersonaHandle(personaHandle).Result;

            //        if (!user.Valid)
            //        {
            //            Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Unknown user");
            //            return;
            //        }

            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //        Client.Instance.SendPrivateMessage(user.Id, message);
            //    }
            //    else if (packet.Message == "!catfact")
            //    {
            //        var newFact = Client.Instance.GetCatFact().Result;
            //        Client.Instance.SendChatMessage(newFact);
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message == "tptest")
            //    {
            //        // Teleport myself somewhere
            //        var newPacket = new SanProtocol.AgentController.WarpCharacter(0, Client.Instance.MyAgentControllerId.Value, 5, 5, 5, 0, 0, 0, 0);
            //        Client.Instance.RegionClient.SendPacket(newPacket);
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message == "quit!")
            //    {
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //        Environment.Exit(0);
            //        return;
            //    }
            //    else if (packet.Message.StartsWith("echo "))
            //    {
            //        var message = packet.Message.Substring(5);
            //        Client.Instance.SendChatMessage(message);
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message == "init1")
            //    {
            //        var newPacket = new SanProtocol.ClientRegion.ClientDynamicReady(
            //            new List<float>() { 0, 0, 0 },
            //            new List<float>() { 0, 0, 0, 0 },
            //            new SanUUID(Client.Instance.PersonaDetails.Id), // todo: unnecessary
            //            "",
            //            0,
            //            1
            //        );
            //        Client.Instance.RegionClient.SendPacket(newPacket);
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message == "init2")
            //    {
            //        var newPacket = new SanProtocol.ClientRegion.ClientStaticReady(1);
            //        Client.Instance.RegionClient.SendPacket(newPacket);
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message == "leave")
            //    {
            //        var newPacket = new SanProtocol.ClientKafka.LeaveRegion(Client.Instance.RegionAddress);
            //        Client.Instance.KafkaClient.SendPacket(newPacket);
            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message.StartsWith("snoop_instance "))
            //    {
            //        var rawInstanceId = packet.Message.Substring(15).Trim();
            //        var instanceId = new SanUUID(rawInstanceId);

            //        var enterRegionPacket = new SanProtocol.ClientKafka.EnterRegion(instanceId.Format());
            //        Client.Instance.RegionAddress = instanceId.Format();
            //        Client.Instance.KafkaClient.SendPacket(enterRegionPacket);

            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message.StartsWith("snoop "))
            //    {
            //        var personaHandle = "1234-5678";
            //        var sceneHandle = "derp5";

            //        var match = patternSnoop.Match(packet.Message);
            //        if (match.Success)
            //        {
            //            personaHandle = match.Groups["personaHandle"].Value;
            //            sceneHandle = match.Groups["sceneHandle"].Value;
            //        }

            //        try
            //        {
            //            var result = Backend.Instance.GetAccountConnectorScene(personaHandle, sceneHandle).Result;
            //            var instanceId = new SanUUID(result.SceneUri.Substring(1 + result.SceneUri.LastIndexOf('/')));

            //            var enterRegionPacket = new SanProtocol.ClientKafka.EnterRegion(instanceId.Format());
            //            Client.Instance.RegionAddress = instanceId.Format();
            //            Client.Instance.KafkaClient.SendPacket(enterRegionPacket);
            //        }
            //        catch (Exception ex)
            //        {
            //            Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Failed: " + ex.Message);
            //            return;
            //        }

            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok");
            //    }
            //    else if (packet.Message.StartsWith("join"))
            //    {
            //        var personaHandle = "1234-5678";
            //        var sceneHandle = "derp5";

            //        var match = patternJoin.Match(packet.Message);
            //        if (match.Success)
            //        {
            //            personaHandle = match.Groups["personaHandle"].Value;
            //            sceneHandle = match.Groups["sceneHandle"].Value;
            //        }

            //        if (voiceClientStarterThread != null)
            //        {
            //            return;
            //        }
            //        if (regionClientStarterThread != null)
            //        {
            //            return;
            //        }

            //        var result = Backend.Instance.GetAccountConnectorScene(personaHandle, sceneHandle).Result;

            //        regionClientStarterThread = new Thread(new ThreadStart(() =>
            //        {
            //            Output("Starting region thread");
            //            var client = new Client(Client.Instance, Client.ClientType.Region, result.RegionResponse.Host, result.RegionResponse.TcpPort, result.RegionResponse.Secret, result.SceneUri);

            //            Client.Instance.RegionClient = client;
            //            client.Start();
            //        }));

            //        voiceClientStarterThread = new Thread(new ThreadStart(() =>
            //        {
            //            Output("Starting voice thread");
            //            var client = new Client(Client.Instance, Client.ClientType.Voice, result.VoiceResponse.Host, result.VoiceResponse.TcpPort, result.VoiceResponse.Secret, result.SceneUri);
            //            Client.Instance.VoiceClient = client;

            //            client.Start();
            //        }));

            //        voiceClientStarterThread.Start();
            //        regionClientStarterThread.Start();

            //        Client.Instance.SendPrivateMessage(packet.FromPersonaId, "Ok, joined");
            //    }
            //    else
            //    {
            //        if (packet.FromPersonaId.Format() != Client.Instance.PersonaDetails.Id)
            //        {
            //          //  Client.Instance.SendCatFact(packet.FromPersonaId);
            //        }
            //    }
            //}
        }

        void HandlePrivateChatStatus(BinaryReader reader)
        {
            var packet = new PrivateChatStatus(reader);
            OnPrivateChatStatus?.Invoke(this, packet);
        }

        void HandleShortLivedNotification(BinaryReader reader)
        {
            var packet = new ShortLivedNotification(reader);
            OnShortLivedNotification?.Invoke(this, packet);
        }

        void HandleLongLivedNotificationsLoaded(BinaryReader reader)
        {
            var packet = new LongLivedNotificationsLoaded(reader);
            OnLongLivedNotificationsLoaded?.Invoke(this, packet);
        }

        void HandleLongLivedNotificationDelete(BinaryReader reader)
        {
            var packet = new LongLivedNotificationDelete(reader);
            OnLongLivedNotificationDelete?.Invoke(this, packet);
        }

        void HandleLongLivedNotification(BinaryReader reader)
        {
            var packet = new LongLivedNotification(reader);
            OnLongLivedNotification?.Invoke(this, packet);
        }

        void HandleUnsubscribeScriptRegionConsole(BinaryReader reader)
        {
            var packet = new UnsubscribeScriptRegionConsole(reader);
            OnUnsubscribeScriptRegionConsole?.Invoke(this, packet);
        }

        void HandleSubscribeScriptRegionConsole(BinaryReader reader)
        {
            var packet = new SubscribeScriptRegionConsole(reader);
            OnSubscribeScriptRegionConsole?.Invoke(this, packet);
        }

        void HandleRegionEventMetric(BinaryReader reader)
        {
            var packet = new RegionEventMetric(reader);
            OnRegionEventMetric?.Invoke(this, packet);
        }

        void HandleRegionHeartbeatMetric(BinaryReader reader)
        {
            var packet = new RegionHeartbeatMetric(reader);
            OnRegionHeartbeatMetric?.Invoke(this, packet);
        }

        void HandleClientMetric(BinaryReader reader)
        {
            var packet = new ClientMetric(reader);
            OnClientMetric?.Invoke(this, packet);
        }

        void HandleScriptRegionConsoleLoaded(BinaryReader reader)
        {
            var packet = new ScriptRegionConsoleLoaded(reader);
            OnScriptRegionConsoleLoaded?.Invoke(this, packet);
        }

        void HandlePrivateChatStatusLoaded(BinaryReader reader)
        {
            var packet = new PrivateChatStatusLoaded(reader);
            OnPrivateChatStatusLoaded?.Invoke(this, packet);
        }

        void HandlePrivateChatLoaded(BinaryReader reader)
        {
            var packet = new PrivateChatLoaded(reader);
            OnPrivateChatLoaded?.Invoke(this, packet);
        }

        void HandleRelationshipTableLoaded(BinaryReader reader)
        {
            var packet = new RelationshipTableLoaded(reader);
            OnRelationshipTableLoaded?.Invoke(this, packet);
        }

        void HandleFriendTableLoaded(BinaryReader reader)
        {
            var packet = new FriendTableLoaded(reader);
            OnFriendTableLoaded?.Invoke(this, packet);
        }

        void HandlePresenceUpdateFanoutLoaded(BinaryReader reader)
        {
            var packet = new PresenceUpdateFanoutLoaded(reader);
            OnPresenceUpdateFanoutLoaded?.Invoke(this, packet);
        }

        void HandleFriendResponseLoaded(BinaryReader reader)
        {
            var packet = new FriendResponseLoaded(reader);
            OnFriendResponseLoaded?.Invoke(this, packet);
        }

        void HandleFriendRequestLoaded(BinaryReader reader)
        {
            var packet = new FriendRequestLoaded(reader);
            OnFriendRequestLoaded?.Invoke(this, packet);
        }

        void HandleInventoryLoaded(BinaryReader reader)
        {
            var packet = new InventoryLoaded(reader);
            OnInventoryLoaded?.Invoke(this, packet);
        }

        void HandleInventoryItemDelete(BinaryReader reader)
        {
            var packet = new InventoryItemDelete(reader);
            OnInventoryItemDelete?.Invoke(this, packet);
        }

        void HandleInventoryItemRevision(BinaryReader reader)
        {
            var packet = new InventoryItemRevision(reader);
            OnInventoryItemRevision?.Invoke(this, packet);
        }

        void HandleInventoryItemCapabilities(BinaryReader reader)
        {
            var packet = new InventoryItemCapabilities(reader);
            OnInventoryItemCapabilities?.Invoke(this, packet);
        }

        void HandleRelationshipOperation(BinaryReader reader)
        {
            var packet = new RelationshipOperation(reader);
            OnRelationshipOperation?.Invoke(this, packet);
        }

        void HandleFriendTable(BinaryReader reader)
        {
            var packet = new FriendTable(reader);
            OnFriendTable?.Invoke(this, packet);
        }

        void HandleFriendResponseStatus(BinaryReader reader)
        {
            var packet = new FriendResponseStatus(reader);
            OnFriendResponseStatus?.Invoke(this, packet);
        }

        void HandleFriendResponse(BinaryReader reader)
        {
            var packet = new FriendResponse(reader);
            OnFriendResponse?.Invoke(this, packet);
        }

        void HandleFriendRequestStatus(BinaryReader reader)
        {
            var packet = new FriendRequestStatus(reader);
            OnFriendRequestStatus?.Invoke(this, packet);
        }

        void HandleFriendRequest(BinaryReader reader)
        {
            var packet = new FriendRequest(reader);
            OnFriendRequest?.Invoke(this, packet);
        }

        void HandleLeaveRegion(BinaryReader reader)
        {
            var packet = new LeaveRegion(reader);
            OnLeaveRegion?.Invoke(this, packet);
        }

        void HandleEnterRegion(BinaryReader reader)
        {
            var packet = new EnterRegion(reader);
            OnEnterRegion?.Invoke(this, packet);
        }

        void HandleLogin(BinaryReader reader)
        {
            var packet = new Login(reader);
            OnLogin?.Invoke(this, packet);
        }
    }
}
