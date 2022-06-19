using SanProtocol;
using SanProtocol.ClientRegion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SanBot.Core.MessageHandlers
{
    public class ClientRegion : IMessageHandler
    {
        public event EventHandler<UserLogin>? OnUserLogin;
        public event EventHandler<UserLoginReply>? OnUserLoginReply;
        public event EventHandler<AddUser>? OnAddUser;
        public event EventHandler<RemoveUser>? OnRemoveUser;
        public event EventHandler<RenameUser>? OnRenameUser;
        public event EventHandler<ChatMessageToClient>? OnChatMessageToClient;
        public event EventHandler<ChatMessageToServer>? OnChatMessageToServer;
        public event EventHandler<SetAgentController>? OnSetAgentController;
        public event EventHandler<DebugTimeChangeToServer>? OnDebugTimeChangeToServer;
        public event EventHandler<VisualDebuggerCaptureToServer>? OnVisualDebuggerCaptureToServer;
        public event EventHandler<ClientStaticReady>? OnClientStaticReady;
        public event EventHandler<ClientDynamicReady>? OnClientDynamicReady;
        public event EventHandler<ClientRegionCommandMessage>? OnClientRegionCommandMessage;
        public event EventHandler<RequestDropPortal>? OnRequestDropPortal;
        public event EventHandler<VibrationPulseToClient>? OnVibrationPulseToClient;
        public event EventHandler<TeleportTo>? OnTeleportTo;
        public event EventHandler<TeleportToUri>? OnTeleportToUri;
        public event EventHandler<TeleportToEditMode>? OnTeleportToEditMode;
        public event EventHandler<DebugTimeChangeToClient>? OnDebugTimeChangeToClient;
        public event EventHandler<VisualDebuggerCaptureToClient>? OnVisualDebuggerCaptureToClient;
        public event EventHandler<ScriptModalDialog>? OnScriptModalDialog;
        public event EventHandler<ScriptModalDialogResponse>? OnScriptModalDialogResponse;
        public event EventHandler<TwitchEventSubscription>? OnTwitchEventSubscription;
        public event EventHandler<TwitchEvent>? OnTwitchEvent;
        public event EventHandler<InitialChunkSubscribed>? OnInitialChunkSubscribed;
        public event EventHandler<ClientKickNotification>? OnClientKickNotification;
        public event EventHandler<ClientSmiteNotification>? OnClientSmiteNotification;
        public event EventHandler<ClientMuteNotification>? OnClientMuteNotification;
        public event EventHandler<ClientVoiceBroadcastStartNotification>? OnClientVoiceBroadcastStartNotification;
        public event EventHandler<ClientVoiceBroadcastStopNotification>? OnClientVoiceBroadcastStopNotification;
        public event EventHandler<ClientRuntimeInventoryUpdatedNotification>? OnClientRuntimeInventoryUpdatedNotification;
        public event EventHandler<ClientSetRegionBroadcasted>? OnClientSetRegionBroadcasted;
        public event EventHandler<SubscribeCommand>? OnSubscribeCommand;
        public event EventHandler<UnsubscribeCommand>? OnUnsubscribeCommand;
        public event EventHandler<ClientCommand>? OnClientCommand;
        public event EventHandler<OpenStoreListing>? OnOpenStoreListing;
        public event EventHandler<OpenUserStore>? OnOpenUserStore;
        public event EventHandler<OpenQuestCharacterDialog>? OnOpenQuestCharacterDialog;
        public event EventHandler<UIScriptableBarStart>? OnUIScriptableBarStart;
        public event EventHandler<UIScriptableBarStopped>? OnUIScriptableBarStopped;
        public event EventHandler<UIScriptableBarCancel>? OnUIScriptableBarCancel;
        public event EventHandler<UIHintTextUpdate>? OnUIHintTextUpdate;
        public event EventHandler<QuestOfferResponse>? OnQuestOfferResponse;
        public event EventHandler<QuestCompleted>? OnQuestCompleted;
        public event EventHandler<QuestRemoved>? OnQuestRemoved;
        public event EventHandler<ShowWorldDetail>? OnShowWorldDetail;
        public event EventHandler<ShowTutorialHint>? OnShowTutorialHint;
        public event EventHandler<TutorialHintsSetEnabled>? OnTutorialHintsSetEnabled;
        public event EventHandler<ReactionDefinition>? OnReactionDefinition; // NEW: 2021-03-25
        public event EventHandler<SystemReactionDefinition>? OnSystemReactionDefinition; // NEW: 2021-03-25
        public event EventHandler<UpdateReactions>? OnUpdateReactions; // NEW: 2021-03-25
        public event EventHandler<AddReaction>? OnAddReaction; // NEW: 2021-03-25
        public event EventHandler<RemoveReaction>? OnRemoveReaction; // NEW: 2021-03-25

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.ClientRegion.UserLogin:
                {
                    this.HandleUserLogin(reader);
                    break;
                }
                case Messages.ClientRegion.UserLoginReply:
                {
                    this.HandleUserLoginReply(reader);
                    break;
                }
                case Messages.ClientRegion.AddUser:
                {
                    this.HandleAddUser(reader);
                    break;
                }
                case Messages.ClientRegion.RemoveUser:
                {
                    this.HandleRemoveUser(reader);
                    break;
                }
                case Messages.ClientRegion.RenameUser:
                {
                    this.HandleRenameUser(reader);
                    break;
                }
                case Messages.ClientRegion.ChatMessageToServer:
                {
                    this.HandleChatMessageToServer(reader);
                    break;
                }
                case Messages.ClientRegion.ChatMessageToClient:
                {
                    this.HandleChatMessageToClient(reader);
                    break;
                }
                case Messages.ClientRegion.VibrationPulseToClient:
                {
                    this.HandleVibrationPulseToClient(reader);
                    break;
                }
                case Messages.ClientRegion.SetAgentController:
                {
                    this.HandleSetAgentController(reader);
                    break;
                }
                case Messages.ClientRegion.TeleportTo:
                {
                    this.HandleTeleportTo(reader);
                    break;
                }
                case Messages.ClientRegion.TeleportToUri:
                {
                    this.HandleTeleportToUri(reader);
                    break;
                }
                case Messages.ClientRegion.TeleportToEditMode:
                {
                    this.HandleTeleportToEditMode(reader);
                    break;
                }
                case Messages.ClientRegion.DebugTimeChangeToServer:
                {
                    this.HandleDebugTimeChangeToServer(reader);
                    break;
                }
                case Messages.ClientRegion.DebugTimeChangeToClient:
                {
                    this.HandleDebugTimeChangeToClient(reader);
                    break;
                }
                case Messages.ClientRegion.VisualDebuggerCaptureToServer:
                {
                    this.HandleVisualDebuggerCaptureToServer(reader);
                    break;
                }
                case Messages.ClientRegion.VisualDebuggerCaptureToClient:
                {
                    this.HandleVisualDebuggerCaptureToClient(reader);
                    break;
                }
                case Messages.ClientRegion.ScriptModalDialog:
                {
                    this.HandleScriptModalDialog(reader);
                    break;
                }
                case Messages.ClientRegion.ScriptModalDialogResponse:
                {
                    this.HandleScriptModalDialogResponse(reader);
                    break;
                }
                case Messages.ClientRegion.TwitchEventSubscription:
                {
                    this.HandleTwitchEventSubscription(reader);
                    break;
                }
                case Messages.ClientRegion.TwitchEvent:
                {
                    this.HandleTwitchEvent(reader);
                    break;
                }
                case Messages.ClientRegion.ClientStaticReady:
                {
                    this.HandleClientStaticReady(reader);
                    break;
                }
                case Messages.ClientRegion.ClientDynamicReady:
                {
                    this.HandleClientDynamicReady(reader);
                    break;
                }
                case Messages.ClientRegion.InitialChunkSubscribed:
                {
                    this.HandleInitialChunkSubscribed(reader);
                    break;
                }
                case Messages.ClientRegion.ClientRegionCommandMessage:
                {
                    this.HandleClientRegionCommandMessage(reader);
                    break;
                }
                case Messages.ClientRegion.ClientKickNotification:
                {
                    this.HandleClientKickNotification(reader);
                    break;
                }
                case Messages.ClientRegion.ClientSmiteNotification:
                {
                    this.HandleClientSmiteNotification(reader);
                    break;
                }
                case Messages.ClientRegion.ClientMuteNotification:
                {
                    this.HandleClientMuteNotification(reader);
                    break;
                }
                case Messages.ClientRegion.ClientVoiceBroadcastStartNotification:
                {
                    this.HandleClientVoiceBroadcastStartNotification(reader);
                    break;
                }
                case Messages.ClientRegion.ClientVoiceBroadcastStopNotification:
                {
                    this.HandleClientVoiceBroadcastStopNotification(reader);
                    break;
                }
                case Messages.ClientRegion.ClientRuntimeInventoryUpdatedNotification:
                {
                    this.HandleClientRuntimeInventoryUpdatedNotification(reader);
                    break;
                }
                case Messages.ClientRegion.ClientSetRegionBroadcasted:
                {
                    this.HandleClientSetRegionBroadcasted(reader);
                    break;
                }
                case Messages.ClientRegion.SubscribeCommand:
                {
                    this.HandleSubscribeCommand(reader);
                    break;
                }
                case Messages.ClientRegion.UnsubscribeCommand:
                {
                    this.HandleUnsubscribeCommand(reader);
                    break;
                }
                case Messages.ClientRegion.ClientCommand:
                {
                    this.HandleClientCommand(reader);
                    break;
                }
                case Messages.ClientRegion.RequestDropPortal:
                {
                    this.HandleRequestDropPortal(reader);
                    break;
                }
                case Messages.ClientRegion.OpenStoreListing:
                {
                    this.HandleOpenStoreListing(reader);
                    break;
                }
                case Messages.ClientRegion.OpenUserStore:
                {
                    this.HandleOpenUserStore(reader);
                    break;
                }
                case Messages.ClientRegion.OpenQuestCharacterDialog:
                {
                    this.HandleOpenQuestCharacterDialog(reader);
                    break;
                }
                case Messages.ClientRegion.UIScriptableBarStart:
                {
                    this.HandleUIScriptableBarStart(reader);
                    break;
                }
                case Messages.ClientRegion.UIScriptableBarStopped:
                {
                    this.HandleUIScriptableBarStopped(reader);
                    break;
                }
                case Messages.ClientRegion.UIScriptableBarCancel:
                {
                    this.HandleUIScriptableBarCancel(reader);
                    break;
                }
                case Messages.ClientRegion.UIHintTextUpdate:
                {
                    this.HandleUIHintTextUpdate(reader);
                    break;
                }
                case Messages.ClientRegion.QuestOfferResponse:
                {
                    this.HandleQuestOfferResponse(reader);
                    break;
                }
                case Messages.ClientRegion.QuestCompleted:
                {
                    this.HandleQuestCompleted(reader);
                    break;
                }
                case Messages.ClientRegion.QuestRemoved:
                {
                    this.HandleQuestRemoved(reader);
                    break;
                }
                case Messages.ClientRegion.ShowWorldDetail:
                {
                    this.HandleShowWorldDetail(reader);
                    break;
                }
                case Messages.ClientRegion.ShowTutorialHint:
                {
                    this.HandleShowTutorialHint(reader);
                    break;
                }
                case Messages.ClientRegion.TutorialHintsSetEnabled:
                {
                    this.HandleTutorialHintsSetEnabled(reader);
                    break;
                }
                case Messages.ClientRegion.ReactionDefinition:
                {
                    this.HandleReactionDefinition(reader);
                    break;
                }
                case Messages.ClientRegion.SystemReactionDefinition:
                {
                    this.HandleSystemReactionDefinition(reader);
                    break;
                }
                case Messages.ClientRegion.UpdateReactions:
                {
                    this.HandleUpdateReactions(reader);
                    break;
                }
                case Messages.ClientRegion.AddReaction:
                {
                    this.HandleAddReaction(reader);
                    break;
                }
                case Messages.ClientRegion.RemoveReaction:
                {
                    this.HandleRemoveReaction(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleUserLogin(BinaryReader reader)
        {
            var packet = new UserLogin(reader);
            OnUserLogin?.Invoke(this, packet);
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

        public void Output(object obj)
        {
            Console.WriteLine(obj);
        }

        void HandleUserLoginReply(BinaryReader reader)
        {
            var packet = new UserLoginReply(reader);
            OnUserLoginReply?.Invoke(this, packet);
        }

        void HandleAddUser(BinaryReader reader)
        {
            var packet = new AddUser(reader);
            OnAddUser?.Invoke(this, packet);
        }

        void HandleRemoveUser(BinaryReader reader)
        {
            var packet = new RemoveUser(reader);
            OnRemoveUser?.Invoke(this, packet);
        }

        void HandleRenameUser(BinaryReader reader)
        {
            var packet = new RenameUser(reader);
            OnRenameUser?.Invoke(this, packet);
        }

        void HandleChatMessageToServer(BinaryReader reader)
        {
            var packet = new ChatMessageToServer(reader);
            OnChatMessageToServer?.Invoke(this, packet);
        }

        void HandleChatMessageToClient(BinaryReader reader)
        {
            var packet = new ChatMessageToClient(reader);
            OnChatMessageToClient?.Invoke(this, packet);
        }

        void HandleVibrationPulseToClient(BinaryReader reader)
        {
            var packet = new VibrationPulseToClient(reader);
            OnVibrationPulseToClient?.Invoke(this, packet);
        }

        void HandleSetAgentController(BinaryReader reader)
        {
            var packet = new SetAgentController(reader);
            OnSetAgentController?.Invoke(this, packet);
        }

        void HandleTeleportTo(BinaryReader reader)
        {
            var packet = new TeleportTo(reader);
            OnTeleportTo?.Invoke(this, packet);
        }

        void HandleTeleportToUri(BinaryReader reader)
        {
            var packet = new TeleportToUri(reader);
            OnTeleportToUri?.Invoke(this, packet);
        }

        void HandleTeleportToEditMode(BinaryReader reader)
        {
            var packet = new TeleportToEditMode(reader);
            OnTeleportToEditMode?.Invoke(this, packet);
        }

        void HandleDebugTimeChangeToServer(BinaryReader reader)
        {
            var packet = new DebugTimeChangeToServer(reader);
            OnDebugTimeChangeToServer?.Invoke(this, packet);
        }

        void HandleDebugTimeChangeToClient(BinaryReader reader)
        {
            var packet = new DebugTimeChangeToClient(reader);
            OnDebugTimeChangeToClient?.Invoke(this, packet);
        }

        void HandleVisualDebuggerCaptureToServer(BinaryReader reader)
        {
            var packet = new VisualDebuggerCaptureToServer(reader);
            OnVisualDebuggerCaptureToServer?.Invoke(this, packet);
        }

        void HandleVisualDebuggerCaptureToClient(BinaryReader reader)
        {
            var packet = new VisualDebuggerCaptureToClient(reader);
            OnVisualDebuggerCaptureToClient?.Invoke(this, packet);
        }

        void HandleScriptModalDialog(BinaryReader reader)
        {
            var packet = new ScriptModalDialog(reader);
            OnScriptModalDialog?.Invoke(this, packet);
        }

        void HandleScriptModalDialogResponse(BinaryReader reader)
        {
            var packet = new ScriptModalDialogResponse(reader);
            OnScriptModalDialogResponse?.Invoke(this, packet);
        }

        void HandleTwitchEventSubscription(BinaryReader reader)
        {
            var packet = new TwitchEventSubscription(reader);
            OnTwitchEventSubscription?.Invoke(this, packet);
        }

        void HandleTwitchEvent(BinaryReader reader)
        {
            var packet = new TwitchEvent(reader);
            OnTwitchEvent?.Invoke(this, packet);
        }

        void HandleClientStaticReady(BinaryReader reader)
        {
            var packet = new ClientStaticReady(reader);
            OnClientStaticReady?.Invoke(this, packet);
        }

        void HandleClientDynamicReady(BinaryReader reader)
        {
            var packet = new ClientDynamicReady(reader);
            OnClientDynamicReady?.Invoke(this, packet);
        }

        void HandleInitialChunkSubscribed(BinaryReader reader)
        {
            var packet = new InitialChunkSubscribed(reader);
            OnInitialChunkSubscribed?.Invoke(this, packet);
        }

        // whenever a user does %%command we send this and the voicemoderationcommand packets
        void HandleClientRegionCommandMessage(BinaryReader reader)
        {
            var packet = new ClientRegionCommandMessage(reader);
            OnClientRegionCommandMessage?.Invoke(this, packet);
        }

        void HandleClientKickNotification(BinaryReader reader)
        {
            var packet = new ClientKickNotification(reader);
            OnClientKickNotification?.Invoke(this, packet);
        }

        void HandleClientSmiteNotification(BinaryReader reader)
        {
            var packet = new ClientSmiteNotification(reader);
            OnClientSmiteNotification?.Invoke(this, packet);
        }

        void HandleClientMuteNotification(BinaryReader reader)
        {
            var packet = new ClientMuteNotification(reader);
            OnClientMuteNotification?.Invoke(this, packet);
        }

        void HandleClientVoiceBroadcastStartNotification(BinaryReader reader)
        {
            var packet = new ClientVoiceBroadcastStartNotification(reader);
            OnClientVoiceBroadcastStartNotification?.Invoke(this, packet);
        }

        void HandleClientVoiceBroadcastStopNotification(BinaryReader reader)
        {
            var packet = new ClientVoiceBroadcastStopNotification(reader);
            OnClientVoiceBroadcastStopNotification?.Invoke(this, packet);
        }

        // %%backpack-on  %%backpack-off  %%backpack-clear
        void HandleClientRuntimeInventoryUpdatedNotification(BinaryReader reader)
        {
            var packet = new ClientRuntimeInventoryUpdatedNotification(reader);
            OnClientRuntimeInventoryUpdatedNotification?.Invoke(this, packet);
        }

        void HandleClientSetRegionBroadcasted(BinaryReader reader)
        {
            var packet = new ClientSetRegionBroadcasted(reader);
            OnClientSetRegionBroadcasted?.Invoke(this, packet);
        }

        void HandleSubscribeCommand(BinaryReader reader)
        {
            var packet = new SubscribeCommand(reader);
            OnSubscribeCommand?.Invoke(this, packet);
        }

        void HandleUnsubscribeCommand(BinaryReader reader)
        {
            var packet = new UnsubscribeCommand(reader);
            OnUnsubscribeCommand?.Invoke(this, packet);
        }

        void HandleClientCommand(BinaryReader reader)
        {
            var packet = new ClientCommand(reader);
            OnClientCommand?.Invoke(this, packet);
        }

        void HandleRequestDropPortal(BinaryReader reader)
        {
            var packet = new RequestDropPortal(reader);
            OnRequestDropPortal?.Invoke(this, packet);
        }

        void HandleOpenStoreListing(BinaryReader reader)
        {
            var packet = new OpenStoreListing(reader);
            OnOpenStoreListing?.Invoke(this, packet);
        }

        void HandleOpenUserStore(BinaryReader reader)
        {
            var packet = new OpenUserStore(reader);
            OnOpenUserStore?.Invoke(this, packet);
        }

        void HandleOpenQuestCharacterDialog(BinaryReader reader)
        {
            var packet = new OpenQuestCharacterDialog(reader);
            OnOpenQuestCharacterDialog?.Invoke(this, packet);
        }

        void HandleUIScriptableBarStart(BinaryReader reader)
        {
            var packet = new UIScriptableBarStart(reader);
            OnUIScriptableBarStart?.Invoke(this, packet);
        }

        void HandleUIScriptableBarStopped(BinaryReader reader)
        {
            var packet = new UIScriptableBarStopped(reader);
            OnUIScriptableBarStopped?.Invoke(this, packet);
        }

        void HandleUIScriptableBarCancel(BinaryReader reader)
        {
            var packet = new UIScriptableBarCancel(reader);
            OnUIScriptableBarCancel?.Invoke(this, packet);
        }

        void HandleUIHintTextUpdate(BinaryReader reader)
        {
            var packet = new UIHintTextUpdate(reader);
            OnUIHintTextUpdate?.Invoke(this, packet);
        }

        void HandleQuestOfferResponse(BinaryReader reader)
        {
            var packet = new QuestOfferResponse(reader);
            OnQuestOfferResponse?.Invoke(this, packet);
        }

        void HandleQuestCompleted(BinaryReader reader)
        {
            var packet = new QuestCompleted(reader);
            OnQuestCompleted?.Invoke(this, packet);
        }

        void HandleQuestRemoved(BinaryReader reader)
        {
            var packet = new QuestRemoved(reader);
            OnQuestRemoved?.Invoke(this, packet);
        }

        void HandleShowWorldDetail(BinaryReader reader)
        {
            var packet = new ShowWorldDetail(reader);
            OnShowWorldDetail?.Invoke(this, packet);
        }

        void HandleShowTutorialHint(BinaryReader reader)
        {
            var packet = new ShowTutorialHint(reader);
            OnShowTutorialHint?.Invoke(this, packet);
        }

        void HandleTutorialHintsSetEnabled(BinaryReader reader)
        {
            var packet = new TutorialHintsSetEnabled(reader);
            OnTutorialHintsSetEnabled?.Invoke(this, packet);
        }

        private void HandleReactionDefinition(BinaryReader reader)
        {
            var packet = new ReactionDefinition(reader);
            OnReactionDefinition?.Invoke(this, packet);
        }

        private void HandleSystemReactionDefinition(BinaryReader reader)
        {
            var packet = new SystemReactionDefinition(reader);
            OnSystemReactionDefinition?.Invoke(this, packet);
        }

        private void HandleUpdateReactions(BinaryReader reader)
        {
            var packet = new UpdateReactions(reader);
            OnUpdateReactions?.Invoke(this, packet);
        }

        private void HandleAddReaction(BinaryReader reader)
        {
            var packet = new AddReaction(reader);
            OnAddReaction?.Invoke(this, packet);
        }

        private void HandleRemoveReaction(BinaryReader reader)
        {
            var packet = new RemoveReaction(reader);
            OnRemoveReaction?.Invoke(this, packet);
        }
    }
}
