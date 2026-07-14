using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiCombatStyleCard : GuiControl
    {
        private static string BackgroundContentFile => "Interface/combatcard";

        private static string NameTextFontName => "SkillCardFont";

        private static int NameTextHeight => 14;

        private GuiImage background;
        private GuiImage icon;
        private GuiText nameText;

        public bool IsToggled { get; set; }

        public string Icon { get; set; }

        public string CombatStyleName { get; set; }

        protected override void DoLoadContent()
        {
            background = new GuiImage { ContentFile = BackgroundContentFile };
            icon = new GuiImage();
            nameText = new GuiText
            {
                FontName = NameTextFontName,
                FontOutline = FontOutline.BottomRight,
                VerticalAlignment = Alignment.Beginning
            };

            RegisterChildren(background, icon, nameText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            background.Size = Size;
            background.Location = Point2D.Empty;
            background.TintColour = Colour.White;

            if (IsToggled)
            {
                background.TintColour = Colour.Red;
            }

            icon.ContentFile = Icon;
            icon.Location = new(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - nameText.Size.Height - icon.Size.Height) / 2);

            nameText.Size = new(Size.Width, NameTextHeight);
            nameText.Location = new(0, Size.Height - nameText.Size.Height);
            nameText.Text = CombatStyleName;
            nameText.ForegroundColour = ForegroundColour;
        }
    }
}

