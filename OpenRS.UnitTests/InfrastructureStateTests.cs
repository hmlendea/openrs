using System;
using System.IO;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Gui.Helpers;
using OpenRS.Localisation;
using OpenRS.Net.Client;
using OpenRS.Settings;

namespace OpenRS.UnitTests
{
    [TestFixture]
    [NonParallelizable]
    public sealed class InfrastructureStateTests
    {
        [Test]
        public void GivenGraphicsSettings_WhenConstructingThem_ThenCompatibleDefaultsAreUsed()
        {
            GraphicsSettings settings = new();

            Assert.That(settings.Resolution, Is.EqualTo(new Size2D(1024, 480)));
            Assert.That(settings.Fullscreen, Is.False);
            Assert.That(settings.FogOfWar);
            Assert.That(settings.ShowRoofs);
        }

        [Test]
        public void GivenGraphicsSettings_WhenAssigningValues_ThenTheyAreRetained()
        {
            GraphicsSettings settings = new()
            {
                Resolution = new Size2D(512, 256),
                Fullscreen = true,
                FogOfWar = false,
                ShowRoofs = false,
            };

            Assert.That(settings.Resolution, Is.EqualTo(new Size2D(512, 256)));
            Assert.That(settings.Fullscreen);
            Assert.That(settings.FogOfWar, Is.False);
            Assert.That(settings.ShowRoofs, Is.False);
        }

        [Test]
        public void GivenSettingsManagerInstances_WhenReadingTheSingleton_ThenItsIdentityAndStateAreRetained()
        {
            SettingsManager firstInstance = SettingsManager.Instance;
            SettingsManager secondInstance = SettingsManager.Instance;

            Assert.That(secondInstance, Is.SameAs(firstInstance));
            Assert.That(firstInstance.GraphicsSettings, Is.Not.Null);

            firstInstance.DebugMode = true;
            firstInstance.CameraAutoAngle = true;

            Assert.That(secondInstance.DebugMode);
            Assert.That(secondInstance.CameraAutoAngle);
        }

        [Test]
        public void GivenGameDefines_WhenReadingThem_ThenCompatibleDimensionsAndCountsAreReturned()
        {
            Assert.That(GameDefines.GuiTileSize, Is.EqualTo(32));
            Assert.That(GameDefines.SpellProjectileCount, Is.EqualTo(7));
            Assert.That(GameDefines.WindowWidth, Is.EqualTo(1024));
            Assert.That(GameDefines.WindowHeight, Is.EqualTo(576));
            Assert.That(GameDefines.GameViewportWidth, Is.EqualTo(768));
            Assert.That(GameDefines.SidePanelWidth, Is.EqualTo(256));
        }

        [Test]
        public void GivenConfig_WhenReadingIt_ThenCompatibleNetworkAndMembershipValuesAreReturned()
        {
            Assert.That(Config.ConfigurationDirectory, Does.EndWith($"Data{Path.DirectorySeparatorChar}"));
            Assert.That(Config.ClientVersion, Is.EqualTo(3));
            Assert.That(Config.MembersFeatures);
            Assert.That(Config.ServerIp, Is.EqualTo("127.0.0.1"));
            Assert.That(Config.ServerPort, Is.EqualTo(43594));
        }

        [Test]
        public void GivenApplicationPaths_WhenReadingThem_ThenEveryDirectoryUsesItsDeclaredParent()
        {
            Assert.That(ApplicationPaths.ApplicationDirectory, Is.Not.Empty);
            Assert.That(ApplicationPaths.ConfigurationDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.ApplicationDirectory, "Data")));
            Assert.That(ApplicationPaths.DataDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.ApplicationDirectory, "Data")));
            Assert.That(ApplicationPaths.AnimationsDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Animations")));
            Assert.That(ApplicationPaths.FontsDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Fonts")));
            Assert.That(ApplicationPaths.MediaDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Media")));
            Assert.That(ApplicationPaths.MapsDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Maps")));
            Assert.That(ApplicationPaths.ModelsDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Models")));
            Assert.That(ApplicationPaths.TexturesDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Textures")));
            Assert.That(ApplicationPaths.EntitiesDirectory, Is.EqualTo(Path.Combine(ApplicationPaths.DataDirectory, "Entities")));
            Assert.That(ApplicationPaths.SettingsFile, Is.EqualTo(Path.Combine(ApplicationPaths.UserDataDirectory, "Settings.xml")));
        }

        [Test]
        public void GivenAMissingLocalisationKey_WhenReadingIt_ThenTheKeyIsReturned()
            => Assert.That(
                LocalisationManager.GetString("test.missing_key"),
                Is.EqualTo("test.missing_key"));

        [Test]
        public void GivenFramerateSamples_WhenUpdatingBeyondCapacity_ThenTotalsAndAveragesAdvance()
        {
            FramerateCounter counter = FramerateCounter.Instance;
            long initialFrames = counter.TotalFrames;
            float initialSeconds = counter.TotalSeconds;

            for (int sampleIndex = 0; sampleIndex <= FramerateCounter.MaximumSamples; sampleIndex += 1)
            {
                counter.Update(0.5F);
            }

            Assert.That(counter.TotalFrames, Is.EqualTo(initialFrames + 101));
            Assert.That(counter.TotalSeconds, Is.EqualTo(initialSeconds + 50.5F).Within(0.001F));
            Assert.That(counter.CurrentFramesPerSecond, Is.EqualTo(2F));
            Assert.That(counter.AverageFramesPerSecond, Is.EqualTo(2F));
            Assert.That(FramerateCounter.Instance, Is.SameAs(counter));
        }

        [TestCase(false, 28)]
        [TestCase(true, 48)]
        public void GivenAFrameTranslationMode_WhenConstructingIt_ThenDimensionsAndOffsetAreStored(
            bool translate,
            int expectedOffset)
        {
            GameApplet applet = new();

            GameFrame frame = new(applet, 42, 64, "OpenRS", true, translate);

            Assert.That(frame.frameWidth, Is.EqualTo(42));
            Assert.That(frame.frameHeight, Is.EqualTo(64));
            Assert.That(frame.yOffset, Is.EqualTo(expectedOffset));
            Assert.That(frame.gameApplet, Is.SameAs(applet));
            Assert.That(applet.mouseYOffset, Is.Zero);
            Assert.That(() => frame.Resize(8, 16), Throws.Nothing);
            Assert.That(() => frame.Paint(null!), Throws.Nothing);
        }

        [Test]
        public void GivenWindowEventsAfterDestruction_WhenHandlingThem_ThenNoFurtherDestroyIsRequested()
        {
            GameApplet applet = new() { runStatus = -1 };
            GameFrame frame = new(applet, 42, 64, "OpenRS", true, false);

            Assert.That(() => frame.WindowClosed(EventArgs.Empty), Throws.Nothing);
            Assert.That(() => frame.WindowClosing(EventArgs.Empty), Throws.Nothing);
            Assert.That(applet.runStatus, Is.EqualTo(-1));
        }
    }
}