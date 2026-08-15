using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Models;
using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiInventoryPanel(GameClient client) : GuiControl
    {
        private static int NoPendingInventorySlot => -1;

        private GuiItemCard[] itemCards;
        private int pendingInventorySlotIndex = NoPendingInventorySlot;
        private MouseButton pendingMouseButton;
        private Point2D pendingMenuLocation;

        protected override void DoLoadContent()
        {
            itemCards = new GuiItemCard[GuiInventoryGridLayout.SlotCount];

            for (int slotIndex = 0;
                 slotIndex < GuiInventoryGridLayout.SlotCount;
                 slotIndex += 1)
            {
                itemCards[slotIndex] = new GuiItemCard();
            }

            RegisterChildren(itemCards);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent()
        {
            UnregisterEvents();
            client.HoveredInventorySlotIndex = null;
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();
            SetItems();
            SynchroniseHoveredInventorySlot();
            ProcessPendingInteraction();
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void SetChildrenProperties()
        {
            Size2D itemCardSize = GuiInventoryGridLayout.CalculateItemCardSize(Size);

            for (int slotIndex = 0;
                 slotIndex < GuiInventoryGridLayout.SlotCount;
                 slotIndex += 1)
            {
                itemCards[slotIndex].Size = itemCardSize;
                itemCards[slotIndex].Location =
                    GuiInventoryGridLayout.CalculateItemCardLocation(Size, slotIndex);
            }
        }

        private void SetItems()
        {
            for (int slotIndex = 0;
                 slotIndex < GuiInventoryGridLayout.SlotCount;
                 slotIndex += 1)
            {
                if (client.entityManager is null || slotIndex >= client.inventoryItemsCount)
                {
                    ClearItem(slotIndex);
                    continue;
                }

                int itemIndex = client.inventoryItems[slotIndex];
                Item item = client.entityManager.GetItem(itemIndex);

                itemCards[slotIndex].SpriteName = item.SpriteName;
                itemCards[slotIndex].Quantity = client.inventoryItemCount[slotIndex];
            }
        }

        private void ClearItem(int slotIndex)
        {
            itemCards[slotIndex].SpriteName = null;
            itemCards[slotIndex].Quantity = 0;
        }

        private void RegisterEvents()
        {
            foreach (GuiItemCard itemCard in itemCards)
            {
                itemCard.MouseButtonPressed += OnItemCardMouseButtonPressed;
                itemCard.MouseEntered += OnItemCardMouseEntered;
                itemCard.MouseLeft += OnItemCardMouseLeft;
            }
        }

        private void UnregisterEvents()
        {
            foreach (GuiItemCard itemCard in itemCards)
            {
                itemCard.MouseButtonPressed -= OnItemCardMouseButtonPressed;
                itemCard.MouseEntered -= OnItemCardMouseEntered;
                itemCard.MouseLeft -= OnItemCardMouseLeft;
            }
        }

        private void OnItemCardMouseButtonPressed(object sender, MouseButtonEventArgs eventArgs)
        {
            int inventorySlotIndex = GetInventorySlotIndex(sender);

            if (client.entityManager is null ||
                inventorySlotIndex < 0 ||
                inventorySlotIndex >= client.inventoryItemsCount ||
                (!Equals(eventArgs.Button, MouseButton.Left) &&
                 !Equals(eventArgs.Button, MouseButton.Right)))
            {
                return;
            }

            pendingInventorySlotIndex = inventorySlotIndex;
            pendingMouseButton = eventArgs.Button;
            pendingMenuLocation = eventArgs.Location;
        }

        private void OnItemCardMouseEntered(object sender, MouseEventArgs eventArgs)
        {
            int inventorySlotIndex = GetInventorySlotIndex(sender);

            if (client.entityManager is null ||
                inventorySlotIndex < 0 ||
                inventorySlotIndex >= client.inventoryItemsCount)
            {
                return;
            }

            client.HoveredInventorySlotIndex = inventorySlotIndex;
        }

        private void OnItemCardMouseLeft(object sender, MouseEventArgs eventArgs)
        {
            int inventorySlotIndex = GetInventorySlotIndex(sender);

            if (Equals(client.HoveredInventorySlotIndex, inventorySlotIndex))
            {
                client.HoveredInventorySlotIndex = null;
            }
        }

        private int GetInventorySlotIndex(object sender)
            => Array.IndexOf(itemCards, sender as GuiItemCard);

        private void SynchroniseHoveredInventorySlot()
        {
            if (!IsVisible ||
                client.entityManager is null ||
                (client.HoveredInventorySlotIndex.HasValue &&
                 client.HoveredInventorySlotIndex.Value >= client.inventoryItemsCount))
            {
                client.HoveredInventorySlotIndex = null;
            }
        }

        private void ProcessPendingInteraction()
        {
            if (pendingInventorySlotIndex == NoPendingInventorySlot ||
                pendingMouseButton is null)
            {
                return;
            }

            int inventorySlotIndex = pendingInventorySlotIndex;
            MouseButton mouseButton = pendingMouseButton;
            Point2D menuLocation = pendingMenuLocation;
            pendingInventorySlotIndex = NoPendingInventorySlot;
            pendingMouseButton = null;
            pendingMenuLocation = Point2D.Empty;

            if (client.entityManager is null ||
                inventorySlotIndex >= client.inventoryItemsCount)
            {
                return;
            }

            if (Equals(mouseButton, MouseButton.Left))
            {
                client.ActivateInventorySlot(
                    inventorySlotIndex,
                    menuLocation.X,
                    menuLocation.Y);
                return;
            }

            client.OpenInventorySlotMenu(
                inventorySlotIndex,
                menuLocation.X,
                menuLocation.Y);
        }
    }
}
