using SanProtocol;
using SanProtocol.AnimationComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class AnimationComponent : IMessageHandler
    {
        public event EventHandler<FloatVariable>? OnFloatVariable;
        public event EventHandler<FloatNodeVariable>? OnFloatNodeVariable;
        public event EventHandler<FloatRangeNodeVariable>? OnFloatRangeNodeVariable;
        public event EventHandler<VectorVariable>? OnVectorVariable;
        public event EventHandler<QuaternionVariable>? OnQuaternionVariable;
        public event EventHandler<Int8Variable>? OnInt8Variable;
        public event EventHandler<BoolVariable>? OnBoolVariable;
        public event EventHandler<CharacterTransform>? OnCharacterTransform;
        public event EventHandler<CharacterTransformPersistent>? OnCharacterTransformPersistent;
        public event EventHandler<CharacterAnimationDestroyed>? OnCharacterAnimationDestroyed;
        public event EventHandler<AnimationOverride>? OnAnimationOverride;
        public event EventHandler<BehaviorInternalState>? OnBehaviorInternalState;
        public event EventHandler<CharacterBehaviorInternalState>? OnCharacterBehaviorInternalState;
        public event EventHandler<BehaviorStateUpdate>? OnBehaviorStateUpdate;
        public event EventHandler<BehaviorInitializationData>? OnBehaviorInitializationData;
        public event EventHandler<CharacterSetPosition>? OnCharacterSetPosition;
        public event EventHandler<PlayAnimation>? OnPlayAnimation;

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.AnimationComponent.FloatVariable:
                {
                    OnFloatVariable?.Invoke(this, (FloatVariable)packet);
                    break;
                }
                case Messages.AnimationComponent.FloatNodeVariable:
                {
                    OnFloatNodeVariable?.Invoke(this, (FloatNodeVariable)packet);
                    break;
                }
                case Messages.AnimationComponent.FloatRangeNodeVariable:
                {
                    OnFloatRangeNodeVariable?.Invoke(this, (FloatRangeNodeVariable)packet);
                    break;
                }
                case Messages.AnimationComponent.VectorVariable:
                {
                    OnVectorVariable?.Invoke(this, (VectorVariable)packet);
                    break;
                }
                case Messages.AnimationComponent.QuaternionVariable:
                {
                    OnQuaternionVariable?.Invoke(this, (QuaternionVariable)packet);
                    break;
                }
                case Messages.AnimationComponent.Int8Variable:
                {
                    OnInt8Variable?.Invoke(this, (Int8Variable)packet);
                    break;
                }
                case Messages.AnimationComponent.BoolVariable:
                {
                    OnBoolVariable?.Invoke(this, (BoolVariable)packet);
                    break;
                }
                case Messages.AnimationComponent.CharacterTransform:
                {
                    OnCharacterTransform?.Invoke(this, (CharacterTransform)packet);
                    break;
                }
                case Messages.AnimationComponent.CharacterTransformPersistent:
                {
                    OnCharacterTransformPersistent?.Invoke(this, (CharacterTransformPersistent)packet);
                    break;
                }
                case Messages.AnimationComponent.CharacterAnimationDestroyed:
                {
                    OnCharacterAnimationDestroyed?.Invoke(this, (CharacterAnimationDestroyed)packet);
                    break;
                }
                case Messages.AnimationComponent.AnimationOverride:
                {
                    OnAnimationOverride?.Invoke(this, (AnimationOverride)packet);
                    break;
                }
                case Messages.AnimationComponent.BehaviorInternalState:
                {
                    OnBehaviorInternalState?.Invoke(this, (BehaviorInternalState)packet);
                    break;
                }
                case Messages.AnimationComponent.CharacterBehaviorInternalState:
                {
                    OnCharacterBehaviorInternalState?.Invoke(this, (CharacterBehaviorInternalState)packet);
                    break;
                }
                case Messages.AnimationComponent.BehaviorStateUpdate:
                {
                    OnBehaviorStateUpdate?.Invoke(this, (BehaviorStateUpdate)packet);
                    break;
                }
                case Messages.AnimationComponent.BehaviorInitializationData:
                {
                    OnBehaviorInitializationData?.Invoke(this, (BehaviorInitializationData)packet);
                    break;
                }
                case Messages.AnimationComponent.CharacterSetPosition:
                {
                    OnCharacterSetPosition?.Invoke(this, (CharacterSetPosition)packet);
                    break;
                }
                case Messages.AnimationComponent.PlayAnimation:
                {
                    OnPlayAnimation?.Invoke(this, (PlayAnimation)packet);
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
