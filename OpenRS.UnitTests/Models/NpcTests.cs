using NUnit.Framework;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class NpcTests
    {
        [Test]
        public void GivenANewNpc_WhenReadingItsAppearance_ThenItHasAnIndependentInstance()
        {
            Npc firstNpc = new();
            Npc secondNpc = new();

            Assert.That(firstNpc.Appearance, Is.Not.Null);
            Assert.That(firstNpc.Appearance, Is.Not.SameAs(secondNpc.Appearance));
        }

        [Test]
        public void GivenANewNpc_WhenReadingItsSprites_ThenItHasTheProtocolSpriteCount()
        {
            Npc npc = new();

            Assert.That(npc.Sprites, Has.Length.EqualTo(Npc.SpriteCount));
            Assert.That(npc.Sprites, Has.All.Zero);
            Assert.That(Npc.SpriteCount, Is.EqualTo(12));
        }

        [Test]
        public void GivenTwoNpcs_WhenReadingTheirSprites_ThenTheirArraysAreIndependent()
        {
            Npc firstNpc = new();
            Npc secondNpc = new();

            firstNpc.Sprites[0] = 42;

            Assert.That(firstNpc.Sprites, Is.Not.SameAs(secondNpc.Sprites));
            Assert.That(secondNpc.Sprites[0], Is.Zero);
        }
    }
}