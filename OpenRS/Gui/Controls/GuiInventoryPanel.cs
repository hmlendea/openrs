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
        private GuiItemCard[] itemCards;

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
            SetChildrenProperties();
        }

        protected override void DoUnloadContent()
        {
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();
            SetItems();
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
    }
}
