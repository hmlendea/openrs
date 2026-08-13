using NUnit.Framework;

using Microsoft.Xna.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class StringDrawTests
    {
        [Test]
        public void GivenANewStringDraw_WhenReadingItsColour_ThenOpaqueRedIsUsed()
            => Assert.That(new StringDraw().ForeColour, Is.EqualTo(new Color(255, 0, 0, 255)));

        [Test]
        public void GivenStringDrawValues_WhenAssigningThem_ThenTheyAreRetained()
        {
            StringDraw draw = new()
            {
                Text = "RuneScape",
                DrawPosition = new Vector2(4, 8),
                ForeColour = Color.Yellow,
                Font = null!,
            };

            Assert.That(draw.Text, Is.EqualTo("RuneScape"));
            Assert.That(draw.DrawPosition, Is.EqualTo(new Vector2(4, 8)));
            Assert.That(draw.ForeColour, Is.EqualTo(Color.Yellow));
            Assert.That(draw.Font, Is.Null);
        }
    }
}