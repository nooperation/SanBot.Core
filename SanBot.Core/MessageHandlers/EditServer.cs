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

        public bool OnMessage(uint messageId, BinaryReader reader)
        {

            switch (messageId)
            {
                case Messages.EditServer.UserLogin:
                {
                    this.HandleUserLogin(reader);
                    break;
                }
                case Messages.EditServer.UserLoginReply:
                {
                    this.HandleUserLoginReply(reader);
                    break;
                }
                case Messages.EditServer.AddUser:
                {
                    this.HandleAddUser(reader);
                    break;
                }
                case Messages.EditServer.RemoveUser:
                {
                    this.HandleRemoveUser(reader);
                    break;
                }
                case Messages.EditServer.OpenWorkspace:
                {
                    this.HandleOpenWorkspace(reader);
                    break;
                }
                case Messages.EditServer.CloseWorkspace:
                {
                    this.HandleCloseWorkspace(reader);
                    break;
                }
                case Messages.EditServer.EditWorkspaceCommand:
                {
                    this.HandleEditWorkspaceCommand(reader);
                    break;
                }
                case Messages.EditServer.SaveWorkspace:
                {
                    this.HandleSaveWorkspace(reader);
                    break;
                }
                case Messages.EditServer.SaveWorkspaceReply:
                {
                    this.HandleSaveWorkspaceReply(reader);
                    break;
                }
                case Messages.EditServer.BuildWorkspace:
                {
                    this.HandleBuildWorkspace(reader);
                    break;
                }
                case Messages.EditServer.UpdateWorkspaceClientBuiltBakeData:
                {
                    this.HandleUpdateWorkspaceClientBuiltBakeData(reader);
                    break;
                }
                case Messages.EditServer.BuildWorkspaceCompileReply:
                {
                    this.HandleBuildWorkspaceCompileReply(reader);
                    break;
                }
                case Messages.EditServer.BuildWorkspaceProgressUpdate:
                {
                    this.HandleBuildWorkspaceProgressUpdate(reader);
                    break;
                }
                case Messages.EditServer.BuildWorkspaceUploadReply:
                {
                    this.HandleBuildWorkspaceUploadReply(reader);
                    break;
                }
                case Messages.EditServer.WorkspaceReadyReply:
                {
                    this.HandleWorkspaceReadyReply(reader);
                    break;
                }
                case Messages.EditServer.SaveWorkspaceSelectionToInventory:
                {
                    this.HandleSaveWorkspaceSelectionToInventory(reader);
                    break;
                }
                case Messages.EditServer.SaveWorkspaceSelectionToInventoryReply:
                {
                    this.HandleSaveWorkspaceSelectionToInventoryReply(reader);
                    break;
                }
                case Messages.EditServer.InventoryCreateItem:
                {
                    this.HandleInventoryCreateItem(reader);
                    break;
                }
                case Messages.EditServer.InventoryDeleteItem:
                {
                    this.HandleInventoryDeleteItem(reader);
                    break;
                }
                case Messages.EditServer.InventoryChangeItemName:
                {
                    this.HandleInventoryChangeItemName(reader);
                    break;
                }
                case Messages.EditServer.InventoryChangeItemState:
                {
                    this.HandleInventoryChangeItemState(reader);
                    break;
                }
                case Messages.EditServer.InventoryModifyItemThumbnailAssetId:
                {
                    this.HandleInventoryModifyItemThumbnailAssetId(reader);
                    break;
                }
                case Messages.EditServer.InventoryModifyItemCapabilities:
                {
                    this.HandleInventoryModifyItemCapabilities(reader);
                    break;
                }
                case Messages.EditServer.InventorySaveItem:
                {
                    this.HandleInventorySaveItem(reader);
                    break;
                }
                case Messages.EditServer.InventoryUpdateItemReply:
                {
                    this.HandleInventoryUpdateItemReply(reader);
                    break;
                }
                case Messages.EditServer.InventoryItemUpload:
                {
                    this.HandleInventoryItemUpload(reader);
                    break;
                }
                case Messages.EditServer.InventoryItemUploadReply:
                {
                    this.HandleInventoryItemUploadReply(reader);
                    break;
                }
                case Messages.EditServer.InventoryCreateListing:
                {
                    this.HandleInventoryCreateListing(reader);
                    break;
                }
                case Messages.EditServer.InventoryCreateListingReply:
                {
                    this.HandleInventoryCreateListingReply(reader);
                    break;
                }
                case Messages.EditServer.BeginEditServerSpawn:
                {
                    this.HandleBeginEditServerSpawn(reader);
                    break;
                }
                case Messages.EditServer.EditServerSpawnReady:
                {
                    this.HandleEditServerSpawnReady(reader);
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

        void HandleOpenWorkspace(BinaryReader reader)
        {
            var packet = new OpenWorkspace(reader);
            OnOpenWorkspace?.Invoke(this, packet);
        }

        void HandleCloseWorkspace(BinaryReader reader)
        {
            var packet = new CloseWorkspace(reader);
            OnCloseWorkspace?.Invoke(this, packet);
        }

        void HandleEditWorkspaceCommand(BinaryReader reader)
        {
            var packet = new EditWorkspaceCommand(reader);
            OnEditWorkspaceCommand?.Invoke(this, packet);
        }

        void HandleSaveWorkspace(BinaryReader reader)
        {
            var packet = new SaveWorkspace(reader);
            OnSaveWorkspace?.Invoke(this, packet);
        }

        void HandleSaveWorkspaceReply(BinaryReader reader)
        {
            var packet = new SaveWorkspaceReply(reader);
            OnSaveWorkspaceReply?.Invoke(this, packet);
        }

        void HandleBuildWorkspace(BinaryReader reader)
        {
            var packet = new BuildWorkspace(reader);
            OnBuildWorkspace?.Invoke(this, packet);
        }

        void HandleUpdateWorkspaceClientBuiltBakeData(BinaryReader reader)
        {
            var packet = new UpdateWorkspaceClientBuiltBakeData(reader);
            OnUpdateWorkspaceClientBuiltBakeData?.Invoke(this, packet);
        }

        void HandleBuildWorkspaceCompileReply(BinaryReader reader)
        {
            var packet = new BuildWorkspaceCompileReply(reader);
            OnBuildWorkspaceCompileReply?.Invoke(this, packet);
        }

        void HandleBuildWorkspaceProgressUpdate(BinaryReader reader)
        {
            var packet = new BuildWorkspaceProgressUpdate(reader);
            OnBuildWorkspaceProgressUpdate?.Invoke(this, packet);
        }

        void HandleBuildWorkspaceUploadReply(BinaryReader reader)
        {
            var packet = new BuildWorkspaceUploadReply(reader);
            OnBuildWorkspaceUploadReply?.Invoke(this, packet);
        }

        void HandleWorkspaceReadyReply(BinaryReader reader)
        {
            var packet = new WorkspaceReadyReply(reader);
            OnWorkspaceReadyReply?.Invoke(this, packet);
        }

        void HandleSaveWorkspaceSelectionToInventory(BinaryReader reader)
        {
            var packet = new SaveWorkspaceSelectionToInventory(reader);
            OnSaveWorkspaceSelectionToInventory?.Invoke(this, packet);
        }

        void HandleSaveWorkspaceSelectionToInventoryReply(BinaryReader reader)
        {
            var packet = new SaveWorkspaceSelectionToInventoryReply(reader);
            OnSaveWorkspaceSelectionToInventoryReply?.Invoke(this, packet);
        }

        void HandleInventoryCreateItem(BinaryReader reader)
        {
            var packet = new InventoryCreateItem(reader);
            OnInventoryCreateItem?.Invoke(this, packet);
        }

        void HandleInventoryDeleteItem(BinaryReader reader)
        {
            var packet = new InventoryDeleteItem(reader);
            OnInventoryDeleteItem?.Invoke(this, packet);
        }

        void HandleInventoryChangeItemName(BinaryReader reader)
        {
            var packet = new InventoryChangeItemName(reader);
            OnInventoryChangeItemName?.Invoke(this, packet);
        }

        void HandleInventoryChangeItemState(BinaryReader reader)
        {
            var packet = new InventoryChangeItemState(reader);
            OnInventoryChangeItemState?.Invoke(this, packet);
        }

        void HandleInventoryModifyItemThumbnailAssetId(BinaryReader reader)
        {
            var packet = new InventoryModifyItemThumbnailAssetId(reader);
            OnInventoryModifyItemThumbnailAssetId?.Invoke(this, packet);
        }

        void HandleInventoryModifyItemCapabilities(BinaryReader reader)
        {
            var packet = new InventoryModifyItemCapabilities(reader);
            OnInventoryModifyItemCapabilities?.Invoke(this, packet);
        }

        void HandleInventorySaveItem(BinaryReader reader)
        {
            var packet = new InventorySaveItem(reader);
            OnInventorySaveItem?.Invoke(this, packet);
        }

        void HandleInventoryUpdateItemReply(BinaryReader reader)
        {
            var packet = new InventoryUpdateItemReply(reader);
            OnInventoryUpdateItemReply?.Invoke(this, packet);
        }

        void HandleInventoryItemUpload(BinaryReader reader)
        {
            var packet = new InventoryItemUpload(reader);
            OnInventoryItemUpload?.Invoke(this, packet);
        }

        void HandleInventoryItemUploadReply(BinaryReader reader)
        {
            var packet = new InventoryItemUploadReply(reader);
            OnInventoryItemUploadReply?.Invoke(this, packet);
        }

        void HandleInventoryCreateListing(BinaryReader reader)
        {
            var packet = new InventoryCreateListing(reader);
            OnInventoryCreateListing?.Invoke(this, packet);
        }

        void HandleInventoryCreateListingReply(BinaryReader reader)
        {
            var packet = new InventoryCreateListingReply(reader);
            OnInventoryCreateListingReply?.Invoke(this, packet);
        }

        void HandleBeginEditServerSpawn(BinaryReader reader)
        {
            var packet = new BeginEditServerSpawn(reader);
            OnBeginEditServerSpawn?.Invoke(this, packet);
        }

        void HandleEditServerSpawnReady(BinaryReader reader)
        {
            var packet = new EditServerSpawnReady(reader);
            OnEditServerSpawnReady?.Invoke(this, packet);
        }
    }
}
