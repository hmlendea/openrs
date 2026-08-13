using System;

using NUnit.Framework;

using OpenRS.Localisation;

namespace OpenRS.UnitTests.Localisation
{
    [TestFixture]
    public sealed class LanguageTests
    {
        [Test]
        public void GivenTheEnglishLanguage_WhenReadingItsValue_ThenItsNameAndCodeRemainCompatible()
        {
            Assert.That(Language.English.Name, Is.EqualTo("English"));
            Assert.That(Language.English.Code, Is.EqualTo("en"));
            Assert.That(Language.English.ToString(), Is.EqualTo("English"));
        }

        [Test]
        public void GivenTheLanguageValues_WhenReadingThem_ThenEnglishIsTheOnlyRegisteredInstance()
        {
            Language[] values = (Language[])Language.GetValues();

            Assert.That(values, Has.Length.EqualTo(1));
            Assert.That(values[0], Is.SameAs(Language.English));
        }

        [Test]
        public void GivenRepeatedValueRequests_WhenReadingThem_ThenIndependentArraySnapshotsAreReturned()
        {
            Array firstValues = Language.GetValues();
            Array secondValues = Language.GetValues();

            Assert.That(firstValues, Is.Not.SameAs(secondValues));
            Assert.That(firstValues, Is.EqualTo(secondValues));
        }

        [TestCase("English")]
        public void GivenARegisteredName_WhenParsingIt_ThenTheRegisteredInstanceIsReturned(string name)
            => Assert.That(Language.FromString(name), Is.SameAs(Language.English));

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("english")]
        [TestCase("ENGLISH")]
        [TestCase("Romanian")]
        [TestCase("Klingon")]
        public void GivenAnUnregisteredName_WhenParsingIt_ThenEnglishIsReturned(string name)
            => Assert.That(Language.FromString(name), Is.SameAs(Language.English));

        [Test]
        public void GivenANullName_WhenParsingIt_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => Language.FromString(null!),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void GivenTheSameLanguage_WhenComparingTypedValues_ThenTheyAreEqual()
            => Assert.That(Language.English.Equals(Language.English));

        [Test]
        public void GivenANullLanguage_WhenComparingTypedValues_ThenTheyAreNotEqual()
            => Assert.That(Language.English.Equals((Language)null!), Is.False);

        [Test]
        public void GivenTheSameLanguage_WhenComparingObjects_ThenTheyAreEqual()
            => Assert.That(Language.English.Equals((object)Language.English));

        [Test]
        public void GivenANullObject_WhenComparingIt_ThenItIsNotEqual()
            => Assert.That(Language.English.Equals((object)null!), Is.False);

        [TestCase("English")]
        [TestCase(42)]
        [TestCase(true)]
        public void GivenAnObjectOfAnotherType_WhenComparingIt_ThenItIsNotEqual(object value)
            => Assert.That(Language.English.Equals(value), Is.False);

        [Test]
        public void GivenTheSameLanguage_WhenCalculatingItsHashCodeRepeatedly_ThenTheHashCodeIsStable()
        {
            int firstHashCode = Language.English.GetHashCode();
            int secondHashCode = Language.English.GetHashCode();

            Assert.That(firstHashCode, Is.EqualTo(secondHashCode));
        }

        [Test]
        public void GivenTheSameLanguageReferences_WhenUsingEqualityOperators_ThenTheyCompareEqual()
        {
            Assert.That(Language.English == Language.English);
            Assert.That(Language.English != Language.English, Is.False);
        }

        [Test]
        public void GivenANullAndEnglishLanguage_WhenUsingEqualityOperators_ThenTheyCompareUnequal()
        {
            Language nullLanguage = null!;

            Assert.That(nullLanguage == Language.English, Is.False);
            Assert.That(Language.English == nullLanguage, Is.False);
            Assert.That(nullLanguage != Language.English);
            Assert.That(Language.English != nullLanguage);
        }

        [Test]
        public void GivenTwoNullLanguages_WhenUsingEqualityOperators_ThenTheyCompareEqual()
        {
            Language firstLanguage = null!;
            Language secondLanguage = null!;

            Assert.That(firstLanguage == secondLanguage);
            Assert.That(firstLanguage != secondLanguage, Is.False);
        }

        [Test]
        public void GivenEnglish_WhenConvertingItToText_ThenItsNameIsReturned()
        {
            string languageName = Language.English;

            Assert.That(languageName, Is.EqualTo("English"));
        }

        [Test]
        public void GivenANullLanguage_WhenConvertingItToText_ThenANullReferenceExceptionIsThrown()
        {
            Language language = null!;

            Assert.That(
                () => ConvertToString(language),
                Throws.TypeOf<NullReferenceException>());
        }

        private static string ConvertToString(Language language) => language;
    }
}