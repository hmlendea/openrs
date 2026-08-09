using System;

namespace OpenRS.Net.Client.Input
{
    internal sealed class CameraInputHandler(GameClient client)
    {
        private static int ActionPictureStep => 1;

        private static int AutoAngleMask => 7;

        private static int AutoRotationAccelerationIncrement => 1;

        private static int AutoRotationDifferenceMaximum => 128;

        private static int AutoRotationDivisor => 256;

        private static int AutoRotationRoundUp => 255;

        private static int CameraAngleFullRotation => 256;

        private static int CameraAngleMultiplier => 32;

        private static int CameraDistanceFollowDivisor => 15;

        private static int CameraDistanceFollowMinimum => 500;

        private static int CameraDistanceFogMaximum => 750;

        private static int CameraDistanceMaximum => 1250;

        private static int CameraDistanceMinimum => 550;

        private static int CameraDistanceStep => 4;

        private static int CameraFollowLagBase => 16;

        private static int CameraFollowThreshold => 500;

        private static int CameraLightingLevel => 17;

        private static int CameraRotationMask => 0xff;

        private static int CameraRotationStep => 2;

        private static int MaximumCameraAngleChecks => 8;

        private static int MaximumMiniObjectCoordinate => 96;

        private static int ModelAnimationStepThreshold => 5;

        private static int ModelClawSpellFrameCount => 5;

        private static int ModelFireLightningFrameCount => 3;

        private static int ModelTorchFrameCount => 4;

        private static int SpecialObjectType => 74;

        private static int TeleBubbleFrameLifetime => 50;

        internal void HandleCameraInput()
        {
            if (client.configCameraAutoAngle)
            {
                HandleAutoAngleInput();
            }
            else
            {
                HandleManualRotationInput();
            }

            UpdateCameraDistanceFromKeys();
            UpdateFogOfWarDistance();
        }

        internal void UpdateCameraTracking()
        {
            SnapCameraTargetToPlayerWhenFarAway();

            if (client.cameraAutoAngleDebug)
            {
                return;
            }

            MoveCameraTargetTowardsPlayer();
            UpdateAutoRotation();
        }

        internal void UpdateSceneEffects()
        {
            UpdateActionPictureType();
            client.gameCamera.UpdateLighting(CameraLightingLevel);
            UpdateAnimatedModelFrames();
            UpdateSpecialObjectAnimations();
            UpdateTeleBubbleAnimations();
        }

        private void FindValidCameraAngle(int rotationStep)
        {
            for (int angleCheckIndex = 0;
                angleCheckIndex < MaximumCameraAngleChecks;
                angleCheckIndex += 1)
            {
                if (client.IsValidCameraAngle(client.cameraAutoAngle))
                {
                    return;
                }

                client.cameraAutoAngle =
                    (client.cameraAutoAngle + rotationStep) & AutoAngleMask;
            }
        }

        private void HandleAutoAngleInput()
        {
            if (client.cameraAutoRotationAmount != 0 && !client.cameraAutoAngleDebug)
            {
                return;
            }

            if (client.keyLeftDown)
            {
                RotateAutoAngle(1, true);
            }

            if (client.keyRightDown)
            {
                RotateAutoAngle(7, false);
            }
        }

        private void HandleManualRotationInput()
        {
            if (client.keyLeftDown)
            {
                client.cameraRotation =
                    (client.cameraRotation + CameraRotationStep) & CameraRotationMask;
                return;
            }

            if (client.keyRightDown)
            {
                client.cameraRotation =
                    (client.cameraRotation - CameraRotationStep) & CameraRotationMask;
            }
        }

        private void MoveCameraTargetTowardsPlayer()
        {
            int followDivisor =
                CameraFollowLagBase +
                (client.cameraDistance - CameraDistanceFollowMinimum) /
                CameraDistanceFollowDivisor;

            if (client.cameraAutoRotatePlayerX != client.ourPlayer.LocationX)
            {
                client.cameraAutoRotatePlayerX +=
                    (client.ourPlayer.LocationX - client.cameraAutoRotatePlayerX) /
                    followDivisor;
            }

            if (client.cameraAutoRotatePlayerY != client.ourPlayer.LocationY)
            {
                client.cameraAutoRotatePlayerY +=
                    (client.ourPlayer.LocationY - client.cameraAutoRotatePlayerY) /
                    followDivisor;
            }
        }

        private void RemoveTeleBubbleAt(int bubbleIndex)
        {
            client.teleBubbleCount -= 1;

            for (int shiftIndex = bubbleIndex;
                shiftIndex < client.teleBubbleCount;
                shiftIndex += 1)
            {
                client.teleBubbleX[shiftIndex] = client.teleBubbleX[shiftIndex + 1];
                client.teleBubbleY[shiftIndex] = client.teleBubbleY[shiftIndex + 1];
                client.teleBubbleTime[shiftIndex] = client.teleBubbleTime[shiftIndex + 1];
                client.teleBubbleType[shiftIndex] = client.teleBubbleType[shiftIndex + 1];
            }
        }

        private void RotateAutoAngle(int rotationStep, bool clearLeftKey)
        {
            client.cameraAutoAngle = (client.cameraAutoAngle + rotationStep) & AutoAngleMask;

            if (clearLeftKey)
            {
                client.keyLeftDown = false;
            }
            else
            {
                client.keyRightDown = false;
            }

            if (!client.cameraZoom)
            {
                SkipEvenCameraAngle(rotationStep);
                FindValidCameraAngle(rotationStep);
            }
        }

