using SanProtocol;
using SanProtocol.ClientVoice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class ClientVoice : IMessageHandler
    {
        public event EventHandler<Login>? OnLogin;
        public event EventHandler<LoginReply>? OnLoginReply;
        public event EventHandler<AudioData>? OnAudioData;
        public event EventHandler<SpeechGraphicsData>? OnSpeechGraphicsData;
        public event EventHandler<LocalAudioData>? OnLocalAudioData;
        public event EventHandler<LocalAudioStreamState>? OnLocalAudioStreamState;
        public event EventHandler<LocalAudioPosition>? OnLocalAudioPosition;
        public event EventHandler<LocalAudioMute>? OnLocalAudioMute;
        public event EventHandler<LocalSetRegionBroadcasted>? OnLocalSetRegionBroadcasted;
        public event EventHandler<LocalSetMuteAll>? OnLocalSetMuteAll;
        public event EventHandler<GroupAudioData>? OnGroupAudioData;
        public event EventHandler<LocalTextData>? OnLocalTextData;
        public event EventHandler<MasterInstance>? OnMasterInstance;
        public event EventHandler<VoiceModerationCommand>? OnVoiceModerationCommand;
        public event EventHandler<VoiceModerationCommandResponse>? OnVoiceModerationCommandResponse;
        public event EventHandler<VoiceNotification>? OnVoiceNotification;

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.ClientVoice.Login:
                {
                    this.HandleLogin(reader);
                    break;
                }
                case Messages.ClientVoice.LoginReply:
                {
                    this.HandleLoginReply(reader);
                    break;
                }
                case Messages.ClientVoice.AudioData:
                {
                    this.HandleAudioData(reader);
                    break;
                }
                case Messages.ClientVoice.SpeechGraphicsData:
                {
                    this.HandleSpeechGraphicsData(reader);
                    break;
                }
                case Messages.ClientVoice.LocalAudioData:
                {
                    this.HandleLocalAudioData(reader);
                    break;
                }
                case Messages.ClientVoice.LocalAudioStreamState:
                {
                    this.HandleLocalAudioStreamState(reader);
                    break;
                }
                case Messages.ClientVoice.LocalAudioPosition:
                {
                    this.HandleLocalAudioPosition(reader);
                    break;
                }
                case Messages.ClientVoice.LocalAudioMute:
                {
                    this.HandleLocalAudioMute(reader);
                    break;
                }
                case Messages.ClientVoice.LocalSetRegionBroadcasted:
                {
                    this.HandleLocalSetRegionBroadcasted(reader);
                    break;
                }
                case Messages.ClientVoice.LocalSetMuteAll:
                {
                    this.HandleLocalSetMuteAll(reader);
                    break;
                }
                case Messages.ClientVoice.GroupAudioData:
                {
                    this.HandleGroupAudioData(reader);
                    break;
                }
                case Messages.ClientVoice.LocalTextData:
                {
                    this.HandleLocalTextData(reader);
                    break;
                }
                case Messages.ClientVoice.MasterInstance:
                {
                    this.HandleMasterInstance(reader);
                    break;
                }
                case Messages.ClientVoice.VoiceModerationCommand:
                {
                    this.HandleVoiceModerationCommand(reader);
                    break;
                }
                case Messages.ClientVoice.VoiceModerationCommandResponse:
                {
                    this.HandleVoiceModerationCommandResponse(reader);
                    break;
                }
                case Messages.ClientVoice.VoiceNotification:
                {
                    this.HandleVoiceNotification(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleLogin(BinaryReader reader)
        {
            var packet = new Login(reader);
            OnLogin?.Invoke(this, packet);
        }

        void HandleLoginReply(BinaryReader reader)
        {
            var packet = new LoginReply(reader);
            OnLoginReply?.Invoke(this, packet);

            //var newPacket = new LocalSetRegionBroadcasted(1);
            ////Client.Instance.VoiceClient.SendPacket(newPacket);
            //Client.SendPacket(newPacket);

            //// TODO: SEND LocalAudioStreamState
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    var setState = new SanProtocol.ClientVoice.LocalAudioStreamState(Client.Instance.VoiceClient.InstanceId, 0, 0, 1);
            //    //Client.Instance.VoiceClient.SendPacket(setState);
            //    Client.SendPacket(setState);
            //}

            //// TODO: SEND LocalAudioPosition
            //// 06 9C BA 98 17 00 00 00 00 DB 4D 89 F0 CC E5 75 E1 BF 9B 0E 6F 4F 4D 5A 9C 10 35 C1 3E C0 6E 10 A2 10 3F 27 3B 01 00 00 00 3C 68
            ////    9C BA 98 17 00 00 00 00 2B 40 74 87 5D 85 0D 75 C9 B9 61 AB 11 5E D2 9A 00 00 00 00 F1 BF FA A1 5A 02 26 3B 02 00 00 00
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    var setState = new SanProtocol.ClientVoice.LocalAudioPosition(
            //        0,
            //        Client.Instance.VoiceClient.InstanceId,
            //        new List<float>() { 0, 0, 0 },
            //        0//Client.Instance.MyAgentControllerId.Value
            //    );

            //    //Client.Instance.VoiceClient.SendPacket(setState);
            //    Client.SendPacket(setState);
            //}
        }

        void HandleAudioData(BinaryReader reader)
        {
            var packet = new AudioData(reader);
            OnAudioData?.Invoke(this, packet);
        }

        void HandleSpeechGraphicsData(BinaryReader reader)
        {
            var packet = new SpeechGraphicsData(reader);
            OnSpeechGraphicsData?.Invoke(this, packet);
        }

        void HandleLocalAudioData(BinaryReader reader)
        {
            var packet = new LocalAudioData(reader);
            OnLocalAudioData?.Invoke(this, packet);
        }

        void HandleLocalAudioStreamState(BinaryReader reader)
        {
            var packet = new LocalAudioStreamState(reader);
            OnLocalAudioStreamState?.Invoke(this, packet);
        }

        void HandleLocalAudioPosition(BinaryReader reader)
        {
            var packet = new LocalAudioPosition(reader);
            OnLocalAudioPosition?.Invoke(this, packet);
        }

        void HandleLocalAudioMute(BinaryReader reader)
        {
            var packet = new LocalAudioMute(reader);
            OnLocalAudioMute?.Invoke(this, packet);
        }

        void HandleLocalSetRegionBroadcasted(BinaryReader reader)
        {
            var packet = new LocalSetRegionBroadcasted(reader);
            OnLocalSetRegionBroadcasted?.Invoke(this, packet);
        }

        void HandleLocalSetMuteAll(BinaryReader reader)
        {
            var packet = new LocalSetMuteAll(reader);
            OnLocalSetMuteAll?.Invoke(this, packet);
        }

        void HandleGroupAudioData(BinaryReader reader)
        {
            var packet = new GroupAudioData(reader);
            OnGroupAudioData?.Invoke(this, packet);
        }

        void HandleLocalTextData(BinaryReader reader)
        {
            var packet = new LocalTextData(reader);
            OnLocalTextData?.Invoke(this, packet);
        }

        void HandleMasterInstance(BinaryReader reader)
        {
            var packet = new MasterInstance(reader);
            OnMasterInstance?.Invoke(this, packet);
        }

        void HandleVoiceModerationCommand(BinaryReader reader)
        {
            var packet = new VoiceModerationCommand(reader);
            OnVoiceModerationCommand?.Invoke(this, packet);
        }

        void HandleVoiceModerationCommandResponse(BinaryReader reader)
        {
            var packet = new VoiceModerationCommandResponse(reader);
            OnVoiceModerationCommandResponse?.Invoke(this, packet);
        }

        void HandleVoiceNotification(BinaryReader reader)
        {
            var packet = new VoiceNotification(reader);
            OnVoiceNotification?.Invoke(this, packet);
        }

        public bool OnMessage(IPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
