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

        public bool OnMessage(IPacket packet)
        {
            switch (packet.MessageId)
            {
                case Messages.GameWorld.Timestamp:
                {
                    OnTimestamp?.Invoke(this, (Timestamp)packet);
                    break;
                }
                case Messages.GameWorld.MoveEntity:
                {
                    OnMoveEntity?.Invoke(this, (MoveEntity)packet);
                    break;
                }
                case Messages.GameWorld.ChangeMaterialVectorParam:
                {
                    OnChangeMaterialVectorParam?.Invoke(this, (ChangeMaterialVectorParam)packet);
                    break;
                }
                case Messages.GameWorld.ChangeMaterialFloatParam:
                {
                    OnChangeMaterialFloatParam?.Invoke(this, (ChangeMaterialFloatParam)packet);
                    break;
                }
                case Messages.GameWorld.ChangeMaterial:
                {
                    OnChangeMaterial?.Invoke(this, (ChangeMaterial)packet);
                    break;
                }
                case Messages.GameWorld.StaticMeshFlagsChanged:
                {
                    OnStaticMeshFlagsChanged?.Invoke(this, (StaticMeshFlagsChanged)packet);
                    break;
                }
                case Messages.GameWorld.StaticMeshScaleChanged:
                {
                    OnStaticMeshScaleChanged?.Invoke(this, (StaticMeshScaleChanged)packet);
                    break;
                }
                case Messages.GameWorld.RiggedMeshFlagsChange:
                {
                    OnRiggedMeshFlagsChange?.Invoke(this, (RiggedMeshFlagsChange)packet);
                    break;
                }
                case Messages.GameWorld.RiggedMeshScaleChanged:
                {
                    OnRiggedMeshScaleChanged?.Invoke(this, (RiggedMeshScaleChanged)packet);
                    break;
                }
                case Messages.GameWorld.ScriptCameraMessage:
                {
                    OnScriptCameraMessage?.Invoke(this, (ScriptCameraMessage)packet);
                    break;
                }
                case Messages.GameWorld.ScriptCameraCapture:
                {
                    OnScriptCameraCapture?.Invoke(this, (ScriptCameraCapture)packet);
                    break;
                }
                case Messages.GameWorld.UpdateRuntimeInventorySettings:
                {
                    OnUpdateRuntimeInventorySettings?.Invoke(this, (UpdateRuntimeInventorySettings)packet);
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
