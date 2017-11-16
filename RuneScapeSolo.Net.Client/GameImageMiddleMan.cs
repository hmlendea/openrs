using RuneScapeSolo.Net.Client.Game;

namespace RuneScapeSolo.Net.Client
{
    public class GameImageMiddleMan : GameImage
    {

        public GameImageMiddleMan(int width, int height, int size)
            : base(width, height, size)
        {

        }

        public override void DrawVisibleEntity(int x, int y, int width, int height, int objectId, int unknownParam1, int unknownParam2)
        {
            if (objectId >= 50000)
            {
                gameReference.DrawTeleBubble(x, y, width, height, objectId - 50000, unknownParam1, unknownParam2);
                return;
            }
            if (objectId >= 40000)
            {
                gameReference.DrawItem(x, y, width, height, objectId - 40000, unknownParam1, unknownParam2);
                return;
            }
            if (objectId >= 20000)
            {
                gameReference.DrawNpc(x, y, width, height, objectId - 20000, unknownParam1, unknownParam2);
                return;
            }
            if (objectId >= 5000)
            {
                gameReference.DrawPlayer(x, y, width, height, objectId - 5000, unknownParam1, unknownParam2);
                return;
            }

            DrawEntity(x, y, width, height, objectId);
        }

        public GameClient gameReference;
    }
}
