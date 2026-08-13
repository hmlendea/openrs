using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.GameLogic.GameManagers;
using OpenRS.Models;

namespace OpenRS.UnitTests.GameLogic.GameManagers
{
    [TestFixture]
    public sealed class InventoryManagerTests
    {
        private InventoryManager inventoryManager = null!;

        [SetUp]
        public void SetUp()
        {
            inventoryManager = new InventoryManager(null!);
            inventoryManager.LoadContent();
        }

        [Test]
        public void GivenTheInventoryContract_WhenReadingItsCapacities_ThenTheyRemainCompatible()
        {
            Assert.That(InventoryManager.MaximumInventorySize, Is.EqualTo(30));
            Assert.That(InventoryManager.MaximumBankSize, Is.EqualTo(48));
        }

        [Test]
        public void GivenLoadedContent_WhenReadingBoundarySlots_ThenEverySlotHasAnIndependentItem()
        {
            Assert.That(inventoryManager.GetItem(0), Is.Not.Null);
            Assert.That(inventoryManager.GetItem(34), Is.Not.Null);
            Assert.That(inventoryManager.GetItem(0), Is.Not.SameAs(inventoryManager.GetItem(1)));
            Assert.That(inventoryManager.GetBankItem(0), Is.Not.Null);
            Assert.That(inventoryManager.GetBankItem(255), Is.Not.Null);
            Assert.That(
                inventoryManager.GetBankItem(0),
                Is.Not.SameAs(inventoryManager.GetServerBankItem(0)));
        }

        [Test]
        public void GivenExistingCounts_WhenReloadingContent_ThenCountsAndItemsReturnToInitialState()
        {
            inventoryManager.InventoryItemsCount = 4;
            inventoryManager.BankItemsCount = 8;
            inventoryManager.ServerBankItemsCount = 16;
            inventoryManager.SetItem(0, 42);

            inventoryManager.LoadContent();

            Assert.That(inventoryManager.InventoryItemsCount, Is.Zero);
            Assert.That(inventoryManager.BankItemsCount, Is.Zero);
            Assert.That(inventoryManager.ServerBankItemsCount, Is.Zero);
            Assert.That(inventoryManager.GetItem(0).Index, Is.Zero);
        }

        [TestCase(0, int.MinValue)]
        [TestCase(4, -42)]
        [TestCase(16, 0)]
        [TestCase(32, 42)]
        [TestCase(34, int.MaxValue)]
        public void GivenAValidInventorySlot_WhenSettingAnItem_ThenItsIdentifierIsStored(
            int itemSlot,
            int itemIdentifier)
        {
            inventoryManager.SetItem(itemSlot, itemIdentifier);

            Assert.That(inventoryManager.GetItem(itemSlot).Index, Is.EqualTo(itemIdentifier));
        }

        [TestCase(0, int.MinValue)]
        [TestCase(4, -42)]
        [TestCase(16, 0)]
        [TestCase(32, 42)]
        [TestCase(34, int.MaxValue)]
        public void GivenAValidInventorySlot_WhenSettingAQuantity_ThenItsValueIsStored(
            int itemSlot,
            int quantity)
        {
            inventoryManager.SetItemCount(itemSlot, quantity);

            Assert.That(inventoryManager.GetItem(itemSlot).Quantity, Is.EqualTo(quantity));
        }

        [TestCase(0, false)]
        [TestCase(4, true)]
        [TestCase(16, false)]
        [TestCase(32, true)]
        [TestCase(34, false)]
        public void GivenAValidInventorySlot_WhenSettingEquipmentState_ThenItsValueIsStored(
            int itemSlot,
            bool isEquipped)
        {
            inventoryManager.SetItemEquippedStatus(itemSlot, isEquipped);

            Assert.That(inventoryManager.GetItem(itemSlot).IsEquipped, Is.EqualTo(isEquipped));
        }

        [TestCase(-1)]
        [TestCase(35)]
        [TestCase(42)]
        public void GivenAnInvalidInventorySlot_WhenReadingIt_ThenAnIndexExceptionIsThrown(int itemSlot)
            => Assert.That(
                () => inventoryManager.GetItem(itemSlot),
                Throws.TypeOf<IndexOutOfRangeException>());

