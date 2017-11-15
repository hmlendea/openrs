using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Net.Client;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiCombatPanel : GuiElement
    {
        const int Spacing = 12;

        GameClient client;

        GuiText combatLevelText;

        GuiCombatStyleCard controlledStyleCard;
        GuiCombatStyleCard aggressiveStyleCard;
        GuiCombatStyleCard accurateStyleCard;
        GuiCombatStyleCard defensiveStyleCard;

        public GuiCombatPanel()
        {
            ForegroundColour = Colour.Gold;
        }

        public override void LoadContent()
        {
            combatLevelText = new GuiText { Size = new Size2D(Size.Width, 24) };
            controlledStyleCard = new GuiCombatStyleCard
            {
                Size = new Size2D(72, 48),
                CombatStyleName = "Controlled",
                Icon = "Icons/CombatStyles/controlled"
            };
            aggressiveStyleCard = new GuiCombatStyleCard
            {
                Size = new Size2D(72, 48),
                CombatStyleName = "Aggressive",
                Icon = "Icons/CombatStyles/aggressive"
            };
            accurateStyleCard = new GuiCombatStyleCard
            {
                Size = new Size2D(72, 48),
                CombatStyleName = "Accurate",
                Icon = "Icons/CombatStyles/accurate"
            };
            defensiveStyleCard = new GuiCombatStyleCard
            {
                Size = new Size2D(72, 48),
                CombatStyleName = "Defensive",
                Icon = "Icons/CombatStyles/defensive"
            };

            Children.Add(combatLevelText);
            Children.Add(controlledStyleCard);
            Children.Add(aggressiveStyleCard);
            Children.Add(accurateStyleCard);
            Children.Add(defensiveStyleCard);

            base.LoadContent();

            LinkEvents();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            UnlinkEvents();
        }

        public void AssociateGameClient(ref GameClient client)
        {
            this.client = client;
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            combatLevelText.Size = new Size2D(Size.Width, combatLevelText.Size.Height);
            combatLevelText.Location = new Point2D(Location.X, Location.Y + Spacing);
            combatLevelText.ForegroundColour = ForegroundColour;

            controlledStyleCard.Location = new Point2D(
                Location.X + (Size.Width - controlledStyleCard.Size.Width * 2 - Spacing) / 2,
                combatLevelText.ClientRectangle.Bottom + Spacing);
            aggressiveStyleCard.Location = new Point2D(
                controlledStyleCard.ClientRectangle.Right + Spacing,
                controlledStyleCard.ClientRectangle.Top);
            accurateStyleCard.Location = new Point2D(
                controlledStyleCard.ClientRectangle.Left,
                controlledStyleCard.ClientRectangle.Bottom + Spacing);
            defensiveStyleCard.Location = new Point2D(
                accurateStyleCard.ClientRectangle.Right + Spacing,
                accurateStyleCard.ClientRectangle.Top);

            controlledStyleCard.ForegroundColour = ForegroundColour;
            aggressiveStyleCard.ForegroundColour = ForegroundColour;
            accurateStyleCard.ForegroundColour = ForegroundColour;
            defensiveStyleCard.ForegroundColour = ForegroundColour;

            if (client != null && client.loggedIn) // TODO: Ugly fix
            {
                combatLevelText.Text = $"Combat Level: {client.CurrentPlayer.CombatLevel}";

                controlledStyleCard.IsToggled = false;
                aggressiveStyleCard.IsToggled = false;
                accurateStyleCard.IsToggled = false;
                defensiveStyleCard.IsToggled = false;

                switch (client.CombatStyle)
                {
                    case 0:
                        controlledStyleCard.IsToggled = true;
                        break;

                    case 1:
                        aggressiveStyleCard.IsToggled = true;
                        break;

                    case 2:
                        accurateStyleCard.IsToggled = true;
                        break;

                    case 3:
                        defensiveStyleCard.IsToggled = true;
                        break;
                }
            }
        }

        void LinkEvents()
        {
            controlledStyleCard.Clicked += ControlledStyleCard_Clicked;
            aggressiveStyleCard.Clicked += AggressiveStyleCard_Clicked;
            accurateStyleCard.Clicked += AccurateStyleCard_Clicked;
            defensiveStyleCard.Clicked += DefensiveStyleCard_Clicked;
        }

        void UnlinkEvents()
        {
            controlledStyleCard.Clicked -= ControlledStyleCard_Clicked;
            aggressiveStyleCard.Clicked -= AggressiveStyleCard_Clicked;
            accurateStyleCard.Clicked -= AccurateStyleCard_Clicked;
            defensiveStyleCard.Clicked -= DefensiveStyleCard_Clicked;
        }

        void ControlledStyleCard_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            client.CombatStyle = 0;
        }

        void AggressiveStyleCard_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            client.CombatStyle = 1;
        }

        void AccurateStyleCard_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            client.CombatStyle = 2;
        }

        void DefensiveStyleCard_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            client.CombatStyle = 3;
        }
    }
}
