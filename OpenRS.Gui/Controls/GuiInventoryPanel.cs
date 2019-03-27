using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using OpenRS.Net.Client;
using OpenRS.Models;

namespace OpenRS.Gui.Controls
{
    public class GuiInventoryPanel : GuiControl
    {
        readonly GameClient client;

        GuiItemCard[] itemCards;

        const int Rows = 8;
        const int Columns = 4;

        public GuiInventoryPanel(GameClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            itemCards = new GuiItemCard[Rows * Columns];

            for (int i = 0; i < Rows * Columns; i++)
            {
                itemCards[i] = new GuiItemCard();
            }

            RegisterChildren(itemCards);
            SetChildrenProperties();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {

        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();
            SetItems();
        }

        /// <summary>
        /// Draw the content on the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }

        void SetChildrenProperties()
        {
            int spacingX = (Size.Width - Columns * itemCards[0].Size.Width) / (Columns + 1);
            int spacingY = (Size.Height - Rows * itemCards[0].Size.Height) / (Rows + 1);

            for (int i = 0; i < Rows * Columns; i++)
            {
                int x = i % Columns;
                int y = i / Columns;

                itemCards[i].Location = new Point2D(
                    Location.X + spacingX * (x + 1) + itemCards[i].Size.Width * x,
                    Location.Y + spacingY * (y + 1) + itemCards[i].Size.Height * y);
            }
        }

        void SetItems()
        {
            for (int itemSlot = 0; itemSlot < Rows * Columns; itemSlot++)
            {
                InventoryItem inventoryItem = client.inventoryManager.GetItem(itemSlot);
                Item item = client.entityManager.GetItem(inventoryItem.Index);

                itemCards[itemSlot].ItemPictureId = item.InventoryPicture;
                itemCards[itemSlot].Quantity = inventoryItem.Quantity;
            }
        }
    }
}
