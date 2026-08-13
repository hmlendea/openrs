using System;

using NUnit.Framework;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class ModelEnumerationCompatibilityTests
    {
        [TestCase(GameAction.IdleOrWalking, 0)]
        [TestCase(GameAction.Attacking, 1)]
        [TestCase(GameAction.FightingMelee, 2)]
        [TestCase(GameAction.FightingRanged, 3)]
        [TestCase(GameAction.TalkingWithNpc, 4)]
        [TestCase(GameAction.DroppingItem, 5)]
        [TestCase(GameAction.TakingItem, 6)]
        [TestCase(GameAction.UsingItemOnGroundItem, 7)]
        [TestCase(GameAction.UsingItemOnNpc, 8)]
        [TestCase(GameAction.UsingItemOnObject, 9)]
        [TestCase(GameAction.UsingItemOnWallObject, 10)]
        [TestCase(GameAction.UsingObject, 11)]
        [TestCase(GameAction.UsingWallObject, 12)]
        [TestCase(GameAction.CastingSpellOnNpc, 13)]
        [TestCase(GameAction.CastingSpellOnGroundItem, 14)]
        public void GivenAGameAction_WhenReadingItsValue_ThenItRemainsCompatible(
            GameAction action,
            int expectedValue)
            => Assert.That((int)action, Is.EqualTo(expectedValue));

        [TestCase(AttackType.Melee, 0)]
        [TestCase(AttackType.Ranged, 1)]
        public void GivenAnAttackType_WhenReadingItsValue_ThenItRemainsCompatible(
            AttackType attackType,
            int expectedValue)
            => Assert.That((int)attackType, Is.EqualTo(expectedValue));

        [TestCase(CombatState.Error, 0)]
        [TestCase(CombatState.Running, 1)]
        [TestCase(CombatState.Waiting, 2)]
        [TestCase(CombatState.Won, 3)]
        [TestCase(CombatState.Lost, 4)]
        public void GivenACombatState_WhenReadingItsValue_ThenItRemainsCompatible(
            CombatState combatState,
            int expectedValue)
            => Assert.That((int)combatState, Is.EqualTo(expectedValue));

        [TestCase(CombatStyle.Controlled, 0)]
        [TestCase(CombatStyle.Aggressive, 1)]
        [TestCase(CombatStyle.Accurate, 2)]
        [TestCase(CombatStyle.Defensive, 3)]
        public void GivenACombatStyle_WhenReadingItsValue_ThenItRemainsCompatible(
            CombatStyle combatStyle,
            int expectedValue)
            => Assert.That((int)combatStyle, Is.EqualTo(expectedValue));

        [TestCase(ElementalStaff.Air, 197)]
        [TestCase(ElementalStaff.Earth, 101)]
        [TestCase(ElementalStaff.Water, 102)]
        [TestCase(ElementalStaff.Fire, 103)]
        public void GivenAnElementalStaff_WhenReadingItsValue_ThenItRemainsCompatible(
            ElementalStaff staff,
            int expectedValue)
            => Assert.That((int)staff, Is.EqualTo(expectedValue));

        [TestCase(ElementalBattlestaff.Air, 615)]
        [TestCase(ElementalBattlestaff.Water, 616)]
        [TestCase(ElementalBattlestaff.Earth, 617)]
        [TestCase(ElementalBattlestaff.Fire, 618)]
        public void GivenAnElementalBattlestaff_WhenReadingItsValue_ThenItRemainsCompatible(
            ElementalBattlestaff staff,
            int expectedValue)
            => Assert.That((int)staff, Is.EqualTo(expectedValue));

        [TestCase(ElementalMysticStaff.Air, 682)]
        [TestCase(ElementalMysticStaff.Water, 683)]
        [TestCase(ElementalMysticStaff.Earth, 684)]
        [TestCase(ElementalMysticStaff.Fire, 685)]
        public void GivenAnElementalMysticStaff_WhenReadingItsValue_ThenItRemainsCompatible(
            ElementalMysticStaff staff,
            int expectedValue)
            => Assert.That((int)staff, Is.EqualTo(expectedValue));

        [TestCase(GameObjectType.WorldObject, 0)]
        [TestCase(GameObjectType.WallObject, 1)]
        public void GivenAGameObjectType_WhenReadingItsValue_ThenItRemainsCompatible(
            GameObjectType objectType,
            int expectedValue)
            => Assert.That((int)objectType, Is.EqualTo(expectedValue));

        [TestCase(Gender.Male, 0)]
        [TestCase(Gender.Female, 1)]
        public void GivenAGender_WhenReadingItsValue_ThenItRemainsCompatible(
            Gender gender,
            int expectedValue)
            => Assert.That((int)gender, Is.EqualTo(expectedValue));

        [TestCase(RuneElement.Air, 31)]
        [TestCase(RuneElement.Water, 32)]
        [TestCase(RuneElement.Earth, 33)]
        [TestCase(RuneElement.Fire, 34)]
        public void GivenARuneElement_WhenReadingItsValue_ThenItRemainsCompatible(
            RuneElement runeElement,
            int expectedValue)
            => Assert.That((int)runeElement, Is.EqualTo(expectedValue));

        [Test]
        public void GivenTheModelEnumerationContracts_WhenCountingMembers_ThenTheyRemainCompatible()
        {
            Assert.That(Enum.GetValues<GameAction>(), Has.Length.EqualTo(15));
            Assert.That(Enum.GetValues<AttackType>(), Has.Length.EqualTo(2));
            Assert.That(Enum.GetValues<CombatState>(), Has.Length.EqualTo(5));
            Assert.That(Enum.GetValues<CombatStyle>(), Has.Length.EqualTo(4));
            Assert.That(Enum.GetValues<ElementalStaff>(), Has.Length.EqualTo(4));
            Assert.That(Enum.GetValues<ElementalBattlestaff>(), Has.Length.EqualTo(4));
            Assert.That(Enum.GetValues<ElementalMysticStaff>(), Has.Length.EqualTo(4));
            Assert.That(Enum.GetValues<GameObjectType>(), Has.Length.EqualTo(2));
            Assert.That(Enum.GetValues<Gender>(), Has.Length.EqualTo(2));
            Assert.That(Enum.GetValues<RuneElement>(), Has.Length.EqualTo(4));
        }
    }
}