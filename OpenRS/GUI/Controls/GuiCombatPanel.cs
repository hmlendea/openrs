using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Input;
using NuciXNA.Primitives;
using NuciXNA.Gui.Controls;

using OpenRS.Models.Enumerations;
using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public class GuiCombatPanel : GuiControl
    {
        private const int Spacing = 12;

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

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
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

            RegisterChildren(
                combatLevelText,
                controlledStyleCard,
                aggressiveStyleCard,
                accurateStyleCard,
                defensiveStyleCard);
            RegisterEvents();
            SetChildrenProperties();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent() => UnregisterEvents();

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        /// <summary>
        /// Draw the content on the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
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
            combatLevelText.Size = new Size2D(Size.Width, combatLevelText.Size.Height);
            combatLevelText.Location = new Point2D(0, Spacing);
            combatLevelText.ForegroundColour = ForegroundColour;

            controlledStyleCard.Location = new Point2D(
                (Size.Width - controlledStyleCard.Size.Width * 2 - Spacing) / 2,
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

            if (client != null && client.loggedIn && client.CurrentPlayer != null) // TODO: Ugly fix
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
