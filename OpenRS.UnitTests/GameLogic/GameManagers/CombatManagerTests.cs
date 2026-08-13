using NUnit.Framework;

using OpenRS.GameLogic.GameManagers;
using OpenRS.Models;

namespace OpenRS.UnitTests.GameLogic.GameManagers
{
    [TestFixture]
    public sealed class CombatManagerTests
    {
        private CombatManager combatManager = null!;
        private InventoryManager inventoryManager = null!;

        [SetUp]
        public void SetUp()
        {
            inventoryManager = new InventoryManager(null!);
            inventoryManager.LoadContent();
            combatManager = new CombatManager(inventoryManager);
        }

        [TestCase(RuneElement.Air, (int)ElementalStaff.Air)]
        [TestCase(RuneElement.Air, (int)ElementalBattlestaff.Air)]
        [TestCase(RuneElement.Air, (int)ElementalMysticStaff.Air)]
        [TestCase(RuneElement.Water, (int)ElementalStaff.Water)]
        [TestCase(RuneElement.Water, (int)ElementalBattlestaff.Water)]
        [TestCase(RuneElement.Water, (int)ElementalMysticStaff.Water)]
        [TestCase(RuneElement.Earth, (int)ElementalStaff.Earth)]
        [TestCase(RuneElement.Earth, (int)ElementalBattlestaff.Earth)]
        [TestCase(RuneElement.Earth, (int)ElementalMysticStaff.Earth)]
        [TestCase(RuneElement.Fire, (int)ElementalStaff.Fire)]
        [TestCase(RuneElement.Fire, (int)ElementalBattlestaff.Fire)]
        [TestCase(RuneElement.Fire, (int)ElementalMysticStaff.Fire)]
        public void GivenTheCorrespondingEquippedStaff_WhenCheckingRunes_ThenAnyQuantityIsCovered(
            RuneElement runeElement,
            int staffItemIdentifier)
        {
            SetEquippedItem(staffItemIdentifier);

            Assert.That(combatManager.HasRequiredRunes((int)runeElement, int.MaxValue));
        }

        [TestCase(RuneElement.Air, (int)ElementalStaff.Water)]
        [TestCase(RuneElement.Water, (int)ElementalStaff.Earth)]
        [TestCase(RuneElement.Earth, (int)ElementalStaff.Fire)]
        [TestCase(RuneElement.Fire, (int)ElementalStaff.Air)]
        public void GivenAnotherElementalStaff_WhenCheckingRunes_ThenTheElementIsNotCovered(
            RuneElement runeElement,
            int staffItemIdentifier)
        {
            SetEquippedItem(staffItemIdentifier);

            Assert.That(combatManager.HasRequiredRunes((int)runeElement, 1), Is.False);
        }

        [TestCase(RuneElement.Air)]
        [TestCase(RuneElement.Water)]
        [TestCase(RuneElement.Earth)]
        [TestCase(RuneElement.Fire)]
        public void GivenAnUnequippedCorrespondingStaff_WhenCheckingRunes_ThenTheElementIsNotCovered(
            RuneElement runeElement)
        {
            int staffItemIdentifier = GetElementalStaffIdentifier(runeElement);
            inventoryManager.SetItem(0, staffItemIdentifier);
            inventoryManager.SetItemEquippedStatus(0, false);
            inventoryManager.InventoryItemsCount = 1;

            Assert.That(combatManager.HasRequiredRunes((int)runeElement, 1), Is.False);
        }

        [TestCase((int)RuneElement.Air, 0)]
        [TestCase((int)RuneElement.Water, -1)]
        [TestCase(42, 0)]
        [TestCase(int.MinValue, int.MinValue)]
        public void GivenNoMatchingInventoryItem_WhenTheRequiredCountIsNonPositive_ThenTheRequirementIsMet(
            int itemIdentifier,
            int requiredCount)
            => Assert.That(combatManager.HasRequiredRunes(itemIdentifier, requiredCount));

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(int.MaxValue)]
        public void GivenNoMatchingInventoryItem_WhenTheRequiredCountIsPositive_ThenTheRequirementIsNotMet(
            int itemIdentifier)
            => Assert.That(combatManager.HasRequiredRunes(itemIdentifier, 1), Is.False);

        private static int GetElementalStaffIdentifier(RuneElement runeElement) => runeElement switch
        {
            RuneElement.Air => (int)ElementalStaff.Air,
            RuneElement.Water => (int)ElementalStaff.Water,
            RuneElement.Earth => (int)ElementalStaff.Earth,
            RuneElement.Fire => (int)ElementalStaff.Fire,
            _ => 0,
        };

        private void SetEquippedItem(int itemIdentifier)
        {
            inventoryManager.SetItem(0, itemIdentifier);
            inventoryManager.SetItemEquippedStatus(0, true);
            inventoryManager.InventoryItemsCount = 1;
        }
    }
}