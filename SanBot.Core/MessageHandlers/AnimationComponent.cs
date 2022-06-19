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

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.AnimationComponent.FloatVariable:
                {
                    this.HandleFloatVariable(reader);
                    break;
                }
                case Messages.AnimationComponent.FloatNodeVariable:
                {
                    this.HandleFloatNodeVariable(reader);
                    break;
                }
                case Messages.AnimationComponent.FloatRangeNodeVariable:
                {
                    this.HandleFloatRangeNodeVariable(reader);
                    break;
                }
                case Messages.AnimationComponent.VectorVariable:
                {
                    this.HandleVectorVariable(reader);
                    break;
                }
                case Messages.AnimationComponent.QuaternionVariable:
                {
                    this.HandleQuaternionVariable(reader);
                    break;
                }
                case Messages.AnimationComponent.Int8Variable:
                {
                    this.HandleInt8Variable(reader);
                    break;
                }
                case Messages.AnimationComponent.BoolVariable:
                {
                    this.HandleBoolVariable(reader);
                    break;
                }
                case Messages.AnimationComponent.CharacterTransform:
                {
                    this.HandleCharacterTransform(reader);
                    break;
                }
                case Messages.AnimationComponent.CharacterTransformPersistent:
                {
                    this.HandleCharacterTransformPersistent(reader);
                    break;
                }
                case Messages.AnimationComponent.CharacterAnimationDestroyed:
                {
                    this.HandleCharacterAnimationDestroyed(reader);
                    break;
                }
                case Messages.AnimationComponent.AnimationOverride:
                {
                    this.HandleAnimationOverride(reader);
                    break;
                }
                case Messages.AnimationComponent.BehaviorInternalState:
                {
                    this.HandleBehaviorInternalState(reader);
                    break;
                }
                case Messages.AnimationComponent.CharacterBehaviorInternalState:
                {
                    this.HandleCharacterBehaviorInternalState(reader);
                    break;
                }
                case Messages.AnimationComponent.BehaviorStateUpdate:
                {
                    this.HandleBehaviorStateUpdate(reader);
                    break;
                }
                case Messages.AnimationComponent.BehaviorInitializationData:
                {
                    this.HandleBehaviorInitializationData(reader);
                    break;
                }
                case Messages.AnimationComponent.CharacterSetPosition:
                {
                    this.HandleCharacterSetPosition(reader);
                    break;
                }
                case Messages.AnimationComponent.PlayAnimation:
                {
                    this.HandlePlayAnimation(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleFloatVariable(BinaryReader reader)
        {
            var packet = new FloatVariable(reader);
            OnFloatVariable?.Invoke(this, packet);
        }

        void HandleFloatNodeVariable(BinaryReader reader)
        {
            var packet = new FloatNodeVariable(reader);
            OnFloatNodeVariable?.Invoke(this, packet);
        }

        void HandleFloatRangeNodeVariable(BinaryReader reader)
        {
            var packet = new FloatRangeNodeVariable(reader);
            OnFloatRangeNodeVariable?.Invoke(this, packet);
        }

        void HandleVectorVariable(BinaryReader reader)
        {
            var packet = new VectorVariable(reader);
            OnVectorVariable?.Invoke(this, packet);
        }

        void HandleQuaternionVariable(BinaryReader reader)
        {
            var packet = new QuaternionVariable(reader);
            OnQuaternionVariable?.Invoke(this, packet);
        }

        void HandleInt8Variable(BinaryReader reader)
        {
            var packet = new Int8Variable(reader);
            OnInt8Variable?.Invoke(this, packet);
        }

        void HandleBoolVariable(BinaryReader reader)
        {
            var packet = new BoolVariable(reader);
            OnBoolVariable?.Invoke(this, packet);
        }

        void HandleCharacterTransform(BinaryReader reader)
        {
            var packet = new CharacterTransform(reader);
            OnCharacterTransform?.Invoke(this, packet);
        }

        void HandleCharacterTransformPersistent(BinaryReader reader)
        {

            var packet = new CharacterTransformPersistent(reader);
            OnCharacterTransformPersistent?.Invoke(this, packet);
        }

        void HandleCharacterAnimationDestroyed(BinaryReader reader)
        {
            var packet = new CharacterAnimationDestroyed(reader);
            OnCharacterAnimationDestroyed?.Invoke(this, packet);
        }

        void HandleAnimationOverride(BinaryReader reader)
        {
            var packet = new AnimationOverride(reader);
            OnAnimationOverride?.Invoke(this, packet);
        }

        void HandleBehaviorInternalState(BinaryReader reader)
        {
            var packet = new BehaviorInternalState(reader);
            OnBehaviorInternalState?.Invoke(this, packet);
        }

        void HandleCharacterBehaviorInternalState(BinaryReader reader)
        {
            var packet = new CharacterBehaviorInternalState(reader);
            OnCharacterBehaviorInternalState?.Invoke(this, packet);
        }

        void HandleBehaviorStateUpdate(BinaryReader reader)
        {
            var packet = new BehaviorStateUpdate(reader);
            OnBehaviorStateUpdate?.Invoke(this, packet);
        }

        void HandleBehaviorInitializationData(BinaryReader reader)
        {
            var packet = new BehaviorInitializationData(reader);
            OnBehaviorInitializationData?.Invoke(this, packet);
        }

        void HandleCharacterSetPosition(BinaryReader reader)
        {
            var packet = new CharacterSetPosition(reader);
            OnCharacterSetPosition?.Invoke(this, packet);
        }

        void HandlePlayAnimation(BinaryReader reader)
        {
            var packet = new PlayAnimation(reader);
            OnPlayAnimation?.Invoke(this, packet);
        }

    }

}
