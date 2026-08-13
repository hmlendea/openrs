using System;

using NUnit.Framework;

using OpenRS.Net;

namespace OpenRS.UnitTests.Net
{
    [TestFixture]
    public sealed class ClientPacketCompatibilityTests
    {
        [TestCase(ClientPacket.Login, 0)]
        [TestCase(ClientPacket.Ping, 5)]
        [TestCase(ClientPacket.SetFriendPrivacySetting, 7)]
        [TestCase(ClientPacket.UseItemWithPlayer, 16)]
        [TestCase(ClientPacket.CastSpellOnGameObject, 17)]
        [TestCase(ClientPacket.AddIgnore, 25)]
        [TestCase(ClientPacket.UseItemWithInventoryItem, 27)]
        [TestCase(ClientPacket.SessionNameHash, 32)]
        [TestCase(ClientPacket.UseItemWithGroundItem, 34)]
        [TestCase(ClientPacket.DeclineDuel, 35)]
        [TestCase(ClientPacket.UseItemWithWallObject, 36)]
        [TestCase(ClientPacket.RequestLogout, 39)]
        [TestCase(ClientPacket.GameObjectCommand2, 40)]
        [TestCase(ClientPacket.SetCombatStyle, 42)]
        [TestCase(ClientPacket.CloseBankWindow, 48)]
        [TestCase(ClientPacket.CastSpellOnInventoryItem, 49)]
        [TestCase(ClientPacket.GameObjectCommand1, 51)]
        [TestCase(ClientPacket.RemoveFriend, 52)]
        [TestCase(ClientPacket.ConfirmTrade, 53)]
        [TestCase(ClientPacket.CastSpellOnPlayer, 55)]
        [TestCase(ClientPacket.ActivatePrayer, 56)]
        [TestCase(ClientPacket.AttackPlayer, 57)]
        [TestCase(ClientPacket.CastSpellOnWallObject, 67)]
        [TestCase(ClientPacket.FollowPlayer, 68)]
        [TestCase(ClientPacket.ClientInfoReport, 69)]
        [TestCase(ClientPacket.UpdateTradeItems, 70)]
        [TestCase(ClientPacket.CastSpellOnNpc, 71)]
        [TestCase(ClientPacket.AttackNpc, 73)]
        [TestCase(ClientPacket.NpcCommand, 74)]
        [TestCase(ClientPacket.ReportEntityCount, 83)]
        [TestCase(ClientPacket.ConfirmDuel, 87)]
        [TestCase(ClientPacket.InventoryItemCommand, 89)]
        [TestCase(ClientPacket.SendCommand, 90)]
        [TestCase(ClientPacket.RemoveInventoryItem, 92)]
        [TestCase(ClientPacket.UseItemWithGameObject, 94)]
        [TestCase(ClientPacket.CastSpellOnGroundItem, 104)]
        [TestCase(ClientPacket.RemoveIgnore, 108)]
        [TestCase(ClientPacket.UpdateDuelItems, 123)]
        [TestCase(ClientPacket.WallObjectCommand1, 126)]
        [TestCase(ClientPacket.BuyShopItem, 128)]
        [TestCase(ClientPacket.SendLogout, 129)]
        [TestCase(ClientPacket.Walk, 132)]
        [TestCase(ClientPacket.UseItemWithNpc, 142)]
        [TestCase(ClientPacket.SendChatMessage, 145)]
        [TestCase(ClientPacket.DropItem, 147)]
        [TestCase(ClientPacket.SelectShopItem, 154)]
        [TestCase(ClientPacket.UpdateGameConfig, 157)]
        [TestCase(ClientPacket.TradeWithPlayer, 166)]
        [TestCase(ClientPacket.AddFriend, 168)]
        [TestCase(ClientPacket.UpdatePrivacySettings, 176)]
        [TestCase(ClientPacket.TalkToNpc, 177)]
        [TestCase(ClientPacket.EquipItem, 181)]
        [TestCase(ClientPacket.ActivateSpell, 183)]
        [TestCase(ClientPacket.DepositBankItem, 198)]
        [TestCase(ClientPacket.AnswerSleepWord, 200)]
        [TestCase(ClientPacket.CastSpellOnSelf, 206)]
        [TestCase(ClientPacket.AcceptTrade, 211)]
        [TestCase(ClientPacket.DeclineTrade, 216)]
        [TestCase(ClientPacket.UpdateAppearance, 218)]
        [TestCase(ClientPacket.DuelWithPlayer, 222)]
        [TestCase(ClientPacket.SendDuelSettings, 225)]
        [TestCase(ClientPacket.CastSpellOnGround, 232)]
        [TestCase(ClientPacket.WallObjectCommand2, 235)]
        [TestCase(ClientPacket.TakeGroundItem, 245)]
        [TestCase(ClientPacket.WalkToCommand, 246)]
        [TestCase(ClientPacket.DeactivatePrayer, 248)]
        [TestCase(ClientPacket.AcceptDuel, 252)]
        [TestCase(ClientPacket.CloseShopWindow, 253)]
        [TestCase(ClientPacket.SendPrivateMessage, 254)]
        [TestCase(ClientPacket.SellShopItem, 255)]
        public void GivenAClientPacket_WhenReadingItsIdentifier_ThenTheValueRemainsCompatible(
            ClientPacket clientPacket,
            int expectedIdentifier)
            => Assert.That(
                (int)clientPacket,
                Is.EqualTo(expectedIdentifier));

        [Test]
        public void GivenTheClientPacketContract_WhenCountingItsMembers_ThenNoMemberIsAddedOrRemoved()
            => Assert.That(
                Enum.GetValues<ClientPacket>(),
                Has.Length.EqualTo(70));
    }
}