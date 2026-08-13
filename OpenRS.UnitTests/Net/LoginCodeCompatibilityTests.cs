using System;

using NUnit.Framework;

using OpenRS.Net;

namespace OpenRS.UnitTests.Net
{
    [TestFixture]
    public sealed class LoginCodeCompatibilityTests
    {
        [TestCase(LoginCode.ServerTimeOut, -1)]
        [TestCase(LoginCode.LoginSuccess, 0)]
        [TestCase(LoginCode.ReconnectSuccess, 1)]
        [TestCase(LoginCode.InvalidCredentials, 2)]
        [TestCase(LoginCode.UsernameAlreadyLoggedIn, 3)]
        [TestCase(LoginCode.ClientUpdated, 4)]
        [TestCase(LoginCode.SessionRejected, 5)]
        [TestCase(LoginCode.AccountBanned, 6)]
        [TestCase(LoginCode.ProfileDecodeFailure, 7)]
        [TestCase(LoginCode.TooManyConnections, 8)]
        [TestCase(LoginCode.AccountAlreadyLoggedIn, 9)]
        [TestCase(LoginCode.LoginComplete, 99)]
        public void GivenALoginCode_WhenReadingItsIdentifier_ThenTheValueRemainsCompatible(
            LoginCode loginCode,
            int expectedIdentifier)
            => Assert.That(
                (int)loginCode,
                Is.EqualTo(expectedIdentifier));

        [Test]
        public void GivenTheLoginCodeContract_WhenCountingItsMembers_ThenNoMemberIsAddedOrRemoved()
            => Assert.That(
                Enum.GetValues<LoginCode>(),
                Has.Length.EqualTo(12));
    }
}