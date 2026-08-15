using System;

using NUnit.Framework;

using OpenRS.Net;
using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    public sealed class GameAppletMiddleManTests
    {
        private GameAppletMiddleManTestDouble applet = null!;

        [SetUp]
        public void SetUp()
        {
            applet = new GameAppletMiddleManTestDouble();
        }

        [Test]
        public void GivenANewMiddleMan_WhenReadingItsState_ThenCompatibilityBuffersAreInitialised()
        {
            Assert.That(applet.username, Is.Empty);
            Assert.That(applet.packetData, Has.Length.EqualTo(10000));
            Assert.That(applet.friendsList, Has.Length.EqualTo(40));
            Assert.That(applet.friendsWorld, Has.Length.EqualTo(400));
            Assert.That(applet.ignoresList, Has.Length.EqualTo(200));
            Assert.That(applet.friendsCount, Is.Zero);
            Assert.That(applet.ignoresCount, Is.Zero);
            Assert.That(applet.reconnecting, Is.False);
        }

        [Test]
        public void GivenAFriendListPacket_WhenHandlingIt_ThenEntriesAreSortedByWorldDescending()
        {
            applet.packetData[1] = 3;
            WriteLong(applet.packetData, 2, 4L);
            applet.packetData[10] = 1;
            WriteLong(applet.packetData, 11, 8L);
            applet.packetData[19] = 42;
            WriteLong(applet.packetData, 20, 16L);
            applet.packetData[28] = 8;

            applet.HandlePacket((int)ServerCommand.FriendList, 29);

            Assert.That(applet.friendsCount, Is.EqualTo(3));
            Assert.That(applet.friendsList[..3], Is.EqualTo(new long[] { 8L, 16L, 4L }));
            Assert.That(applet.friendsWorld[..3], Is.EqualTo(new[] { 42, 8, 1 }));
        }

        [Test]
        public void GivenFriendsInTheSameWorld_WhenHandlingTheList_ThenTheirWireOrderIsPreserved()
        {
            long[] expectedFriends = [4L, 8L, 16L];
            int[] expectedWorlds = [42, 42, 8];
            applet.packetData[1] = 3;
            WriteLong(applet.packetData, 2, 4L);
            applet.packetData[10] = 42;
            WriteLong(applet.packetData, 11, 8L);
            applet.packetData[19] = 42;
            WriteLong(applet.packetData, 20, 16L);
            applet.packetData[28] = 8;

            applet.HandlePacket((int)ServerCommand.FriendList, 29);

            Assert.That(applet.friendsList[..3], Is.EqualTo(expectedFriends));
            Assert.That(applet.friendsWorld[..3], Is.EqualTo(expectedWorlds));
        }

        [Test]
        public void GivenAnExistingFriendUpdate_WhenHandlingIt_ThenStatusAndOrderingAreUpdated()
        {
            applet.friendsCount = 2;
            applet.friendsList[0] = 4L;
            applet.friendsWorld[0] = 1;
            applet.friendsList[1] = 8L;
            applet.friendsWorld[1] = 0;
            WriteLong(applet.packetData, 1, 8L);
            applet.packetData[9] = 42;

            applet.HandlePacket((int)ServerCommand.FriendUpdate, 10);

            Assert.That(applet.friendsList[..2], Is.EqualTo(new long[] { 8L, 4L }));
            Assert.That(applet.friendsWorld[..2], Is.EqualTo(new[] { 42, 1 }));
            Assert.That(applet.LastDisplayedMessage, Is.EqualTo("social.friend_logged_in"));
        }

        [Test]
        public void GivenAnOnlineFriendGoingOffline_WhenHandlingIt_ThenStatusAndMessageAreUpdated()
        {
            applet.friendsCount = 1;
            applet.friendsList[0] = 4L;
            applet.friendsWorld[0] = 42;
            WriteLong(applet.packetData, 1, 4L);
            applet.packetData[9] = 0;

            applet.HandlePacket((int)ServerCommand.FriendUpdate, 10);

            Assert.That(applet.friendsWorld[0], Is.Zero);
            Assert.That(applet.LastDisplayedMessage, Is.EqualTo("social.friend_logged_out"));
        }

        [Test]
        public void GivenANewFriendUpdate_WhenHandlingIt_ThenTheFriendIsAppended()
        {
            WriteLong(applet.packetData, 1, 42L);
            applet.packetData[9] = 8;

            applet.HandlePacket((int)ServerCommand.FriendUpdate, 10);

            Assert.That(applet.friendsCount, Is.EqualTo(1));
            Assert.That(applet.friendsList[0], Is.EqualTo(42L));
            Assert.That(applet.friendsWorld[0], Is.EqualTo(8));
        }

        [Test]
        public void GivenAnIgnoreListPacket_WhenHandlingIt_ThenEntriesAreReadInWireOrder()
        {
            applet.packetData[1] = 3;
            WriteLong(applet.packetData, 2, 4L);
            WriteLong(applet.packetData, 10, 8L);
            WriteLong(applet.packetData, 18, 16L);

            applet.HandlePacket((int)ServerCommand.IgnoreList, 26);

            Assert.That(applet.ignoresCount, Is.EqualTo(3));
            Assert.That(applet.ignoresList[..3], Is.EqualTo(new long[] { 4L, 8L, 16L }));
        }

        [Test]
        public void GivenAnIgnoreCountBeyondPacketData_WhenHandlingIt_ThenCountIsClampedToCompleteEntries()
        {
            applet.packetData[1] = 42;
            WriteLong(applet.packetData, 2, 4L);

            applet.HandlePacket((int)ServerCommand.IgnoreList, 10);

            Assert.That(applet.ignoresCount, Is.EqualTo(1));
            Assert.That(applet.ignoresList[0], Is.EqualTo(4L));
        }

        [Test]
        public void GivenAPrivacyPacket_WhenHandlingIt_ThenEveryBlockFlagIsUpdated()
        {
            applet.packetData[1] = 4;
            applet.packetData[2] = 8;
            applet.packetData[3] = 16;
            applet.packetData[4] = 32;

            applet.HandlePacket((int)ServerCommand.WontImplement158, 5);

            Assert.That(applet.blockChat, Is.EqualTo(4));
            Assert.That(applet.blockPrivate, Is.EqualTo(8));
            Assert.That(applet.blockTrade, Is.EqualTo(16));
            Assert.That(applet.blockDuel, Is.EqualTo(32));
        }

        [Test]
        public void GivenALogoutCannotPacket_WhenHandlingIt_ThenTheCompatibilityCallbackIsInvoked()
        {
            applet.HandlePacket((int)ServerCommand.LogoutCannot, 1);

            Assert.That(applet.IsCantLogoutInvoked);
        }

        [Test]
        public void GivenALogoutRequestPacket_WhenHandlingIt_ThenCredentialsAndRuntimeStateReset()
        {
            applet.username = "zezima";

            applet.HandlePacket((int)ServerCommand.LogoutRequest, 1);

            Assert.That(applet.username, Is.Empty);
            Assert.That(applet.IsResetIntVarsInvoked);
            Assert.That(applet.LastLoginFirstLine, Is.EqualTo("login.enter_credentials"));
            Assert.That(applet.LastLoginSecondLine, Is.Empty);
        }

        [Test]
        public void GivenAUnknownPacket_WhenHandlingIt_ThenTheThreeArgumentCallbackReceivesIt()
        {
            applet.HandlePacket(42, 8);

            Assert.That(applet.LastPacketCommand, Is.EqualTo(42));
            Assert.That(applet.LastPacketLength, Is.EqualTo(8));
            Assert.That(applet.LastPacketData, Is.SameAs(applet.packetData));
        }

        [Test]
        public void GivenAPrivateMessagePacket_WhenHandlingIt_ThenTheDisplayCallbackIsInvoked()
        {
            WriteLong(applet.packetData, 1, 42L);

            applet.HandlePacket((int)ServerCommand.PrivateMessage, 9);

            Assert.That(applet.LastDisplayedMessage, Is.EqualTo("social.private_message_received"));
        }

        [Test]
        public void GivenAConnectionLoss_WhenHandlingIt_ThenTheLoginPromptIsDisplayed()
        {
            applet.LostConnection();

            Assert.That(applet.LastLoginFirstLine, Is.EqualTo("login.enter_credentials"));
            Assert.That(applet.LastLoginSecondLine, Is.Empty);
        }

        [Test]
        public void GivenAServerAnnouncement_WhenHandlingIt_ThenUtf8TextIsDisplayed()
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes("RuneScape România");

            for (int byteIndex = 0; byteIndex < textBytes.Length; byteIndex += 1)
            {
                applet.packetData[byteIndex + 1] = unchecked((sbyte)textBytes[byteIndex]);
            }

            applet.HandlePacket((int)ServerCommand.ServerAnnouncement, textBytes.Length + 1);

            Assert.That(applet.LastDisplayedMessage, Is.EqualTo("RuneScape România"));
        }

        [Test]
        public void GivenAServerInfoPacketWithoutAStream_WhenHandlingIt_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => applet.HandlePacket((int)ServerCommand.ServerInfo, 1),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenASocketTimeout_WhenConnecting_ThenServerFullStatusIsDisplayedWithoutNetworking()
        {
            applet.socketTimeout = 1;

            applet.Connect("zezima", "NucileRullz!", false);

            Assert.That(applet.LastLoginFirstLine, Is.EqualTo("login.server_full"));
            Assert.That(applet.LastLoginSecondLine, Is.EqualTo("login.please_try_again_later"));
        }

        [Test]
        public void GivenTheProtectedGameBoxCompatibilityMethod_WhenInvokingIt_ThenNoExceptionIsThrown()
            => Assert.That(
                () => applet.InvokeGameBoxPrint("RuneScape", "Dark Souls III"),
                Throws.Nothing);

        [Test]
        public void GivenTheBaseMiddleManCallbacks_WhenInvokingThem_ThenTheyRemainNoOps()
        {
            GameAppletMiddleMan middleMan = new();

            Assert.That(() => middleMan.LoginScreenPrint("RuneScape", "Dark Souls III"), Throws.Nothing);
            Assert.That(() => middleMan.InitVars(), Throws.Nothing);
            Assert.That(() => middleMan.ResetIntVars(), Throws.Nothing);
            Assert.That(() => middleMan.CantLogout(), Throws.Nothing);
            Assert.That(() => middleMan.HandlePacket(4, 8, [16]), Throws.Nothing);
            Assert.That(() => middleMan.DisplayMessage("RuneScape"), Throws.Nothing);
        }

        [Test]
        public void GivenCompatibilityCallbacks_WhenInvokingTheirDefaults_ThenNoExceptionIsThrown()
        {
            Assert.That(() => applet.LoginScreenPrint("RuneScape", "Dark Souls III"), Throws.Nothing);
            Assert.That(() => applet.InitVars(), Throws.Nothing);
            Assert.That(() => applet.ResetIntVars(), Throws.Nothing);
            Assert.That(() => applet.CantLogout(), Throws.Nothing);
            Assert.That(() => applet.HandlePacket(4, 8, [16]), Throws.Nothing);
            Assert.That(() => applet.DisplayMessage("RuneScape"), Throws.Nothing);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GivenAReconnectState_WhenSettingIt_ThenTheStateIsRetained(bool isReconnecting)
        {
            applet.reconnecting = isReconnecting;

            Assert.That(applet.reconnecting, Is.EqualTo(isReconnecting));
        }

        private static void WriteLong(sbyte[] target, int offset, long value)
        {
            for (int byteIndex = 0; byteIndex < sizeof(long); byteIndex += 1)
            {
                int shift = (sizeof(long) - byteIndex - 1) * 8;
                target[offset + byteIndex] = unchecked((sbyte)(value >> shift));
            }
        }
    }
}