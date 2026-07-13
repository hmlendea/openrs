using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using OpenRS.Models;
using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiInventoryPanel(GameClient client) : GuiControl
    {
        private static int Rows => 8;

        private static int Columns => 4;

        private GuiItemCard[] itemCards;
        protected override void DoLoadContent()
        {
            itemCards = new GuiItemCard[Rows * Columns];

            for (int slotIndex = 0; slotIndex < Rows * Columns; slotIndex += 1)
            {
                itemCards[slotIndex] = new GuiItemCard();
            }

            RegisterChildren(itemCards);
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();
            SetItems();
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            int spacingX = (Size.Width - Columns * itemCards[0].Size.Width) / (Columns + 1);
            int spacingY = (Size.Height - Rows * itemCards[0].Size.Height) / (Rows + 1);

            for (int slotIndex = 0; slotIndex < Rows * Columns; slotIndex += 1)
            {
                int columnIndex = slotIndex % Columns;
                int rowIndex = slotIndex / Columns;

                itemCards[slotIndex].Location = new Point2D(
                    spacingX * (columnIndex + 1) + itemCards[slotIndex].Size.Width * columnIndex,
                    spacingY * (rowIndex + 1) + itemCards[slotIndex].Size.Height * rowIndex);
            }
        }

        private void SetItems()
        {
            if (client.inventoryManager is null || client.entityManager is null)
            {
                return;
            }

            for (int itemSlot = 0; itemSlot < Rows * Columns; itemSlot += 1)
            {
                InventoryItem inventoryItem = client.inventoryManager.GetItem(itemSlot);
                Item item = client.entityManager.GetItem(inventoryItem.Index);

                itemCards[itemSlot].ItemPictureId = item.InventoryPicture;
                itemCards[itemSlot].Quantity = inventoryItem.Quantity;
            }
        }
    }
}
