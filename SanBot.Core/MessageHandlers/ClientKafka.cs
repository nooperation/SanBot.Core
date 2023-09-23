using SanProtocol;
using SanProtocol.ClientKafka;

namespace SanBot.Core.MessageHandlers
{
    public class ClientKafka : IMessageHandler
    {
        public Action<IPacket>? OnPacket;

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            IPacket? newPacket = null;

            switch (messageId)
            {
                case Messages.ClientKafkaMessages.RegionChat:
                    {
                        newPacket = new RegionChat(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.Login:
                    {
                        newPacket = new Login(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.LoginReply:
                    {
                        newPacket = new LoginReply(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.EnterRegion:
                    {
                        newPacket = new EnterRegion(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.LeaveRegion:
                    {
                        newPacket = new LeaveRegion(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.PrivateChat:
                    {
                        newPacket = new PrivateChat(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.PrivateChatStatus:
                    {
                        newPacket = new PrivateChatStatus(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.PresenceUpdate:
                    {
                        newPacket = new PresenceUpdate(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendRequest:
                    {
                        newPacket = new FriendRequest(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendRequestStatus:
                    {
                        newPacket = new FriendRequestStatus(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendResponse:
                    {
                        newPacket = new FriendResponse(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendResponseStatus:
                    {
                        newPacket = new FriendResponseStatus(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendTable:
                    {
                        newPacket = new FriendTable(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.RelationshipOperation:
                    {
                        newPacket = new RelationshipOperation(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.RelationshipTable:
                    {
                        newPacket = new RelationshipTable(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.InventoryItemCapabilities:
                    {
                        newPacket = new InventoryItemCapabilities(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.InventoryItemRevision:
                    {
                        newPacket = new InventoryItemRevision(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.InventoryItemUpdate:
                    {
                        newPacket = new InventoryItemUpdate(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.InventoryItemDelete:
                    {
                        newPacket = new InventoryItemDelete(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.InventoryLoaded:
                    {
                        newPacket = new InventoryLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendRequestLoaded:
                    {
                        newPacket = new FriendRequestLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendResponseLoaded:
                    {
                        newPacket = new FriendResponseLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.PresenceUpdateFanoutLoaded:
                    {
                        newPacket = new PresenceUpdateFanoutLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.FriendTableLoaded:
                    {
                        newPacket = new FriendTableLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.RelationshipTableLoaded:
                    {
                        newPacket = new RelationshipTableLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.PrivateChatLoaded:
                    {
                        newPacket = new PrivateChatLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.PrivateChatStatusLoaded:
                    {
                        newPacket = new PrivateChatStatusLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.ScriptRegionConsoleLoaded:
                    {
                        newPacket = new ScriptRegionConsoleLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.ClientMetric:
                    {
                        newPacket = new ClientMetric(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.RegionHeartbeatMetric:
                    {
                        newPacket = new RegionHeartbeatMetric(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.RegionEventMetric:
                    {
                        newPacket = new RegionEventMetric(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.SubscribeScriptRegionConsole:
                    {
                        newPacket = new SubscribeScriptRegionConsole(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.UnsubscribeScriptRegionConsole:
                    {
                        newPacket = new UnsubscribeScriptRegionConsole(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.ScriptConsoleLog:
                    {
                        newPacket = new ScriptConsoleLog(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.LongLivedNotification:
                    {
                        newPacket = new LongLivedNotification(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.LongLivedNotificationDelete:
                    {
                        newPacket = new LongLivedNotificationDelete(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.LongLivedNotificationsLoaded:
                    {
                        newPacket = new LongLivedNotificationsLoaded(reader);
                        break;
                    }
                case Messages.ClientKafkaMessages.ShortLivedNotification:
                    {
                        newPacket = new ShortLivedNotification(reader);
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            OnPacket?.Invoke(newPacket);

            return true;
        }

        public bool OnMessage(IPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
