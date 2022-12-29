using SanProtocol;
using SanProtocol.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class Audio : IMessageHandler
    {
        public event EventHandler<LoadSound>? OnLoadSound;
        public event EventHandler<PlaySound>? OnPlaySound;
        public event EventHandler<PlayStream>? OnPlayStream;
        public event EventHandler<StopBroadcastingSound>? OnStopBroadcastingSound;
        public event EventHandler<SetAudioStream>? OnSetAudioStream;
        public event EventHandler<SetMediaSource>? OnSetMediaSource;
        public event EventHandler<PerformMediaAction>? OnPerformMediaAction;
        public event EventHandler<StopSound>? OnStopSound;
        public event EventHandler<SetLoudness>? OnSetLoudness;
        public event EventHandler<SetPitch>? OnSetPitch;

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.Audio.LoadSound:
                {
                    OnLoadSound?.Invoke(this, (LoadSound)packet);
                    break;
                }
                case Messages.Audio.PlaySound:
                {
                    OnPlaySound?.Invoke(this, (PlaySound)packet);
                    break;
                }
                case Messages.Audio.PlayStream:
                {
                    OnPlayStream?.Invoke(this, (PlayStream)packet);
                    break;
                }
                case Messages.Audio.StopBroadcastingSound:
                {
                    OnStopBroadcastingSound?.Invoke(this, (StopBroadcastingSound)packet);
                    break;
                }
                case Messages.Audio.SetAudioStream:
                {
                    OnSetAudioStream?.Invoke(this, (SetAudioStream)packet);
                    break;
                }
                case Messages.Audio.SetMediaSource:
                {
                    OnSetMediaSource?.Invoke(this, (SetMediaSource)packet);
                    break;
                }
                case Messages.Audio.PerformMediaAction:
                {
                    OnPerformMediaAction?.Invoke(this, (PerformMediaAction)packet);
                    break;
                }
                case Messages.Audio.StopSound:
                {
                    OnStopSound?.Invoke(this, (StopSound)packet);
                    break;
                }
                case Messages.Audio.SetLoudness:
                {
                    OnSetLoudness?.Invoke(this, (SetLoudness)packet);
                    break;
                }
                case Messages.Audio.SetPitch:
                {
                    OnSetPitch?.Invoke(this, (SetPitch)packet);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleLoadSound(BinaryReader reader)
        {
            var packet = new LoadSound(reader);
            OnLoadSound?.Invoke(this, packet);
        }

        void HandlePlaySound(BinaryReader reader)
        {
            var packet = new PlaySound(reader);
            OnPlaySound?.Invoke(this, packet);
        }

        void HandlePlayStream(BinaryReader reader)
        {
            var packet = new PlayStream(reader);
            OnPlayStream?.Invoke(this, packet);
        }

        void HandleStopBroadcastingSound(BinaryReader reader)
        {
            var packet = new StopBroadcastingSound(reader);
            OnStopBroadcastingSound?.Invoke(this, packet);
        }

        void HandleSetAudioStream(BinaryReader reader)
        {
            var packet = new SetAudioStream(reader);
            OnSetAudioStream?.Invoke(this, packet);
        }

        void HandleSetMediaSource(BinaryReader reader)
        {
            var packet = new SetMediaSource(reader);
            OnSetMediaSource?.Invoke(this, packet);
        }

        void HandlePerformMediaAction(BinaryReader reader)
        {
            var packet = new PerformMediaAction(reader);
            OnPerformMediaAction?.Invoke(this, packet);
        }

        void HandleStopSound(BinaryReader reader)
        {
            var packet = new StopSound(reader);
            OnStopSound?.Invoke(this, packet);
        }

        void HandleSetLoudness(BinaryReader reader)
        {
            var packet = new SetLoudness(reader);
            OnSetLoudness?.Invoke(this, packet);
        }

        void HandleSetPitch(BinaryReader reader)
        {
            var packet = new SetPitch(reader);
            OnSetPitch?.Invoke(this, packet);
        }

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
