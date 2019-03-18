using NuciXNA.Gui.GuiElements;

namespace OpenRS.Gui.GuiElements
{
    public class GuiSideBarPanel : GuiElement
    {
        GuiImage background;

        public override void LoadContent()
        {
            background = new GuiImage
            {
                ContentFile = "Interface/SideBar/panel"
            };

            AddChild(background);

            base.LoadContent();
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            background.Location = Location;
            background.Size = Size;
        }
    }
}
