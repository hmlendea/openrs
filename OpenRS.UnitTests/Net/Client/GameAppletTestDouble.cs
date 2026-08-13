using Microsoft.Xna.Framework.Input;

using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    internal sealed class GameAppletTestDouble : GameApplet
    {
        internal int CheckInputsInvocationCount { get; private set; }

        internal int CloseInvocationCount { get; private set; }

        internal char LastHandledCharacter { get; private set; }

        internal Keys LastHandledKey { get; private set; }

        internal int LastMouseX { get; private set; }

        internal int LastMouseY { get; private set; }

        internal int LastPressedMouseButton { get; private set; }

        public override void CheckInputs() => CheckInputsInvocationCount += 1;

        public override void Close() => CloseInvocationCount += 1;

        public override void HandleKeyDown(Keys key, char character)
        {
            LastHandledKey = key;
            LastHandledCharacter = character;
        }

        public override void HandleMouseDown(
            int pressedMouseButton,
            int mouseXPosition,
            int mouseYPosition)
        {
            LastPressedMouseButton = pressedMouseButton;
            LastMouseX = mouseXPosition;
            LastMouseY = mouseYPosition;
        }
    }
}