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

        private static int CombatLevelTextHeight => 24;

        private static int StyleCardWidth => 72;

        private static int StyleCardHeight => 48;

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
            combatLevelText = new GuiText { Size = new(Size.Width, CombatLevelTextHeight) };
            controlledStyleCard = CreateStyleCard("Controlled", "Icons/CombatStyles/controlled");
            aggressiveStyleCard = CreateStyleCard("Aggressive", "Icons/CombatStyles/aggressive");
            accurateStyleCard = CreateStyleCard("Accurate", "Icons/CombatStyles/accurate");
            defensiveStyleCard = CreateStyleCard("Defensive", "Icons/CombatStyles/defensive");

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

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private static GuiCombatStyleCard CreateStyleCard(string styleName, string icon) => new()
        {
            Size = new(StyleCardWidth, StyleCardHeight),
            CombatStyleName = styleName,
            Icon = icon
        };

        private void RegisterEvents()
        {
            controlledStyleCard.Clicked += OnControlledStyleCardClicked;
            aggressiveStyleCard.Clicked += OnAggressiveStyleCardClicked;
            accurateStyleCard.Clicked += OnAccurateStyleCardClicked;
            defensiveStyleCard.Clicked += OnDefensiveStyleCardClicked;
        }

        private void UnregisterEvents()
        {
            controlledStyleCard.Clicked -= OnControlledStyleCardClicked;
            aggressiveStyleCard.Clicked -= OnAggressiveStyleCardClicked;
            accurateStyleCard.Clicked -= OnAccurateStyleCardClicked;
            defensiveStyleCard.Clicked -= OnDefensiveStyleCardClicked;
        }

        private void SetChildrenProperties()
        {
            UpdateCombatLevelText();
            UpdateCardLayouts();

            if (client.loggedIn && client.CurrentPlayer is not null)
            {
                UpdateCombatState();
            }
        }

        private void UpdateCombatLevelText()
        {
            combatLevelText.Size = new(Size.Width, combatLevelText.Size.Height);
            combatLevelText.Location = new(0, Spacing);
            combatLevelText.ForegroundColour = ForegroundColour;
        }

        private void UpdateCardLayouts()
        {
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
        }

        private void UpdateCombatState()
        {
            combatLevelText.Text = $"Combat Level: {client.CurrentPlayer.CombatLevel}";

            controlledStyleCard.IsToggled = client.CombatStyle == CombatStyle.Controlled;
            aggressiveStyleCard.IsToggled = client.CombatStyle == CombatStyle.Aggressive;
            accurateStyleCard.IsToggled = client.CombatStyle == CombatStyle.Accurate;
            defensiveStyleCard.IsToggled = client.CombatStyle == CombatStyle.Defensive;
        }

        private void OnControlledStyleCardClicked(object sender, MouseButtonEventArgs e)
            => client.SetCombatStyle(CombatStyle.Controlled);

        private void OnAggressiveStyleCardClicked(object sender, MouseButtonEventArgs e)
            => client.SetCombatStyle(CombatStyle.Aggressive);

        private void OnAccurateStyleCardClicked(object sender, MouseButtonEventArgs e)
            => client.SetCombatStyle(CombatStyle.Accurate);

        private void OnDefensiveStyleCardClicked(object sender, MouseButtonEventArgs e)
            => client.SetCombatStyle(CombatStyle.Defensive);
    }
}

