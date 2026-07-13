using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.SpriteEffects;
using NuciXNA.Gui;
using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Screens
{
    public sealed class SplashScreen : Screen
    {
        public float Delay { get; set; }

        private GuiImage backgroundImage;
        private GuiImage overlayImage;
        private GuiImage logoImage;
        public SplashScreen()
        {
            Delay = 3;
            BackgroundColour = Colour.DodgerBlue;
        }
        protected override void DoLoadContent()
        {
            backgroundImage = new GuiImage
            {
                Id = nameof(backgroundImage),
                ContentFile = "SplashScreen/Background",
                RotationEffect = new OscilationEffect
                {
                    Speed = 0.25f,
                    MinimumMultiplier = 0.5f,
                    MaximumMultiplier = 1.5f
                },
                ScaleEffect = new ZoomEffect
                {
                    MinimumMultiplier = 0.5f,
                    MaximumMultiplier = 1.5f
                },
                AreEffectsActive = true
            };
            overlayImage = new GuiImage
            {
                Id = nameof(overlayImage),
                ContentFile = "SplashScreen/Overlay"
            };
            logoImage = new GuiImage
            {
                Id = nameof(logoImage),
                ContentFile = "SplashScreen/Logo"
            };

            GuiManager.Instance.RegisterControls(backgroundImage, overlayImage, logoImage);

            RegisterEvents();
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();

            if (!backgroundImage.RotationEffect.IsActive)
            {
                backgroundImage.RotationEffect.Activate();
                backgroundImage.ScaleEffect.Activate();
            }

            if (Delay <= 0 && !ScreenManager.Instance.Transitioning)
            {
                ChangeScreens();
            }

            Delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void RegisterEvents()
        {
            KeyPressed += OnKeyPressed;
            MouseButtonPressed += OnMouseButtonPressed;
        }

        private void UnregisterEvents()
        {
            KeyPressed -= OnKeyPressed;
            MouseButtonPressed -= OnMouseButtonPressed;
        }

        private void SetChildrenProperties()
        {
            overlayImage.Size = ScreenManager.Instance.Size;

            backgroundImage.Location = new Point2D(
                (ScreenManager.Instance.Size.Width - backgroundImage.ClientRectangle.Width) / 2,
                (ScreenManager.Instance.Size.Height - backgroundImage.ClientRectangle.Height) / 2);

            logoImage.Location = new Point2D(
                (ScreenManager.Instance.Size.Width - logoImage.Size.Width) / 2,
                (ScreenManager.Instance.Size.Height - logoImage.Size.Height) / 2);
        }

        private void OnKeyPressed(object sender, KeyboardKeyEventArgs e) => ChangeScreens();

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e) => ChangeScreens();

        private void ChangeScreens() => ScreenManager.Instance.ChangeScreens(typeof(TitleScreen));
    }
}