        [TestCase(-1)]
        [TestCase(256)]
        [TestCase(512)]
        public void GivenAnInvalidBankSlot_WhenReadingIt_ThenAnIndexExceptionIsThrown(int itemSlot)
            => Assert.That(
                () => inventoryManager.GetBankItem(itemSlot),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenAnEquippedMatchingItemWithinTheActiveRange_WhenCheckingIt_ThenItIsEquipped()
        {
            SetInventoryItem(0, 42, 1, true);
            inventoryManager.InventoryItemsCount = 1;

            Assert.That(inventoryManager.IsItemEquipped(42));
        }

        [Test]
        public void GivenAMatchingUnequippedItem_WhenCheckingIt_ThenItIsNotEquipped()
        {
            SetInventoryItem(0, 42, 1, false);
            inventoryManager.InventoryItemsCount = 1;

            Assert.That(inventoryManager.IsItemEquipped(42), Is.False);
        }

        [Test]
        public void GivenAnEquippedMatchingItemBeyondTheActiveRange_WhenCheckingIt_ThenItIsIgnored()
        {
            SetInventoryItem(1, 42, 1, true);
            inventoryManager.InventoryItemsCount = 1;

            Assert.That(inventoryManager.IsItemEquipped(42), Is.False);
        }

        [Test]
        public void GivenSeveralMatchingItems_WhenOneIsEquipped_ThenItIsDetected()
        {
            SetInventoryItem(0, 42, 1, false);
            SetInventoryItem(1, 42, 1, false);
            SetInventoryItem(2, 42, 1, true);
            inventoryManager.InventoryItemsCount = 3;

            Assert.That(inventoryManager.IsItemEquipped(42));
        }

        [Test]
        public void GivenThreeItems_WhenRemovingTheFirst_ThenAllItemStateShiftsLeft()
        {
            SetInventoryItem(0, 4, 32, true);
            SetInventoryItem(1, 8, 42, false);
            SetInventoryItem(2, 16, 48, true);
            inventoryManager.InventoryItemsCount = 3;

            inventoryManager.RemoveItem(0);

            Assert.That(inventoryManager.InventoryItemsCount, Is.EqualTo(2));
            AssertInventoryItem(0, 8, 42, false);
            AssertInventoryItem(1, 16, 48, true);
        }

        [Test]
        public void GivenThreeItems_WhenRemovingTheMiddle_ThenTheLastItemShiftsLeft()
        {
            SetInventoryItem(0, 4, 32, true);
            SetInventoryItem(1, 8, 42, false);
            SetInventoryItem(2, 16, 48, true);
            inventoryManager.InventoryItemsCount = 3;

            inventoryManager.RemoveItem(1);

            Assert.That(inventoryManager.InventoryItemsCount, Is.EqualTo(2));
            AssertInventoryItem(0, 4, 32, true);
            AssertInventoryItem(1, 16, 48, true);
        }

        [Test]
        public void GivenThreeItems_WhenRemovingTheLast_ThenEarlierItemsRemainUnchanged()
        {
            SetInventoryItem(0, 4, 32, true);
            SetInventoryItem(1, 8, 42, false);
            SetInventoryItem(2, 16, 48, true);
            inventoryManager.InventoryItemsCount = 3;

            inventoryManager.RemoveItem(2);

            Assert.That(inventoryManager.InventoryItemsCount, Is.EqualTo(2));
            AssertInventoryItem(0, 4, 32, true);
            AssertInventoryItem(1, 8, 42, false);
        }

        [Test]
        public void GivenAnEmptyInventory_WhenCountingAnAbsentItem_ThenZeroIsReturnedWithoutEntityLookup()
            => Assert.That(inventoryManager.GetItemTotalCount(42), Is.Zero);

        [Test]
        public void GivenOtherInventoryItems_WhenCountingAnAbsentItem_ThenZeroIsReturnedWithoutEntityLookup()
        {
            SetInventoryItem(0, 4, 32, true);
            SetInventoryItem(1, 8, 42, false);
            inventoryManager.InventoryItemsCount = 2;

            Assert.That(inventoryManager.GetItemTotalCount(16), Is.Zero);
        }

        [TestCase(0, 4, 32)]
        [TestCase(4, 8, 42)]
        [TestCase(255, 16, int.MaxValue)]
        public void GivenABankItem_WhenAddingIt_ThenServerAndVisibleBankStateAreUpdated(
            int itemSlot,
            int itemIdentifier,
            int quantity)
        {
            inventoryManager.BankItem(itemIdentifier, itemSlot, quantity);

            Assert.That(inventoryManager.ServerBankItemsCount, Is.EqualTo(itemSlot + 1));
            Assert.That(inventoryManager.BankItemsCount, Is.EqualTo(itemSlot + 1));
            AssertServerBankItem(itemSlot, itemIdentifier, quantity);
            AssertBankItem(itemSlot, itemIdentifier, quantity);
        }

        [Test]
        public void GivenAnExistingBankSlot_WhenUpdatingIt_ThenTheBankCountDoesNotIncrease()
        {
            inventoryManager.BankItem(4, 0, 32);

            inventoryManager.BankItem(8, 0, 42);

            Assert.That(inventoryManager.ServerBankItemsCount, Is.EqualTo(1));
            Assert.That(inventoryManager.BankItemsCount, Is.EqualTo(1));
            AssertServerBankItem(0, 8, 42);
            AssertBankItem(0, 8, 42);
        }

        [Test]
        public void GivenThreeBankItems_WhenRemovingTheMiddle_ThenTheLastItemShiftsLeft()
        {
            inventoryManager.BankItem(4, 0, 32);
            inventoryManager.BankItem(8, 1, 42);
            inventoryManager.BankItem(16, 2, 48);

            inventoryManager.BankItem(8, 1, 0);

            Assert.That(inventoryManager.ServerBankItemsCount, Is.EqualTo(2));
            Assert.That(inventoryManager.BankItemsCount, Is.EqualTo(2));
            AssertServerBankItem(0, 4, 32);
            AssertServerBankItem(1, 16, 48);
            AssertBankItem(0, 4, 32);
            AssertBankItem(1, 16, 48);
        }

        [Test]
        public void GivenServerAndInventoryItems_WhenUpdatingVisibleBankItems_ThenUniqueMissingItemsAreAppended()
        {
            inventoryManager.BankItem(4, 0, 32);
            SetInventoryItem(0, 4, 42, true);
            SetInventoryItem(1, 8, 48, true);
            SetInventoryItem(2, 8, 64, false);
            SetInventoryItem(3, 16, 96, true);
            inventoryManager.InventoryItemsCount = 4;

            inventoryManager.UpdateBankItems();

            Assert.That(inventoryManager.BankItemsCount, Is.EqualTo(3));
            AssertBankItem(0, 4, 32);
            AssertBankItem(1, 8, 0);
            AssertBankItem(2, 16, 0);
            Assert.That(inventoryManager.GetBankItem(1).IsEquipped, Is.False);
            Assert.That(inventoryManager.GetBankItem(2).IsEquipped, Is.False);
        }

        [Test]
        public void GivenAFullVisibleBank_WhenUpdatingIt_ThenInventoryItemsAreNotAppended()
        {
            inventoryManager.BankItem(4, InventoryManager.MaximumBankSize - 1, 32);
            SetInventoryItem(0, 42, 64, true);
            inventoryManager.InventoryItemsCount = 1;

            inventoryManager.UpdateBankItems();

            Assert.That(inventoryManager.BankItemsCount, Is.EqualTo(InventoryManager.MaximumBankSize));
            Assert.That(
                Enumerable.Range(0, InventoryManager.MaximumBankSize)
                    .Select(slot => inventoryManager.GetBankItem(slot).Index),
                Does.Not.Contain(42));
        }

        [TestCase(-1)]
        [TestCase(256)]
        public void GivenAnInvalidBankSlot_WhenAddingAnItem_ThenAnIndexExceptionIsThrown(int itemSlot)
            => Assert.That(
                () => inventoryManager.BankItem(42, itemSlot, 64),
                Throws.TypeOf<IndexOutOfRangeException>());

        private void AssertBankItem(int itemSlot, int expectedIdentifier, int expectedQuantity)
        {
            InventoryItem item = inventoryManager.GetBankItem(itemSlot);

            Assert.That(item.Index, Is.EqualTo(expectedIdentifier));
            Assert.That(item.Quantity, Is.EqualTo(expectedQuantity));
        }

        private void AssertInventoryItem(
            int itemSlot,
            int expectedIdentifier,
            int expectedQuantity,
            bool expectedIsEquipped)
        {
            InventoryItem item = inventoryManager.GetItem(itemSlot);

            Assert.That(item.Index, Is.EqualTo(expectedIdentifier));
            Assert.That(item.Quantity, Is.EqualTo(expectedQuantity));
            Assert.That(item.IsEquipped, Is.EqualTo(expectedIsEquipped));
        }

        private void AssertServerBankItem(int itemSlot, int expectedIdentifier, int expectedQuantity)
        {
            InventoryItem item = inventoryManager.GetServerBankItem(itemSlot);

            Assert.That(item.Index, Is.EqualTo(expectedIdentifier));
            Assert.That(item.Quantity, Is.EqualTo(expectedQuantity));
        }

        private void SetInventoryItem(
            int itemSlot,
            int itemIdentifier,
            int quantity,
            bool isEquipped)
        {
            inventoryManager.SetItem(itemSlot, itemIdentifier);
            inventoryManager.SetItemCount(itemSlot, quantity);
            inventoryManager.SetItemEquippedStatus(itemSlot, isEquipped);
        }
    }
}