using System;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Data
{
    public sealed class GameData
    {
        public static int itemCount;
        public static int highestLoadedPicture;
        public static string[] npcName;
        public static string[] npcDescription;
        public static string[] npcCommand;
        public static int spellProjectileCount;
        public static int objectCount;
        public static string[] wallObjectName;
        public static string[] wallObjectDescription;
        public static string[] wallObjectCommand1;
        public static string[] wallObjectCommand2;
        public static int spellCount;
        public static int[] npcCameraArray1;
        public static int[] npcCameraArray2;
        public static string[] itemName;
        public static string[] itemDescription;
        public static string[] itemCommand;
        public static int[] itemInventoryPicture;
        public static int[] itemBasePrice;
        public static int[] itemStackable;
        public static int[] itemUnused;
        public static int[] itemIsEquippable;
        public static int[] itemPictureMask;
        public static int[] itemSpecial;
        public static int[] itemMembers;
        public static string[] prayerName;
        public static string[] prayerDescription;
        public static string[] animationName;
        public static int prayerCount;
        public static int[] npcHairColor;
        public static int[] npcTopColor;
        public static int[] npcBottomColor;
        public static int[] npcSkinColor;
        public static int overlayTextureCount;
        public static int wallObjectCount;
        public static int animationCount;
        public static string[] modelName = new string[5000];
        public static string[] textureName;
        public static string[] textureSubName;
        public static string[] objectName;
        public static string[] objectDescription;
        public static string[] objectCommand1;
        public static string[] objectCommand2;
        public static int textureCount;
        public static int elevationCount;
        public static int[] prayerRequiredLevel;
        public static int[] prayerDrainRate;
        public static int[] tileGroundOverlayTexture;
        public static int[] tileGroundOverlayTypes;
        public static int[] overlayTextureFlags;
        public static string[] spellName;
        public static string[] spellDescription;
        public static int[] npcWalkModelArray;
        public static int[] npcCombatModel;
        public static int[] npcCombatSprite;
        public static int[][] npcAnimationCount;
        public static int[] npcAttack;
        public static int[] npcStrength;
        public static int[] npcHits;
        public static int[] npcDefense;
        public static int[] npcAttackable;
        public static int npcCount;
        public static int[] animationCharacterColor;
        public static int[] animationGenderModels;
        public static int[] animationHasA;
        public static int[] animationHasF;
        public static int[] animationNumber;
        public static int[] roofs;
        public static int[] roofAltitudes;
        public static int modelCount;
        public static int[] spellRequiredLevel;
        public static int[] spellDifferentRuneCount;
        public static int[] spellType;
        public static int[][] spellRequiredRuneIds;
        public static int[][] spellRequiredRuneCount;
        public static int[] objectModelNumber;
        public static int[] objectWidth;
        public static int[] objectHeight;
        public static int[] objectType;
        public static int[] objectGroundItemVar;
        public static string[] wallObjectModelNames = new string[5000];
        public static int[] wallObjectModelHeight;
        public static int[] wallObjectModel_FaceBack;
        public static int[] wallObjectModel_FaceFront;
        public static int[] wallObjectType;
        public static int[] wallObjectUnknown;
        public static string[] floorModelNames = new string[5000];
        public static int additionalModelCount;

        private static sbyte[] stringData;
        private static sbyte[] integerData;
        private static int stringDataIndex;
        private static int integerDataIndex;

        public static int GetModelNameIndex(string name)
        {
            if (string.Equals(name, "na", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            for (int modelIndex = 0; modelIndex < modelCount; modelIndex++)
            {
                if (string.Equals(modelName[modelIndex], name, StringComparison.OrdinalIgnoreCase))
                {
                    return modelIndex;
                }
            }

            modelName[modelCount] = name;
            modelCount += 1;

            return modelCount - 1;
        }

        public static int ReadByte()
        {
            int byteValue = integerData[integerDataIndex] & 0xff;
            integerDataIndex += 1;

            return byteValue;
        }

        public static int ReadShort()
        {
            int shortValue = DataOperations.GetShort(integerData, integerDataIndex);
            integerDataIndex += 2;

            return shortValue;
        }

        public static int ReadInt()
        {
            int intValue = DataOperations.GetInt(integerData, integerDataIndex);
            integerDataIndex += 4;

            if (intValue > 0x5f5e0ff)
            {
                intValue = 0x5f5e0ff - intValue;
            }

            return intValue;
        }

        public static string ReadString()
        {
            string result = "";

            while (stringData[stringDataIndex] != 0)
            {
                result += (char)stringData[stringDataIndex];
                stringDataIndex += 1;
            }

            stringDataIndex += 1;

            return result;
        }

        public static void Load(sbyte[] rawData)
        {
            stringData = DataOperations.LoadData("string.dat", 0, rawData);
            stringDataIndex = 0;
            integerData = DataOperations.LoadData("integer.dat", 0, rawData);
            integerDataIndex = 0;

            itemCount = ReadShort();
            itemName = new string[itemCount];
            itemDescription = new string[itemCount];
            itemCommand = new string[itemCount];
            itemInventoryPicture = new int[itemCount];
            itemBasePrice = new int[itemCount];
            itemStackable = new int[itemCount];
            itemUnused = new int[itemCount];
            itemIsEquippable = new int[itemCount];
            itemPictureMask = new int[itemCount];
            itemSpecial = new int[itemCount];
            itemMembers = new int[itemCount];

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemName[itemIndex] = ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemDescription[itemIndex] = ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemCommand[itemIndex] = ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemInventoryPicture[itemIndex] = ReadShort();

                if (itemInventoryPicture[itemIndex] + 1 > highestLoadedPicture)
                {
                    highestLoadedPicture = itemInventoryPicture[itemIndex] + 1;
                }
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemBasePrice[itemIndex] = ReadInt();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemStackable[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemUnused[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemIsEquippable[itemIndex] = ReadShort();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemPictureMask[itemIndex] = ReadInt();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemSpecial[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                itemMembers[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                if (!Config.MembersFeatures && itemMembers[itemIndex] == 1)
                {
                    itemName[itemIndex] = "Members object";
                    itemDescription[itemIndex] = "You need to be a member to use this object";
                    itemBasePrice[itemIndex] = 0;
                    itemCommand[itemIndex] = "";
                    itemUnused[0] = 0;
                    itemIsEquippable[itemIndex] = 0;
                    itemSpecial[itemIndex] = 1;
                }
            }

            npcCount = ReadShort();
            npcName = new string[npcCount];
            npcDescription = new string[npcCount];
            npcCommand = new string[npcCount];
            npcAttack = new int[npcCount];
            npcStrength = new int[npcCount];
            npcHits = new int[npcCount];
            npcDefense = new int[npcCount];
            npcAttackable = new int[npcCount];
            npcAnimationCount = new int[npcCount][];

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcAnimationCount[npcIndex] = new int[12];
            }

            npcHairColor = new int[npcCount];
            npcTopColor = new int[npcCount];
            npcBottomColor = new int[npcCount];
            npcSkinColor = new int[npcCount];
            npcCameraArray1 = new int[npcCount];
            npcCameraArray2 = new int[npcCount];
            npcWalkModelArray = new int[npcCount];
            npcCombatModel = new int[npcCount];
            npcCombatSprite = new int[npcCount];

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcName[npcIndex] = ReadString();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcDescription[npcIndex] = ReadString();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcAttack[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcStrength[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcHits[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcDefense[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcAttackable[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                for (int animationPartIndex = 0; animationPartIndex < 12; animationPartIndex++)
                {
                    npcAnimationCount[npcIndex][animationPartIndex] = ReadByte();

                    if (npcAnimationCount[npcIndex][animationPartIndex] == 255)
                    {
                        npcAnimationCount[npcIndex][animationPartIndex] = -1;
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcHairColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcTopColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcBottomColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcSkinColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcCameraArray1[npcIndex] = ReadShort();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcCameraArray2[npcIndex] = ReadShort();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcWalkModelArray[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcCombatModel[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcCombatSprite[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex++)
            {
                npcCommand[npcIndex] = ReadString();
            }

            textureCount = ReadShort();
            textureName = new string[textureCount];
            textureSubName = new string[textureCount];

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
            {
                textureName[textureIndex] = ReadString();
            }

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
            {
                textureSubName[textureIndex] = ReadString();
            }

            animationCount = ReadShort();
            animationName = new string[animationCount];
            animationCharacterColor = new int[animationCount];
            animationGenderModels = new int[animationCount];
            animationHasA = new int[animationCount];
            animationHasF = new int[animationCount];
            animationNumber = new int[animationCount];

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                animationName[animationIndex] = ReadString();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                animationCharacterColor[animationIndex] = ReadInt();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                animationGenderModels[animationIndex] = ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                animationHasA[animationIndex] = ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                animationHasF[animationIndex] = ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
            {
                animationNumber[animationIndex] = ReadByte();
            }

            objectCount = ReadShort();
            objectName = new string[objectCount];
            objectDescription = new string[objectCount];
            objectCommand1 = new string[objectCount];
            objectCommand2 = new string[objectCount];
            objectModelNumber = new int[objectCount];
            objectWidth = new int[objectCount];
            objectHeight = new int[objectCount];
            objectType = new int[objectCount];
            objectGroundItemVar = new int[objectCount];

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectName[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectDescription[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectCommand1[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectCommand2[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectModelNumber[objectIndex] = GetModelNameIndex(ReadString());
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectWidth[objectIndex] = ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectHeight[objectIndex] = ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectType[objectIndex] = ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
            {
                objectGroundItemVar[objectIndex] = ReadByte();
            }

            wallObjectCount = ReadShort();
            wallObjectName = new string[wallObjectCount];
            wallObjectDescription = new string[wallObjectCount];
            wallObjectCommand1 = new string[wallObjectCount];
            wallObjectCommand2 = new string[wallObjectCount];
            wallObjectModelHeight = new int[wallObjectCount];
            wallObjectModel_FaceBack = new int[wallObjectCount];
            wallObjectModel_FaceFront = new int[wallObjectCount];
            wallObjectType = new int[wallObjectCount];
            wallObjectUnknown = new int[wallObjectCount];

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectName[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectDescription[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectCommand1[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectCommand2[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectModelHeight[wallObjectIndex] = ReadShort();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectModel_FaceBack[wallObjectIndex] = ReadInt();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectModel_FaceFront[wallObjectIndex] = ReadInt();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectType[wallObjectIndex] = ReadByte();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex++)
            {
                wallObjectUnknown[wallObjectIndex] = ReadByte();
            }

            elevationCount = ReadShort();
            roofs = new int[elevationCount];
            roofAltitudes = new int[elevationCount];

            for (int elevationIndex = 0; elevationIndex < elevationCount; elevationIndex++)
            {
                roofs[elevationIndex] = ReadByte();
            }

            for (int elevationIndex = 0; elevationIndex < elevationCount; elevationIndex++)
            {
                roofAltitudes[elevationIndex] = ReadByte();
            }

            overlayTextureCount = ReadShort();
            tileGroundOverlayTexture = new int[overlayTextureCount];
            tileGroundOverlayTypes = new int[overlayTextureCount];
            overlayTextureFlags = new int[overlayTextureCount];

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex++)
            {
                tileGroundOverlayTexture[overlayIndex] = ReadInt();
            }

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex++)
            {
                tileGroundOverlayTypes[overlayIndex] = ReadByte();
            }

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex++)
            {
                overlayTextureFlags[overlayIndex] = ReadByte();
            }

            spellProjectileCount = ReadShort();
            spellCount = ReadShort();
            spellName = new string[spellCount];
            spellDescription = new string[spellCount];
            spellRequiredLevel = new int[spellCount];
            spellDifferentRuneCount = new int[spellCount];
            spellType = new int[spellCount];
            spellRequiredRuneIds = new int[spellCount][];
            spellRequiredRuneCount = new int[spellCount][];

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                spellName[spellIndex] = ReadString();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                spellDescription[spellIndex] = ReadString();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                spellRequiredLevel[spellIndex] = ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                spellDifferentRuneCount[spellIndex] = ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                spellType[spellIndex] = ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                int runeIdCount = ReadByte();
                spellRequiredRuneIds[spellIndex] = new int[runeIdCount];

                for (int runeIndex = 0; runeIndex < runeIdCount; runeIndex++)
                {
                    spellRequiredRuneIds[spellIndex][runeIndex] = ReadShort();
                }
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex++)
            {
                int runeCount = ReadByte();
                spellRequiredRuneCount[spellIndex] = new int[runeCount];

                for (int runeIndex = 0; runeIndex < runeCount; runeIndex++)
                {
                    spellRequiredRuneCount[spellIndex][runeIndex] = ReadByte();
                }
            }

            prayerCount = ReadShort();
            prayerName = new string[prayerCount];
            prayerDescription = new string[prayerCount];
            prayerRequiredLevel = new int[prayerCount];
            prayerDrainRate = new int[prayerCount];

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex++)
            {
                prayerName[prayerIndex] = ReadString();
            }

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex++)
            {
                prayerDescription[prayerIndex] = ReadString();
            }

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex++)
            {
                prayerRequiredLevel[prayerIndex] = ReadByte();
            }

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex++)
            {
                prayerDrainRate[prayerIndex] = ReadByte();
            }

            stringData = null;
            integerData = null;
        }
    }
}
