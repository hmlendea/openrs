using System;

using NUnit.Framework;

using OpenRS.Net;

namespace OpenRS.UnitTests.Net
{
    [TestFixture]
    public sealed class MenuActionCompatibilityTests
    {
        [TestCase(MenuAction.CastSpellOnGroundItem, 200)]
        [TestCase(MenuAction.UseItemWithGroundItem, 210)]
        [TestCase(MenuAction.TakeItem, 220)]
        [TestCase(MenuAction.CastSpellOnWallObject, 300)]
        [TestCase(MenuAction.UseItemWithWallObject, 310)]
        [TestCase(MenuAction.Command1OnWallObject, 320)]
        [TestCase(MenuAction.CastSpellOnModel, 400)]
        [TestCase(MenuAction.UseItemWithModel, 410)]
        [TestCase(MenuAction.Command1OnModel, 420)]
        [TestCase(MenuAction.CastSpellOnItem, 600)]
        [TestCase(MenuAction.UseItemWithItem, 610)]
        [TestCase(MenuAction.RemoveItem, 620)]
        [TestCase(MenuAction.EquipItem, 630)]
        [TestCase(MenuAction.CommandOnItem, 640)]
        [TestCase(MenuAction.UseItem, 650)]
        [TestCase(MenuAction.DropItem, 660)]
        [TestCase(MenuAction.CastSpellOnNpc, 700)]
        [TestCase(MenuAction.UseItemWithNpc, 710)]
        [TestCase(MenuAction.AttackNpc, 715)]
        [TestCase(MenuAction.TalkToNpc, 720)]
        [TestCase(MenuAction.CommandOnNpc, 725)]
        [TestCase(MenuAction.CastSpellOnPlayer, 800)]
        [TestCase(MenuAction.UseItemWithPlayer, 810)]
        [TestCase(MenuAction.AttackPlayerSafe, 805)]
        [TestCase(MenuAction.CastSpellOnGround, 900)]
        [TestCase(MenuAction.WalkHere, 920)]
        [TestCase(MenuAction.CastSpellOnSelf, 1000)]
        [TestCase(MenuAction.Command2OnWallObject, 2300)]
        [TestCase(MenuAction.Command2OnModel, 2400)]
        [TestCase(MenuAction.AttackNpcOffscreen, 2715)]
        [TestCase(MenuAction.AttackPlayerUnsafe, 2805)]
        [TestCase(MenuAction.DuelWithPlayer, 2806)]
        [TestCase(MenuAction.TradeWithPlayer, 2810)]
        [TestCase(MenuAction.FollowPlayer, 2820)]
        [TestCase(MenuAction.AddFriend, 2830)]
        [TestCase(MenuAction.ExamineGroundItem, 3200)]
        [TestCase(MenuAction.ExamineWallObject, 3300)]
        [TestCase(MenuAction.ExamineModel, 3400)]
        [TestCase(MenuAction.ExamineItem, 3600)]
        [TestCase(MenuAction.ExamineNpc, 3700)]
        [TestCase(MenuAction.Cancel, 4000)]
        [TestCase(MenuAction.TeleportHere, 9902)]
        public void GivenAMenuAction_WhenReadingItsIdentifier_ThenTheValueRemainsCompatible(
            MenuAction menuAction,
            int expectedIdentifier)
            => Assert.That(
                (int)menuAction,
                Is.EqualTo(expectedIdentifier));

        [Test]
        public void GivenTheMenuActionContract_WhenCountingItsMembers_ThenNoMemberIsAddedOrRemoved()
            => Assert.That(
                Enum.GetValues<MenuAction>(),
                Has.Length.EqualTo(42));
    }
}