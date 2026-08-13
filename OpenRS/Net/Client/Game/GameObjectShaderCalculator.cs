using System;

namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectShaderCalculator
    {
        private static int MaximumShadeLevel => 256;

        private static int LightIntensityMultiplier => 4;

        private static int AmbientLightOffsetRange => 64;

        private static int AmbientLightScale => 16;

        private static int AmbientLightBase => 128;

        internal static void ApplyShading(
            GameObject gameObject,
            bool applyShadeValue,
            int lightIntensity,
            int ambientLight,
            int directionX,
            int directionY,
            int directionZ)
        {
            SetShadeLevels(gameObject, lightIntensity, ambientLight);

            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            InitialiseGouraudShades(gameObject, applyShadeValue);
            ApplyLightDirection(gameObject, directionX, directionY, directionZ);
        }

        internal static void SetLightingColours(
            GameObject gameObject,
            int lightIntensity,
            int ambientLight,
            int directionX,
            int directionY,
            int directionZ)
        {
            SetShadeLevels(gameObject, lightIntensity, ambientLight);

            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            ApplyLightDirection(gameObject, directionX, directionY, directionZ);
        }

        internal static void OffsetLightingColours(
            GameObject gameObject,
            int directionX,
            int directionY,
            int directionZ)
        {
            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            ApplyLightDirection(gameObject, directionX, directionY, directionZ);
        }

        private static void SetShadeLevels(
            GameObject gameObject,
            int lightIntensity,
            int ambientLight)
        {
            gameObject.BaseShadeLevel =
                MaximumShadeLevel - lightIntensity * LightIntensityMultiplier;
            gameObject.AmbientLightLevel =
                (AmbientLightOffsetRange - ambientLight) * AmbientLightScale +
                AmbientLightBase;
        }

        private static void ApplyLightDirection(
            GameObject gameObject,
            int directionX,
            int directionY,
            int directionZ)
        {
            gameObject.LightDirectionX = directionX;
            gameObject.LightDirectionY = directionY;
            gameObject.LightDirectionZ = directionZ;
            gameObject.LightMagnitude = (int)Math.Sqrt(
                directionX * directionX + directionY * directionY + directionZ * directionZ);
            GameObjectNormalCalculator.Recalculate(gameObject);
        }

        private static void InitialiseGouraudShades(GameObject gameObject, bool applyShadeValue)
        {
            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                gameObject.GouraudShade[faceIndex] = 0;

                if (applyShadeValue)
                {
                    gameObject.GouraudShade[faceIndex] = GameObject.DefaultShadeValue;
                }
            }
        }

    }
}
