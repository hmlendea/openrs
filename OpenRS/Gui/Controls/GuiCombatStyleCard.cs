using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiCombatStyleCard : GuiControl
    {
        private GuiImage background;
        private GuiImage icon;
        private GuiText nameText;

        public bool IsToggled { get; set; }

        public string Icon { get; set; }

        public string CombatStyleName { get; set; }

        protected override void DoLoadContent()
        {
            background = new GuiImage { ContentFile = "Interface/combatcard" };
            icon = new();
            nameText = new GuiText
            {
                FontName = "SkillCardFont",
                FontOutline = FontOutline.BottomRight,
                VerticalAlignment = Alignment.Beginning
            };

            RegisterChildren(background, icon, nameText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent()
        {
        }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void SetChildrenProperties()
        {
            background.Size = Size;
            background.Location = new(0, 0);

            if (IsToggled)
            {
                background.TintColour = Colour.Red;
            }
            else
            {
                background.TintColour = Colour.White;
            }

            icon.ContentFile = Icon;
            icon.Location = new(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - nameText.Size.Height - icon.Size.Height) / 2);

            nameText.Size = new(Size.Width, 14);
            nameText.Location = new(0, Size.Height - nameText.Size.Height);
            nameText.Text = CombatStyleName;
            nameText.ForegroundColour = ForegroundColour;
        }
    }
}