        private void SkipEvenCameraAngle(int rotationStep)
        {
            if ((client.cameraAutoAngle & 1) == 0)
            {
                client.cameraAutoAngle =
                    (client.cameraAutoAngle + rotationStep) & AutoAngleMask;
            }
        }

        private void SnapCameraTargetToPlayerWhenFarAway()
        {
            if (Math.Abs(client.cameraAutoRotatePlayerX - client.ourPlayer.LocationX) >
                CameraFollowThreshold ||
                Math.Abs(client.cameraAutoRotatePlayerY - client.ourPlayer.LocationY) >
                CameraFollowThreshold)
            {
                client.cameraAutoRotatePlayerX = client.ourPlayer.LocationX;
                client.cameraAutoRotatePlayerY = client.ourPlayer.LocationY;
            }
        }

        private void UpdateActionPictureType()
        {
            if (client.actionPictureType > 0)
            {
                client.actionPictureType -= ActionPictureStep;
            }
            else if (client.actionPictureType < 0)
            {
                client.actionPictureType += ActionPictureStep;
            }
        }

        private void UpdateAnimatedModelFrames()
        {
            client.modelUpdatingTimer += 1;

            if (client.modelUpdatingTimer <= ModelAnimationStepThreshold)
            {
                return;
            }

            client.modelUpdatingTimer = 0;
            client.modelFireLightningSpellNumber =
                (client.modelFireLightningSpellNumber + 1) % ModelFireLightningFrameCount;
            client.modelTorchNumber =
                (client.modelTorchNumber + 1) % ModelTorchFrameCount;
            client.modelClawSpellNumber =
                (client.modelClawSpellNumber + 1) % ModelClawSpellFrameCount;
        }

        private void UpdateAutoRotation()
        {
            if (!client.configCameraAutoAngle)
            {
                return;
            }

            int targetCameraRotation = client.cameraAutoAngle * CameraAngleMultiplier;
            int rotationDifference = targetCameraRotation - client.cameraRotation;
            int rotationDirection = 1;

            if (rotationDifference > AutoRotationDifferenceMaximum)
            {
                rotationDirection = -1;
                rotationDifference = CameraAngleFullRotation - rotationDifference;
            }
            else if (rotationDifference > 0)
            {
                rotationDirection = 1;
            }
            else if (rotationDifference < -AutoRotationDifferenceMaximum)
            {
                rotationDirection = 1;
                rotationDifference = CameraAngleFullRotation + rotationDifference;
            }
            else if (rotationDifference < 0)
            {
                rotationDirection = -1;
                rotationDifference = -rotationDifference;
            }

            if (rotationDifference != 0)
            {
                client.cameraAutoRotationAmount += AutoRotationAccelerationIncrement;
                client.cameraRotation +=
                    ((client.cameraAutoRotationAmount * rotationDifference) +
                    AutoRotationRoundUp) /
                    AutoRotationDivisor * rotationDirection;
                client.cameraRotation &= CameraRotationMask;
            }
            else
            {
                client.cameraAutoRotationAmount = 0;
            }
        }

        private void UpdateCameraDistanceFromKeys()
        {
            if (client.keyUpDown && client.cameraDistance > CameraDistanceMinimum)
            {
                client.cameraDistance -= CameraDistanceStep;
            }
            else if (client.keyDownDown && client.cameraDistance < CameraDistanceMaximum)
            {
                client.cameraDistance += CameraDistanceStep;
            }
        }

        private void UpdateFogOfWarDistance()
        {
            if (!client.fogOfWar)
            {
                return;
            }

            if ((client.cameraZoom && client.cameraDistance > CameraDistanceMinimum) ||
                client.cameraDistance > CameraDistanceFogMaximum)
            {
                client.cameraDistance -= CameraDistanceStep;
            }

            if (!client.cameraZoom && client.cameraDistance < CameraDistanceFogMaximum)
            {
                client.cameraDistance += CameraDistanceStep;
            }
        }

        private void UpdateSpecialObjectAnimations()
        {
            for (int objectIndex = 0; objectIndex < client.objectCount; objectIndex += 1)
            {
                int objectXCoordinate = client.objectX[objectIndex];
                int objectYCoordinate = client.objectY[objectIndex];

                if (objectXCoordinate >= 0 &&
                    objectYCoordinate >= 0 &&
                    objectXCoordinate < MaximumMiniObjectCoordinate &&
                    objectYCoordinate < MaximumMiniObjectCoordinate &&
                    client.objectType[objectIndex] == SpecialObjectType)
                {
                    client.objectArray[objectIndex].OffsetMiniPosition(1, 0, 0);
                }
            }
        }

        private void UpdateTeleBubbleAnimations()
        {
            for (int bubbleIndex = 0; bubbleIndex < client.teleBubbleCount; bubbleIndex += 1)
            {
                client.teleBubbleTime[bubbleIndex] += 1;

                if (client.teleBubbleTime[bubbleIndex] > TeleBubbleFrameLifetime)
                {
                    RemoveTeleBubbleAt(bubbleIndex);
                }
            }
        }
    }
}