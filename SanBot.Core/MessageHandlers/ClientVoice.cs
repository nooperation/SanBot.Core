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
        public Action<IPacket>? OnPacket;

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            IPacket? newPacket = null;

            switch (messageId)
            {
                case Messages.ClientVoiceMessages.Login:
                {
                    newPacket = new Login(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LoginReply:
                {
                    newPacket = new LoginReply(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.AudioData:
                {
                    newPacket = new AudioData(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.SpeechGraphicsData:
                {
                    newPacket = new SpeechGraphicsData(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalAudioData:
                {
                    newPacket = new LocalAudioData(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalAudioStreamState:
                {
                    newPacket = new LocalAudioStreamState(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalAudioPosition:
                {
                    newPacket = new LocalAudioPosition(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalAudioMute:
                {
                    newPacket = new LocalAudioMute(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalSetRegionBroadcasted:
                {
                    newPacket = new LocalSetRegionBroadcasted(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalSetMuteAll:
                {
                    newPacket = new LocalSetMuteAll(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.GroupAudioData:
                {
                    newPacket = new GroupAudioData(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.LocalTextData:
                {
                    newPacket = new LocalTextData(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.MasterInstance:
                {
                    newPacket = new MasterInstance(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.VoiceModerationCommand:
                {
                    newPacket = new VoiceModerationCommand(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.VoiceModerationCommandResponse:
                {
                    newPacket = new VoiceModerationCommandResponse(reader);
                    break;
                }
                case Messages.ClientVoiceMessages.VoiceNotification:
                {
                    newPacket = new VoiceNotification(reader);
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
