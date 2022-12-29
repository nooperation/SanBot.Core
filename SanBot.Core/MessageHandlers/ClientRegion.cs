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

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.ClientRegion.UserLogin:
                {
                    OnUserLogin?.Invoke(this, (UserLogin)packet);
                    break;
                }
                case Messages.ClientRegion.UserLoginReply:
                {
                    OnUserLoginReply?.Invoke(this, (UserLoginReply)packet);
                    break;
                }
                case Messages.ClientRegion.AddUser:
                {
                    OnAddUser?.Invoke(this, (AddUser)packet);
                    break;
                }
                case Messages.ClientRegion.RemoveUser:
                {
                    OnRemoveUser?.Invoke(this, (RemoveUser)packet);
                    break;
                }
                case Messages.ClientRegion.RenameUser:
                {
                    OnRenameUser?.Invoke(this, (RenameUser)packet);
                    break;
                }
                case Messages.ClientRegion.ChatMessageToServer:
                {
                    OnChatMessageToServer?.Invoke(this, (ChatMessageToServer)packet);
                    break;
                }
                case Messages.ClientRegion.ChatMessageToClient:
                {
                    OnChatMessageToClient?.Invoke(this, (ChatMessageToClient)packet);
                    break;
                }
                case Messages.ClientRegion.VibrationPulseToClient:
                {
                    OnVibrationPulseToClient?.Invoke(this, (VibrationPulseToClient)packet);
                    break;
                }
                case Messages.ClientRegion.SetAgentController:
                {
                    OnSetAgentController?.Invoke(this, (SetAgentController)packet);
                    break;
                }
                case Messages.ClientRegion.TeleportTo:
                {
                    OnTeleportTo?.Invoke(this, (TeleportTo)packet);
                    break;
                }
                case Messages.ClientRegion.TeleportToUri:
                {
                    OnTeleportToUri?.Invoke(this, (TeleportToUri)packet);
                    break;
                }
                case Messages.ClientRegion.TeleportToEditMode:
                {
                    OnTeleportToEditMode?.Invoke(this, (TeleportToEditMode)packet);
                    break;
                }
                case Messages.ClientRegion.DebugTimeChangeToServer:
                {
                    OnDebugTimeChangeToServer?.Invoke(this, (DebugTimeChangeToServer)packet);
                    break;
                }
                case Messages.ClientRegion.DebugTimeChangeToClient:
                {
                    OnDebugTimeChangeToClient?.Invoke(this, (DebugTimeChangeToClient)packet);
                    break;
                }
                case Messages.ClientRegion.VisualDebuggerCaptureToServer:
                {
                    OnVisualDebuggerCaptureToServer?.Invoke(this, (VisualDebuggerCaptureToServer)packet);
                    break;
                }
                case Messages.ClientRegion.VisualDebuggerCaptureToClient:
                {
                    OnVisualDebuggerCaptureToClient?.Invoke(this, (VisualDebuggerCaptureToClient)packet);
                    break;
                }
                case Messages.ClientRegion.ScriptModalDialog:
                {
                    OnScriptModalDialog?.Invoke(this, (ScriptModalDialog)packet);
                    break;
                }
                case Messages.ClientRegion.ScriptModalDialogResponse:
                {
                    OnScriptModalDialogResponse?.Invoke(this, (ScriptModalDialogResponse)packet);
                    break;
                }
                case Messages.ClientRegion.TwitchEventSubscription:
                {
                    OnTwitchEventSubscription?.Invoke(this, (TwitchEventSubscription)packet);
                    break;
                }
                case Messages.ClientRegion.TwitchEvent:
                {
                    OnTwitchEvent?.Invoke(this, (TwitchEvent)packet);
                    break;
                }
                case Messages.ClientRegion.ClientStaticReady:
                {
                    OnClientStaticReady?.Invoke(this, (ClientStaticReady)packet);
                    break;
                }
                case Messages.ClientRegion.ClientDynamicReady:
                {
                    OnClientDynamicReady?.Invoke(this, (ClientDynamicReady)packet);
                    break;
                }
                case Messages.ClientRegion.InitialChunkSubscribed:
                {
                    OnInitialChunkSubscribed?.Invoke(this, (InitialChunkSubscribed)packet);
                    break;
                }
                case Messages.ClientRegion.ClientRegionCommandMessage:
                {
                    OnClientRegionCommandMessage?.Invoke(this, (ClientRegionCommandMessage)packet);
                    break;
                }
                case Messages.ClientRegion.ClientKickNotification:
                {
                    OnClientKickNotification?.Invoke(this, (ClientKickNotification)packet);
                    break;
                }
                case Messages.ClientRegion.ClientSmiteNotification:
                {
                    OnClientSmiteNotification?.Invoke(this, (ClientSmiteNotification)packet);
                    break;
                }
                case Messages.ClientRegion.ClientMuteNotification:
                {
                    OnClientMuteNotification?.Invoke(this, (ClientMuteNotification)packet);
                    break;
                }
                case Messages.ClientRegion.ClientVoiceBroadcastStartNotification:
                {
                    OnClientVoiceBroadcastStartNotification?.Invoke(this, (ClientVoiceBroadcastStartNotification)packet);
                    break;
                }
                case Messages.ClientRegion.ClientVoiceBroadcastStopNotification:
                {
                    OnClientVoiceBroadcastStopNotification?.Invoke(this, (ClientVoiceBroadcastStopNotification)packet);
                    break;
                }
                case Messages.ClientRegion.ClientRuntimeInventoryUpdatedNotification:
                {
                    OnClientRuntimeInventoryUpdatedNotification?.Invoke(this, (ClientRuntimeInventoryUpdatedNotification)packet);
                    break;
                }
                case Messages.ClientRegion.ClientSetRegionBroadcasted:
                {
                    OnClientSetRegionBroadcasted?.Invoke(this, (ClientSetRegionBroadcasted)packet);
                    break;
                }
                case Messages.ClientRegion.SubscribeCommand:
                {
                    OnSubscribeCommand?.Invoke(this, (SubscribeCommand)packet);
                    break;
                }
                case Messages.ClientRegion.UnsubscribeCommand:
                {
                    OnUnsubscribeCommand?.Invoke(this, (UnsubscribeCommand)packet);
                    break;
                }
                case Messages.ClientRegion.ClientCommand:
                {
                    OnClientCommand?.Invoke(this, (ClientCommand)packet);
                    break;
                }
                case Messages.ClientRegion.RequestDropPortal:
                {
                    OnRequestDropPortal?.Invoke(this, (RequestDropPortal)packet);
                    break;
                }
                case Messages.ClientRegion.OpenStoreListing:
                {
                    OnOpenStoreListing?.Invoke(this, (OpenStoreListing)packet);
                    break;
                }
                case Messages.ClientRegion.OpenUserStore:
                {
                    OnOpenUserStore?.Invoke(this, (OpenUserStore)packet);
                    break;
                }
                case Messages.ClientRegion.OpenQuestCharacterDialog:
                {
                    OnOpenQuestCharacterDialog?.Invoke(this, (OpenQuestCharacterDialog)packet);
                    break;
                }
                case Messages.ClientRegion.UIScriptableBarStart:
                {
                    OnUIScriptableBarStart?.Invoke(this, (UIScriptableBarStart)packet);
                    break;
                }
                case Messages.ClientRegion.UIScriptableBarStopped:
                {
                    OnUIScriptableBarStopped?.Invoke(this, (UIScriptableBarStopped)packet);
                    break;
                }
                case Messages.ClientRegion.UIScriptableBarCancel:
                {
                    OnUIScriptableBarCancel?.Invoke(this, (UIScriptableBarCancel)packet);
                    break;
                }
                case Messages.ClientRegion.UIHintTextUpdate:
                {
                    OnUIHintTextUpdate?.Invoke(this, (UIHintTextUpdate)packet);
                    break;
                }
                case Messages.ClientRegion.QuestOfferResponse:
                {
                    OnQuestOfferResponse?.Invoke(this, (QuestOfferResponse)packet);
                    break;
                }
                case Messages.ClientRegion.QuestCompleted:
                {
                    OnQuestCompleted?.Invoke(this, (QuestCompleted)packet);
                    break;
                }
                case Messages.ClientRegion.QuestRemoved:
                {
                    OnQuestRemoved?.Invoke(this, (QuestRemoved)packet);
                    break;
                }
                case Messages.ClientRegion.ShowWorldDetail:
                {
                    OnShowWorldDetail?.Invoke(this, (ShowWorldDetail)packet);
                    break;
                }
                case Messages.ClientRegion.ShowTutorialHint:
                {
                    OnShowTutorialHint?.Invoke(this, (ShowTutorialHint)packet);
                    break;
                }
                case Messages.ClientRegion.TutorialHintsSetEnabled:
                {
                    OnTutorialHintsSetEnabled?.Invoke(this, (TutorialHintsSetEnabled)packet);
                    break;
                }
                case Messages.ClientRegion.ReactionDefinition:
                {
                    OnReactionDefinition?.Invoke(this, (ReactionDefinition)packet);
                    break;
                }
                case Messages.ClientRegion.SystemReactionDefinition:
                {
                    OnSystemReactionDefinition?.Invoke(this, (SystemReactionDefinition)packet);
                    break;
                }
                case Messages.ClientRegion.UpdateReactions:
                {
                    OnUpdateReactions?.Invoke(this, (UpdateReactions)packet);
                    break;
                }
                case Messages.ClientRegion.AddReaction:
                {
                    OnAddReaction?.Invoke(this, (AddReaction)packet);
                    break;
                }
                case Messages.ClientRegion.RemoveReaction:
                {
                    OnRemoveReaction?.Invoke(this, (RemoveReaction)packet);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
