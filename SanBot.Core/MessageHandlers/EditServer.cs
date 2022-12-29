using SanProtocol.EditServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SanProtocol;

namespace SanBot.Core.MessageHandlers
{
    public class EditServer : IMessageHandler
    {
        public event EventHandler<UserLogin>? OnUserLogin;
        public event EventHandler<UserLoginReply>? OnUserLoginReply;
        public event EventHandler<AddUser>? OnAddUser;
        public event EventHandler<RemoveUser>? OnRemoveUser;
        public event EventHandler<OpenWorkspace>? OnOpenWorkspace;
        public event EventHandler<CloseWorkspace>? OnCloseWorkspace;
        public event EventHandler<EditWorkspaceCommand>? OnEditWorkspaceCommand;
        public event EventHandler<SaveWorkspace>? OnSaveWorkspace;
        public event EventHandler<SaveWorkspaceReply>? OnSaveWorkspaceReply;
        public event EventHandler<BuildWorkspace>? OnBuildWorkspace;
        public event EventHandler<UpdateWorkspaceClientBuiltBakeData>? OnUpdateWorkspaceClientBuiltBakeData;
        public event EventHandler<BuildWorkspaceCompileReply>? OnBuildWorkspaceCompileReply;
        public event EventHandler<BuildWorkspaceProgressUpdate>? OnBuildWorkspaceProgressUpdate;
        public event EventHandler<BuildWorkspaceUploadReply>? OnBuildWorkspaceUploadReply;
        public event EventHandler<WorkspaceReadyReply>? OnWorkspaceReadyReply;
        public event EventHandler<SaveWorkspaceSelectionToInventory>? OnSaveWorkspaceSelectionToInventory;
        public event EventHandler<SaveWorkspaceSelectionToInventoryReply>? OnSaveWorkspaceSelectionToInventoryReply;
        public event EventHandler<InventoryCreateItem>? OnInventoryCreateItem;
        public event EventHandler<InventoryDeleteItem>? OnInventoryDeleteItem;
        public event EventHandler<InventoryChangeItemName>? OnInventoryChangeItemName;
        public event EventHandler<InventoryChangeItemState>? OnInventoryChangeItemState;
        public event EventHandler<InventoryModifyItemThumbnailAssetId>? OnInventoryModifyItemThumbnailAssetId;
        public event EventHandler<InventoryModifyItemCapabilities>? OnInventoryModifyItemCapabilities;
        public event EventHandler<InventorySaveItem>? OnInventorySaveItem;
        public event EventHandler<InventoryUpdateItemReply>? OnInventoryUpdateItemReply;
        public event EventHandler<InventoryItemUpload>? OnInventoryItemUpload;
        public event EventHandler<InventoryItemUploadReply>? OnInventoryItemUploadReply;
        public event EventHandler<InventoryCreateListing>? OnInventoryCreateListing;
        public event EventHandler<InventoryCreateListingReply>? OnInventoryCreateListingReply;
        public event EventHandler<BeginEditServerSpawn>? OnBeginEditServerSpawn;
        public event EventHandler<EditServerSpawnReady>? OnEditServerSpawnReady;

