namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectTransformController
    {
        private static int RotationComponentMask => 0xff;

        private static int ModifiedObjectState => 1;

        private static int TransformTypeIdentity => 0;

        private static int TransformTypePosition => 1;

        private static int TransformTypeRotation => 2;

        private static int TransformTypeScale => 3;

        private static int TransformTypeSecondaryScale => 4;

        internal static void SetPosition(GameObject gameObject, int x, int y, int z)
        {
            gameObject.positionX = x;
            gameObject.positionY = y;
            gameObject.positionZ = z;
            FinaliseTransform(gameObject);
        }

        internal static void OffsetPosition(
            GameObject gameObject,
            int xOffset,
            int yOffset,
            int zOffset)
        {
            gameObject.positionX += xOffset;
            gameObject.positionY += yOffset;
            gameObject.positionZ += zOffset;
            FinaliseTransform(gameObject);
        }

        internal static void SetRotation(GameObject gameObject, int x, int y, int z)
        {
            gameObject.rotationX = x & RotationComponentMask;
            gameObject.rotationY = y & RotationComponentMask;
            gameObject.rotationZ = z & RotationComponentMask;
            FinaliseTransform(gameObject);
        }

        internal static void OffsetMiniPosition(
            GameObject gameObject,
            int deltaX,
            int deltaY,
            int deltaZ)
        {
            gameObject.rotationX = (gameObject.rotationX + deltaX) & RotationComponentMask;
            gameObject.rotationY = (gameObject.rotationY + deltaY) & RotationComponentMask;
            gameObject.rotationZ = (gameObject.rotationZ + deltaZ) & RotationComponentMask;
            FinaliseTransform(gameObject);
        }

        internal static void CopyTranslation(GameObject destination, GameObject source)
        {
            destination.rotationX = source.rotationX;
            destination.rotationY = source.rotationY;
            destination.rotationZ = source.rotationZ;
            destination.positionX = source.positionX;
            destination.positionY = source.positionY;
            destination.positionZ = source.positionZ;
            FinaliseTransform(destination);
        }

        internal static void RecalculateTransformType(GameObject gameObject)
        {
            if (HasNonDefaultSecondaryOrTertiaryScale(gameObject))
            {
                gameObject.transformType = TransformTypeSecondaryScale;

                return;
            }

            if (HasNonDefaultPrimaryScale(gameObject))
            {
                gameObject.transformType = TransformTypeScale;

                return;
            }

            if (HasAnyRotation(gameObject))
            {
                gameObject.transformType = TransformTypeRotation;

                return;
            }

            if (HasAnyPosition(gameObject))
            {
                gameObject.transformType = TransformTypePosition;

                return;
            }

            gameObject.transformType = TransformTypeIdentity;
        }

        private static void FinaliseTransform(GameObject gameObject)
        {
            RecalculateTransformType(gameObject);
            gameObject.ObjectState = ModifiedObjectState;
        }

        private static bool HasNonDefaultSecondaryOrTertiaryScale(GameObject gameObject) =>
            gameObject.secondaryScaleX != GameObject.DefaultScale ||
            gameObject.secondaryScaleY != GameObject.DefaultScale ||
            gameObject.secondaryScaleZ != GameObject.DefaultScale ||
            gameObject.tertiaryScaleX != GameObject.DefaultScale ||
            gameObject.tertiaryScaleY != GameObject.DefaultScale ||
            gameObject.tertiaryScaleZ != GameObject.DefaultScale;

        private static bool HasNonDefaultPrimaryScale(GameObject gameObject) =>
            gameObject.scaleX != GameObject.DefaultScale ||
            gameObject.scaleY != GameObject.DefaultScale ||
            gameObject.scaleZ != GameObject.DefaultScale;

        private static bool HasAnyRotation(GameObject gameObject) =>
            gameObject.rotationX != 0 ||
            gameObject.rotationY != 0 ||
            gameObject.rotationZ != 0;

        private static bool HasAnyPosition(GameObject gameObject) =>
            gameObject.positionX != 0 ||
            gameObject.positionY != 0 ||
            gameObject.positionZ != 0;
    }
}
