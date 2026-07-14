using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Models;
using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiCombatPanel : GuiControl
    {
        private static int Spacing => 12;

        private readonly GameClient client;

        private GuiText combatLevelText;

        private GuiCombatStyleCard controlledStyleCard;
        private GuiCombatStyleCard aggressiveStyleCard;
        private GuiCombatStyleCard accurateStyleCard;
        private GuiCombatStyleCard defensiveStyleCard;

        public GuiCombatPanel(GameClient client)
        {
            this.client = client;

            ForegroundColour = Colour.Gold;
        }

        protected override void DoLoadContent()
        {
            combatLevelText = new GuiText { Size = new(Size.Width, 24) };
            controlledStyleCard = new GuiCombatStyleCard
            {
                Size = new(72, 48),
                CombatStyleName = "Controlled",
                Icon = "Icons/CombatStyles/controlled"
            };
            aggressiveStyleCard = new GuiCombatStyleCard
            {
                Size = new(72, 48),
                CombatStyleName = "Aggressive",
                Icon = "Icons/CombatStyles/aggressive"
            };
            accurateStyleCard = new GuiCombatStyleCard
            {
                Size = new(72, 48),
                CombatStyleName = "Accurate",
                Icon = "Icons/CombatStyles/accurate"
            };
            defensiveStyleCard = new GuiCombatStyleCard
            {
                Size = new(72, 48),
                CombatStyleName = "Defensive",
                Icon = "Icons/CombatStyles/defensive"
            };

            RegisterChildren(
                combatLevelText,
                controlledStyleCard,
                aggressiveStyleCard,
                accurateStyleCard,
                defensiveStyleCard);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void RegisterEvents()
        {
            controlledStyleCard.Clicked += ControlledStyleCard_Clicked;
            aggressiveStyleCard.Clicked += AggressiveStyleCard_Clicked;
            accurateStyleCard.Clicked += AccurateStyleCard_Clicked;
            defensiveStyleCard.Clicked += DefensiveStyleCard_Clicked;
        }

        private void UnregisterEvents()
        {
            controlledStyleCard.Clicked -= ControlledStyleCard_Clicked;
            aggressiveStyleCard.Clicked -= AggressiveStyleCard_Clicked;
            accurateStyleCard.Clicked -= AccurateStyleCard_Clicked;
            defensiveStyleCard.Clicked -= DefensiveStyleCard_Clicked;
        }

        private void SetChildrenProperties()
        {
            combatLevelText.Size = new(Size.Width, combatLevelText.Size.Height);
            combatLevelText.Location = new(0, Spacing);
            combatLevelText.ForegroundColour = ForegroundColour;

            controlledStyleCard.Location = new(
                (Size.Width - controlledStyleCard.Size.Width * 2 - Spacing) / 2,
                combatLevelText.ClientRectangle.Bottom + Spacing);
            aggressiveStyleCard.Location = new(
                controlledStyleCard.ClientRectangle.Right + Spacing,
                controlledStyleCard.ClientRectangle.Top);
            accurateStyleCard.Location = new(
                controlledStyleCard.ClientRectangle.Left,
                controlledStyleCard.ClientRectangle.Bottom + Spacing);
            defensiveStyleCard.Location = new(
                accurateStyleCard.ClientRectangle.Right + Spacing,
                accurateStyleCard.ClientRectangle.Top);

            controlledStyleCard.ForegroundColour = ForegroundColour;
            aggressiveStyleCard.ForegroundColour = ForegroundColour;
            accurateStyleCard.ForegroundColour = ForegroundColour;
            defensiveStyleCard.ForegroundColour = ForegroundColour;

            if (client is not null && client.loggedIn && client.CurrentPlayer is not null) // TODO: Ugly fix.
            {
                combatLevelText.Text = $"Combat Level: {client.CurrentPlayer.CombatLevel}";

                controlledStyleCard.IsToggled = false;
                aggressiveStyleCard.IsToggled = false;
                accurateStyleCard.IsToggled = false;
                defensiveStyleCard.IsToggled = false;

                switch (client.CombatStyle)
                {
                    case CombatStyle.Controlled:
                        controlledStyleCard.IsToggled = true;
                        break;

                    case CombatStyle.Aggressive:
                        aggressiveStyleCard.IsToggled = true;
                        break;

                    case CombatStyle.Accurate:
                        accurateStyleCard.IsToggled = true;
                        break;

                    case CombatStyle.Defensive:
                        defensiveStyleCard.IsToggled = true;
                        break;
                }
            }
        }

        private void ControlledStyleCard_Clicked(object sender, MouseButtonEventArgs e) => client.SetCombatStyle(CombatStyle.Controlled);

        private void AggressiveStyleCard_Clicked(object sender, MouseButtonEventArgs e) => client.SetCombatStyle(CombatStyle.Aggressive);

        private void AccurateStyleCard_Clicked(object sender, MouseButtonEventArgs e) => client.SetCombatStyle(CombatStyle.Accurate);

        private void DefensiveStyleCard_Clicked(object sender, MouseButtonEventArgs e) => client.SetCombatStyle(CombatStyle.Defensive);
    }
}
