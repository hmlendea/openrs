using NuciLog.Core;

namespace OpenRS.Logging
{
    public sealed class GameOperation : Operation
    {
        private GameOperation(string name) : base(name) { }

        public static Operation AddSceneObject => new GameOperation(nameof(AddSceneObject));
        public static Operation CalculatePath => new GameOperation(nameof(CalculatePath));
        public static Operation Connect => new GameOperation(nameof(Connect));
        public static Operation ForceShutdown => new GameOperation(nameof(ForceShutdown));
        public static Operation HandlePacket => new GameOperation(nameof(HandlePacket));
        public static Operation LoadAnimations => new GameOperation(nameof(LoadAnimations));
        public static Operation LoadItemTexture => new GameOperation(nameof(LoadItemTexture));
        public static Operation LoadSection => new GameOperation(nameof(LoadSection));
        public static Operation Authenticate => new GameOperation(nameof(Authenticate));
        public static Operation LostConnection => new GameOperation(nameof(LostConnection));
        public static Operation NetworkDisconnect => new GameOperation(nameof(NetworkDisconnect));
        public static Operation ProcessInput => new GameOperation(nameof(ProcessInput));
        public static Operation ReadDataBlock => new GameOperation(nameof(ReadDataBlock));
        public static Operation RenderCharacter => new GameOperation(nameof(RenderCharacter));
        public static Operation RenderEntity => new GameOperation(nameof(RenderEntity));
        public static Operation RenderGame => new GameOperation(nameof(RenderGame));
        public static Operation RenderImage => new GameOperation(nameof(RenderImage));
        public static Operation RenderMinimap => new GameOperation(nameof(RenderMinimap));
        public static Operation RenderSprite => new GameOperation(nameof(RenderSprite));
        public static Operation RenderText => new GameOperation(nameof(RenderText));
        public static Operation SendPing => new GameOperation(nameof(SendPing));
        public static Operation Shutdown => new GameOperation(nameof(Shutdown));
        public static Operation Startup => new GameOperation(nameof(Startup));
        public static Operation UnpackData => new GameOperation(nameof(UnpackData));
        public static Operation UpdateEntitySprite => new GameOperation(nameof(UpdateEntitySprite));
    }
}
