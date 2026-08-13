using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.World;

namespace OpenRS.UnitTests.Net.Client.World
{
    [TestFixture]
    public sealed class ClientCommandTests
    {
        [TestCase("unknown")]
        [TestCase("closecon")]
        [TestCase("logout")]
        [TestCase("lostcon")]
        [TestCase("tell")]
        public void GivenAKnownName_WhenParsingACommand_ThenTheNamedCommandIsReturned(string name)
            => Assert.That(
                ClientCommand.FromString(name).Name,
                Is.EqualTo(name));

        [TestCase("Unknown")]
        [TestCase("closeconnection")]
        [TestCase("")]
        [TestCase(" ")]
        public void GivenAnUnknownName_WhenParsingACommand_ThenTheUnknownCommandIsReturned(string name)
            => Assert.That(
                ClientCommand.FromString(name),
                Is.SameAs(ClientCommand.Unknown));

        [Test]
        public void GivenANullName_WhenParsingACommand_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => ClientCommand.FromString(null!),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void GivenTheCommandContract_WhenRetrievingAllValues_ThenTheFiveCommandsAreReturned()
        {
            string[] commandNames = ClientCommand.GetValues()
                .Cast<ClientCommand>()
                .Select(command => command.Name)
                .ToArray();

            Assert.That(
                commandNames,
                Is.EqualTo(["unknown", "closecon", "logout", "lostcon", "tell"]));
        }

        [Test]
        public void GivenTheSameCommand_WhenComparingIt_ThenItIsEqual()
        {
            ClientCommand command = ClientCommand.Logout;

            Assert.That(command.Equals(ClientCommand.Logout));
            Assert.That(command.Equals((object)ClientCommand.Logout));
            Assert.That(command == ClientCommand.Logout);
            Assert.That(command != ClientCommand.Logout, Is.False);
            Assert.That(
                command.GetHashCode(),
                Is.EqualTo(ClientCommand.Logout.GetHashCode()));
        }

        [Test]
        public void GivenDifferentCommands_WhenComparingThem_ThenTheyAreNotEqual()
        {
            ClientCommand command = ClientCommand.Logout;

            Assert.That(command.Equals(ClientCommand.Tell), Is.False);
            Assert.That(command == ClientCommand.Tell, Is.False);
            Assert.That(command != ClientCommand.Tell);
        }

        [Test]
        public void GivenANullOrDifferentType_WhenUsingEquals_ThenItIsNotEqual()
        {
            ClientCommand command = ClientCommand.Logout;
            ClientCommand nullCommand = null!;
            object nullObject = null!;

            Assert.That(command.Equals(nullCommand), Is.False);
            Assert.That(command.Equals(nullObject), Is.False);
            Assert.That(command.Equals("logout"), Is.False);
        }

        [Test]
        public void GivenANullAndACommand_WhenUsingEqualityOperators_ThenTheyCompareUnequal()
        {
            ClientCommand nullCommand = null!;

            Assert.That(nullCommand == ClientCommand.Unknown, Is.False);
            Assert.That(ClientCommand.Unknown == nullCommand, Is.False);
            Assert.That(nullCommand != ClientCommand.Unknown);
            Assert.That(ClientCommand.Unknown != nullCommand);
        }

        [Test]
        public void GivenTwoNullCommands_WhenUsingEqualityOperators_ThenTheyCompareEqual()
        {
            ClientCommand firstCommand = null!;
            ClientCommand secondCommand = null!;

            Assert.That(firstCommand == secondCommand);
            Assert.That(firstCommand != secondCommand, Is.False);
        }

        [Test]
        public void GivenACommand_WhenConvertingItToText_ThenItsNameIsReturned()
        {
            ClientCommand command = ClientCommand.CloseConnection;
            string implicitText = command;

            Assert.That(command.ToString(), Is.EqualTo("closecon"));
            Assert.That(implicitText, Is.EqualTo("closecon"));
        }
    }
}