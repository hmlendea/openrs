using NUnit.Framework;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class AppearanceTests
    {
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void GivenAValidHeadSprite_WhenValidatingTheAppearance_ThenTheAppearanceIsValid(int head)
        {
            Appearance appearance = BuildValidAppearance();
            appearance.Head = head;

            Assert.That(appearance.IsValid);
        }

        [TestCase(2)]
        [TestCase(5)]
        public void GivenAValidBodySprite_WhenValidatingTheAppearance_ThenTheAppearanceIsValid(int body)
        {
            Appearance appearance = BuildValidAppearance();
            appearance.Body = body;

            Assert.That(appearance.IsValid);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(9, 14, 14, 4)]
        [TestCase(4, 8, 12, 2)]
        public void GivenValidColours_WhenValidatingTheAppearance_ThenTheAppearanceIsValid(
            int hairColour,
            int topColour,
            int trousersColour,
            int skinColour)
        {
            Appearance appearance = BuildValidAppearance();
            appearance.HairColour = hairColour;
            appearance.TopColour = topColour;
            appearance.TrousersColour = trousersColour;
            appearance.SkinColour = skinColour;

            Assert.That(appearance.IsValid);
        }

        [TestCase(0, 2, 4, 8, 12, 2)]
        [TestCase(9, 3, 4, 8, 12, 2)]
        [TestCase(9, 2, -1, 8, 12, 2)]
        [TestCase(9, 2, 10, 8, 12, 2)]
        [TestCase(9, 2, 4, -1, 12, 2)]
        [TestCase(9, 2, 4, 15, 12, 2)]
        [TestCase(9, 2, 4, 8, -1, 2)]
        [TestCase(9, 2, 4, 8, 15, 2)]
        [TestCase(9, 2, 4, 8, 12, -1)]
        [TestCase(9, 2, 4, 8, 12, 5)]
        public void GivenAnInvalidProperty_WhenValidatingTheAppearance_ThenTheAppearanceIsInvalid(
            int head,
            int body,
            int hairColour,
            int topColour,
            int trousersColour,
            int skinColour)
        {
            Appearance appearance = new()
            {
                Head = head,
                Body = body,
                HairColour = hairColour,
                TopColour = topColour,
                TrousersColour = trousersColour,
                SkinColour = skinColour
            };

            Assert.That(appearance.IsValid, Is.False);
        }

        [TestCase(0, 8)]
        [TestCase(1, 5)]
        [TestCase(2, 3)]
        [TestCase(-1, 0)]
        [TestCase(3, 0)]
        [TestCase(12, 0)]
        public void GivenASpritePosition_WhenRetrievingTheSprite_ThenTheCompatibleSpriteIsReturned(
            int position,
            int expectedSprite)
        {
            Appearance appearance = BuildValidAppearance();

            Assert.That(
                appearance.GetSprite(position),
                Is.EqualTo(expectedSprite));
        }

        [Test]
        public void GivenAnAppearance_WhenRetrievingAllSprites_ThenTheTwelveCompatibleSpritesAreReturned()
        {
            Appearance appearance = BuildValidAppearance();
            int[] expectedSprites = [8, 5, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0];

            Assert.That(
                appearance.GetSprites(),
                Is.EqualTo(expectedSprites));
        }

        [Test]
        public void GivenAnAppearance_WhenRetrievingSpritesTwice_ThenTheArraysAreIndependent()
        {
            Appearance appearance = BuildValidAppearance();

            int[] firstSprites = appearance.GetSprites();
            int[] secondSprites = appearance.GetSprites();
            firstSprites[0] = 42;

            Assert.That(firstSprites, Is.Not.SameAs(secondSprites));
            Assert.That(secondSprites[0], Is.EqualTo(appearance.Head));
            Assert.That(appearance.GetSprite(0), Is.EqualTo(appearance.Head));
        }

        [Test]
        public void GivenChangedHeadAndBodySprites_WhenRetrievingSprites_ThenCurrentValuesAreReturned()
        {
            Appearance appearance = BuildValidAppearance();
            appearance.Head = 1;
            appearance.Body = 2;

            Assert.That(appearance.GetSprite(0), Is.EqualTo(1));
            Assert.That(appearance.GetSprite(1), Is.EqualTo(2));
            Assert.That(appearance.GetSprites()[..3], Is.EqualTo(new[] { 1, 2, 3 }));
        }

        private static Appearance BuildValidAppearance() => new()
        {
            Head = 8,
            Body = 5,
            HairColour = 4,
            TopColour = 8,
            TrousersColour = 12,
            SkinColour = 2
        };
    }
}