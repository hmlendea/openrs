using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSideBarPanel : GuiControl
    {
        private GuiImage background;

        protected override void DoLoadContent()
        {
            background = new GuiImage
            {
                ContentFile = "Interface/SideBar/panel"
            };

            RegisterChild(background);
        }
        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) { }

        protected override void DoDraw(SpriteBatch spriteBatch) { }
    }
}
