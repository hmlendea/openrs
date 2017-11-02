using RuneScapeSolo.Lib.Game;

namespace RuneScapeSolo.Lib
{
    public class GameImageMiddleMan : GameImage
    {

        public GameImageMiddleMan(int width, int height, int size /*, java.awt.Component arg2*/)
            : base(width, height, size)
        {
            //super(i, l, i1, c);
        }

        public override void DrawVisibleEntity(int x, int y, int width, int height, int objectId, int l1, int i2)
        {
            if (objectId >= 50000)
            {
                gameReference.DrawTeleBubble(x, y, width, height, objectId - 50000, l1, i2);
                return;
            }
            if (objectId >= 40000)
            {
                gameReference.DrawItem(x, y, width, height, objectId - 40000, l1, i2);
                return;
            }
            if (objectId >= 20000)
            {
                gameReference.DrawNpc(x, y, width, height, objectId - 20000, l1, i2);
                return;
            }
            if (objectId >= 5000)
            {
                gameReference.DrawPlayer(x, y, width, height, objectId - 5000, l1, i2);
                return;
            }
            else
            {
                base.DrawEntity(x, y, width, height, objectId);
                return;
            }
        }

        public mudclient gameReference;
    }
}
