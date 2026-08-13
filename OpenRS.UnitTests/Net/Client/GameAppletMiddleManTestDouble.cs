using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    internal sealed class GameAppletMiddleManTestDouble : GameAppletMiddleMan
    {
        internal bool IsCantLogoutInvoked { get; private set; }

        internal bool IsResetIntVarsInvoked { get; private set; }

        internal string LastDisplayedMessage { get; private set; } = null!;

        internal string LastLoginFirstLine { get; private set; } = null!;

        internal string LastLoginSecondLine { get; private set; } = null!;

        internal int LastPacketCommand { get; private set; }

        internal sbyte[] LastPacketData { get; private set; } = null!;

        internal int LastPacketLength { get; private set; }

        internal void InvokeGameBoxPrint(string firstLine, string secondLine)
            => GameBoxPrint(firstLine, secondLine);

        public override void CantLogout() => IsCantLogoutInvoked = true;

        public override void DisplayMessage(string message) => LastDisplayedMessage = message;

        public override void HandlePacket(int command, int length, sbyte[] data)
        {
            LastPacketCommand = command;
            LastPacketLength = length;
            LastPacketData = data;
        }

        public override void LoginScreenPrint(string firstLine, string secondLine)
        {
            LastLoginFirstLine = firstLine;
            LastLoginSecondLine = secondLine;
        }

        public override void ResetIntVars() => IsResetIntVarsInvoked = true;
    }
}