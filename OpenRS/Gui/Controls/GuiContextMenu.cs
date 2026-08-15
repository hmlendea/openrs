using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Settings;

namespace OpenRS.Gui.Controls
{
    internal sealed class GuiContextMenu : GuiControl
    {
        private static int MaximumOptionCount => 20;

        private static int OptionHeight => GameDefines.GuiTileSize;

        private static int MenuWidth => GameDefines.SidePanelWidth;

        private GuiContextMenuOption[] options = [];
        private GuiButton[] optionButtons;

        internal Point2D AnchorScreenLocation { get; set; }

        internal IEnumerable<GuiContextMenuOption> Options
        {
            get => options;
            set => SetOptions(value);
        }

        internal GuiContextMenu() => Size = new Size2D(MenuWidth, 0);

        protected override void DoLoadContent()
        {
            optionButtons = new GuiButton[MaximumOptionCount];

            for (int optionIndex = 0;
                 optionIndex < MaximumOptionCount;
                 optionIndex += 1)
            {
                optionButtons[optionIndex] = new GuiButton
                {
                    Size = new Size2D(MenuWidth, OptionHeight),
                    ForegroundColour = Colour.White
                };
            }

            RegisterChildren(optionButtons);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void SetOptions(IEnumerable<GuiContextMenuOption> availableOptions)
        {
            if (availableOptions is null)
            {
                throw new ArgumentNullException(nameof(availableOptions));
            }

            GuiContextMenuOption[] newOptions = availableOptions.ToArray();
            ValidateOptions(newOptions);
            options = newOptions;
            Size = new Size2D(MenuWidth, options.Length * OptionHeight);

            if (IsContentLoaded)
            {
                SetChildrenProperties();
            }
        }

        private static void ValidateOptions(IEnumerable<GuiContextMenuOption> availableOptions)
        {
            int optionCount = availableOptions.Count();

            if (optionCount > MaximumOptionCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(availableOptions),
                    optionCount,
                    $"A context menu supports at most {MaximumOptionCount} options.");
            }

            if (availableOptions.Any(option =>
                option is null ||
                string.IsNullOrWhiteSpace(option.Text) ||
                option.SelectedAction is null))
            {
                throw new ArgumentException(
                    "Every context menu option must contain text and a selected action.",
                    nameof(availableOptions));
            }
        }

        private void RegisterEvents()
        {
            foreach (GuiButton optionButton in optionButtons)
            {
                optionButton.Clicked += OnOptionButtonClicked;
            }

            InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
        }

        private void UnregisterEvents()
        {
            foreach (GuiButton optionButton in optionButtons)
            {
                optionButton.Clicked -= OnOptionButtonClicked;
            }

            InputManager.Instance.MouseButtonPressed -= OnMouseButtonPressed;
        }

        private void SetChildrenProperties()
        {
            UpdateLocation();

            for (int optionIndex = 0;
                 optionIndex < MaximumOptionCount;
                 optionIndex += 1)
            {
                GuiButton optionButton = optionButtons[optionIndex];
                optionButton.Location = new Point2D(0, optionIndex * OptionHeight);
                optionButton.Size = new Size2D(MenuWidth, OptionHeight);

                if (optionIndex < options.Length)
                {
                    optionButton.Text = options[optionIndex].Text;
                    optionButton.Show();
                }
                else
                {
                    optionButton.Text = string.Empty;
                    optionButton.Hide();
                }
            }
        }

        private void UpdateLocation()
        {
            if (Parent is null)
            {
                return;
            }

            int maximumScreenX = Math.Max(0, ScreenManager.Instance.Size.Width - Size.Width);
            int maximumScreenY = Math.Max(0, ScreenManager.Instance.Size.Height - Size.Height);
            int screenX = Math.Clamp(AnchorScreenLocation.X, 0, maximumScreenX);
            int screenY = Math.Clamp(AnchorScreenLocation.Y, 0, maximumScreenY);

            Location = new Point2D(
                screenX - Parent.ScreenLocation.X,
                screenY - Parent.ScreenLocation.Y);
        }

        private void OnOptionButtonClicked(object sender, MouseButtonEventArgs eventArgs)
        {
            int optionIndex = Array.IndexOf(optionButtons, sender as GuiButton);

            if (optionIndex < 0 || optionIndex >= options.Length)
            {
                return;
            }

            Action selectedAction = options[optionIndex].SelectedAction;
            Hide();
            selectedAction();
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs eventArgs)
        {
            if (!IsVisible || DisplayRectangle.Contains(eventArgs.Location))
            {
                return;
            }

            Hide();
        }
    }
}