        public bool OnMessage(IPacket packet)
        {

            switch (packet.MessageId)
            {
                case Messages.EditServer.UserLogin:
                {
                    OnUserLogin?.Invoke(this, (UserLogin)packet);
                    break;
                }
                case Messages.EditServer.UserLoginReply:
                {
                    OnUserLoginReply?.Invoke(this, (UserLoginReply)packet);
                    break;
                }
                case Messages.EditServer.AddUser:
                {
                    OnAddUser?.Invoke(this, (AddUser)packet);
                    break;
                }
                case Messages.EditServer.RemoveUser:
                {
                    OnRemoveUser?.Invoke(this, (RemoveUser)packet);
                    break;
                }
                case Messages.EditServer.OpenWorkspace:
                {
                    OnOpenWorkspace?.Invoke(this, (OpenWorkspace)packet);
                    break;
                }
                case Messages.EditServer.CloseWorkspace:
                {
                    OnCloseWorkspace?.Invoke(this, (CloseWorkspace)packet);
                    break;
                }
                case Messages.EditServer.EditWorkspaceCommand:
                {
                    OnEditWorkspaceCommand?.Invoke(this, (EditWorkspaceCommand)packet);
                    break;
                }
                case Messages.EditServer.SaveWorkspace:
                {
                    OnSaveWorkspace?.Invoke(this, (SaveWorkspace)packet);
                    break;
                }
                case Messages.EditServer.SaveWorkspaceReply:
                {
                    OnSaveWorkspaceReply?.Invoke(this, (SaveWorkspaceReply)packet);
                    break;
                }
                case Messages.EditServer.BuildWorkspace:
                {
                    OnBuildWorkspace?.Invoke(this, (BuildWorkspace)packet);
                    break;
                }
                case Messages.EditServer.UpdateWorkspaceClientBuiltBakeData:
                {
                    OnUpdateWorkspaceClientBuiltBakeData?.Invoke(this, (UpdateWorkspaceClientBuiltBakeData)packet);
                    break;
                }
                case Messages.EditServer.BuildWorkspaceCompileReply:
                {
                    OnBuildWorkspaceCompileReply?.Invoke(this, (BuildWorkspaceCompileReply)packet);
                    break;
                }
                case Messages.EditServer.BuildWorkspaceProgressUpdate:
                {
                    OnBuildWorkspaceProgressUpdate?.Invoke(this, (BuildWorkspaceProgressUpdate)packet);
                    break;
                }
                case Messages.EditServer.BuildWorkspaceUploadReply:
                {
                    OnBuildWorkspaceUploadReply?.Invoke(this, (BuildWorkspaceUploadReply)packet);
                    break;
                }
                case Messages.EditServer.WorkspaceReadyReply:
                {
                    OnWorkspaceReadyReply?.Invoke(this, (WorkspaceReadyReply)packet);
                    break;
                }
                case Messages.EditServer.SaveWorkspaceSelectionToInventory:
                {
                    OnSaveWorkspaceSelectionToInventory?.Invoke(this, (SaveWorkspaceSelectionToInventory)packet);
                    break;
                }
                case Messages.EditServer.SaveWorkspaceSelectionToInventoryReply:
                {
                    OnSaveWorkspaceSelectionToInventoryReply?.Invoke(this, (SaveWorkspaceSelectionToInventoryReply)packet);
                    break;
                }
                case Messages.EditServer.InventoryCreateItem:
                {
                    OnInventoryCreateItem?.Invoke(this, (InventoryCreateItem)packet);
                    break;
                }
                case Messages.EditServer.InventoryDeleteItem:
                {
                    OnInventoryDeleteItem?.Invoke(this, (InventoryDeleteItem)packet);
                    break;
                }
                case Messages.EditServer.InventoryChangeItemName:
                {
                    OnInventoryChangeItemName?.Invoke(this, (InventoryChangeItemName)packet);
                    break;
                }
                case Messages.EditServer.InventoryChangeItemState:
                {
                    OnInventoryChangeItemState?.Invoke(this, (InventoryChangeItemState)packet);
                    break;
                }
                case Messages.EditServer.InventoryModifyItemThumbnailAssetId:
                {
                    OnInventoryModifyItemThumbnailAssetId?.Invoke(this, (InventoryModifyItemThumbnailAssetId)packet);
                    break;
                }
                case Messages.EditServer.InventoryModifyItemCapabilities:
                {
                    OnInventoryModifyItemCapabilities?.Invoke(this, (InventoryModifyItemCapabilities)packet);
                    break;
                }
                case Messages.EditServer.InventorySaveItem:
                {
                    OnInventorySaveItem?.Invoke(this, (InventorySaveItem)packet);
                    break;
                }
                case Messages.EditServer.InventoryUpdateItemReply:
                {
                    OnInventoryUpdateItemReply?.Invoke(this, (InventoryUpdateItemReply)packet);
                    break;
                }
                case Messages.EditServer.InventoryItemUpload:
                {
                    OnInventoryItemUpload?.Invoke(this, (InventoryItemUpload)packet);
                    break;
                }
                case Messages.EditServer.InventoryItemUploadReply:
                {
                    OnInventoryItemUploadReply?.Invoke(this, (InventoryItemUploadReply)packet);
                    break;
                }
                case Messages.EditServer.InventoryCreateListing:
                {
                    OnInventoryCreateListing?.Invoke(this, (InventoryCreateListing)packet);
                    break;
                }
                case Messages.EditServer.InventoryCreateListingReply:
                {
                    OnInventoryCreateListingReply?.Invoke(this, (InventoryCreateListingReply)packet);
                    break;
                }
                case Messages.EditServer.BeginEditServerSpawn:
                {
                    OnBeginEditServerSpawn?.Invoke(this, (BeginEditServerSpawn)packet);
                    break;
                }
                case Messages.EditServer.EditServerSpawnReady:
                {
                    OnEditServerSpawnReady?.Invoke(this, (EditServerSpawnReady)packet);
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
