namespace OpenRS.Net.Client
{
    using OpenRS.Net.Client.Game;

    public sealed class GameImageMiddleMan(int width, int height, int size) : GameImage(width, height, size)
    {
        public GameClient gameReference;

        public override void drawVisibleEntity(int x, int y, int width, int height, int objectId, int unknownParam1, int unknownParam2)
        {
            if (objectId >= 50000)
            {
                gameReference.drawTeleBubble(x, y, width, height, objectId - 50000, unknownParam1, unknownParam2);
                return;
            }

            if (objectId >= 40000)
            {
                gameReference.drawItem(x, y, width, height, objectId - 40000, unknownParam1, unknownParam2);
                return;
            }

            if (objectId >= 20000)
            {
                gameReference.drawNpc(x, y, width, height, objectId - 20000, unknownParam1, unknownParam2);
                return;
            }

            if (objectId >= 5000)
            {
                gameReference.drawPlayer(x, y, width, height, objectId - 5000, unknownParam1, unknownParam2);
                return;
            }

            drawEntity(x, y, width, height, objectId);
        }
    }
}
