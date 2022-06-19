using SanProtocol;
using SanProtocol.GameWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core.MessageHandlers
{
    public class GameWorld : IMessageHandler
    {
        public event EventHandler<StaticMeshFlagsChanged>? OnStaticMeshFlagsChanged;
        public event EventHandler<StaticMeshScaleChanged>? OnStaticMeshScaleChanged;
        public event EventHandler<Timestamp>? OnTimestamp;
        public event EventHandler<MoveEntity>? OnMoveEntity;
        public event EventHandler<ChangeMaterialVectorParam>? OnChangeMaterialVectorParam;
        public event EventHandler<ChangeMaterialFloatParam>? OnChangeMaterialFloatParam;
        public event EventHandler<ChangeMaterial>? OnChangeMaterial;
        public event EventHandler<RiggedMeshFlagsChange>? OnRiggedMeshFlagsChange;
        public event EventHandler<RiggedMeshScaleChanged>? OnRiggedMeshScaleChanged;
        public event EventHandler<ScriptCameraMessage>? OnScriptCameraMessage;
        public event EventHandler<ScriptCameraCapture>? OnScriptCameraCapture; // NEW: 2021-03-25
        public event EventHandler<UpdateRuntimeInventorySettings>? OnUpdateRuntimeInventorySettings;

        public bool OnMessage(uint messageId, BinaryReader reader)
        {
            switch (messageId)
            {
                case Messages.GameWorld.Timestamp:
                {
                    this.HandleTimestamp(reader);
                    break;
                }
                case Messages.GameWorld.MoveEntity:
                {
                    this.HandleMoveEntity(reader);
                    break;
                }
                case Messages.GameWorld.ChangeMaterialVectorParam:
                {
                    this.HandleChangeMaterialVectorParam(reader);
                    break;
                }
                case Messages.GameWorld.ChangeMaterialFloatParam:
                {
                    this.HandleChangeMaterialFloatParam(reader);
                    break;
                }
                case Messages.GameWorld.ChangeMaterial:
                {
                    this.HandleChangeMaterial(reader);
                    break;
                }
                case Messages.GameWorld.StaticMeshFlagsChanged:
                {
                    this.HandleStaticMeshFlagsChanged(reader);
                    break;
                }
                case Messages.GameWorld.StaticMeshScaleChanged:
                {
                    this.HandleStaticMeshScaleChanged(reader);
                    break;
                }
                case Messages.GameWorld.RiggedMeshFlagsChange:
                {
                    this.HandleRiggedMeshFlagsChange(reader);
                    break;
                }
                case Messages.GameWorld.RiggedMeshScaleChanged:
                {
                    this.HandleRiggedMeshScaleChanged(reader);
                    break;
                }
                case Messages.GameWorld.ScriptCameraMessage:
                {
                    this.HandleScriptCameraMessage(reader);
                    break;
                }
                case Messages.GameWorld.ScriptCameraCapture:
                {
                    this.HandleScriptCameraCapture(reader);
                    break;
                }
                case Messages.GameWorld.UpdateRuntimeInventorySettings:
                {
                    this.HandleUpdateRuntimeInventorySettings(reader);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            return true;
        }

        void HandleTimestamp(BinaryReader reader)
        {
            var packet = new Timestamp(reader);
            OnTimestamp?.Invoke(this, packet);
        }

        void HandleMoveEntity(BinaryReader reader)
        {
            var packet = new MoveEntity(reader);
            OnMoveEntity?.Invoke(this, packet);
        }

        void HandleChangeMaterialVectorParam(BinaryReader reader)
        {
            var packet = new ChangeMaterialVectorParam(reader);
            OnChangeMaterialVectorParam?.Invoke(this, packet);
        }

        void HandleChangeMaterialFloatParam(BinaryReader reader)
        {
            var packet = new ChangeMaterialFloatParam(reader);
            OnChangeMaterialFloatParam?.Invoke(this, packet);
        }

        void HandleChangeMaterial(BinaryReader reader)
        {
            var packet = new ChangeMaterial(reader);
            OnChangeMaterial?.Invoke(this, packet);
        }

        void HandleStaticMeshFlagsChanged(BinaryReader reader)
        {
            var packet = new StaticMeshFlagsChanged(reader);
            OnStaticMeshFlagsChanged?.Invoke(this, packet);
        }

        void HandleStaticMeshScaleChanged(BinaryReader reader)
        {
            var packet = new StaticMeshScaleChanged(reader);
            OnStaticMeshScaleChanged?.Invoke(this, packet);
        }

        void HandleRiggedMeshFlagsChange(BinaryReader reader)
        {
            var packet = new RiggedMeshFlagsChange(reader);
            OnRiggedMeshFlagsChange?.Invoke(this, packet);
        }

        void HandleRiggedMeshScaleChanged(BinaryReader reader)
        {
            var packet = new RiggedMeshScaleChanged(reader);
            OnRiggedMeshScaleChanged?.Invoke(this, packet);
        }

        void HandleScriptCameraMessage(BinaryReader reader)
        {
            var packet = new ScriptCameraMessage(reader);
            OnScriptCameraMessage?.Invoke(this, packet);
        }

        void HandleScriptCameraCapture(BinaryReader reader)
        {
            var packet = new ScriptCameraCapture(reader);
            OnScriptCameraCapture?.Invoke(this, packet);
        }

        void HandleUpdateRuntimeInventorySettings(BinaryReader reader)
        {
            var packet = new UpdateRuntimeInventorySettings(reader);
            OnUpdateRuntimeInventorySettings?.Invoke(this, packet);
        }
    }
}
