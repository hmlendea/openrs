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

            for (int modelIndex = 0; modelIndex < modelCount; modelIndex += 1)
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

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemName[itemIndex] = ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemDescription[itemIndex] = ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemCommand[itemIndex] = ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemInventoryPicture[itemIndex] = ReadShort();

                if (itemInventoryPicture[itemIndex] + 1 > highestLoadedPicture)
                {
                    highestLoadedPicture = itemInventoryPicture[itemIndex] + 1;
                }
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemBasePrice[itemIndex] = ReadInt();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemStackable[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemUnused[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemIsEquippable[itemIndex] = ReadShort();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemPictureMask[itemIndex] = ReadInt();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemSpecial[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                itemMembers[itemIndex] = ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
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

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
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

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcName[npcIndex] = ReadString();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcDescription[npcIndex] = ReadString();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcAttack[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcStrength[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcHits[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcDefense[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcAttackable[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                for (int animationPartIndex = 0; animationPartIndex < 12; animationPartIndex += 1)
                {
                    npcAnimationCount[npcIndex][animationPartIndex] = ReadByte();

                    if (npcAnimationCount[npcIndex][animationPartIndex] == 255)
                    {
                        npcAnimationCount[npcIndex][animationPartIndex] = -1;
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcHairColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcTopColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcBottomColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcSkinColor[npcIndex] = ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcCameraArray1[npcIndex] = ReadShort();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcCameraArray2[npcIndex] = ReadShort();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcWalkModelArray[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcCombatModel[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcCombatSprite[npcIndex] = ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                npcCommand[npcIndex] = ReadString();
            }

            textureCount = ReadShort();
            textureName = new string[textureCount];
            textureSubName = new string[textureCount];

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex += 1)
            {
                textureName[textureIndex] = ReadString();
            }

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex += 1)
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

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                animationName[animationIndex] = ReadString();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                animationCharacterColor[animationIndex] = ReadInt();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                animationGenderModels[animationIndex] = ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                animationHasA[animationIndex] = ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                animationHasF[animationIndex] = ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
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

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectName[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectDescription[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectCommand1[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectCommand2[objectIndex] = ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectModelNumber[objectIndex] = GetModelNameIndex(ReadString());
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectWidth[objectIndex] = ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectHeight[objectIndex] = ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                objectType[objectIndex] = ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
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

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectName[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectDescription[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectCommand1[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectCommand2[wallObjectIndex] = ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectModelHeight[wallObjectIndex] = ReadShort();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectModel_FaceBack[wallObjectIndex] = ReadInt();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectModel_FaceFront[wallObjectIndex] = ReadInt();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectType[wallObjectIndex] = ReadByte();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                wallObjectUnknown[wallObjectIndex] = ReadByte();
            }

            elevationCount = ReadShort();
            roofs = new int[elevationCount];
            roofAltitudes = new int[elevationCount];

            for (int elevationIndex = 0; elevationIndex < elevationCount; elevationIndex += 1)
            {
                roofs[elevationIndex] = ReadByte();
            }

            for (int elevationIndex = 0; elevationIndex < elevationCount; elevationIndex += 1)
            {
                roofAltitudes[elevationIndex] = ReadByte();
            }

            overlayTextureCount = ReadShort();
            tileGroundOverlayTexture = new int[overlayTextureCount];
            tileGroundOverlayTypes = new int[overlayTextureCount];
            overlayTextureFlags = new int[overlayTextureCount];

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex += 1)
            {
                tileGroundOverlayTexture[overlayIndex] = ReadInt();
            }

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex += 1)
            {
                tileGroundOverlayTypes[overlayIndex] = ReadByte();
            }

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex += 1)
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

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                spellName[spellIndex] = ReadString();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                spellDescription[spellIndex] = ReadString();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                spellRequiredLevel[spellIndex] = ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                spellDifferentRuneCount[spellIndex] = ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                spellType[spellIndex] = ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                int runeIdCount = ReadByte();
                spellRequiredRuneIds[spellIndex] = new int[runeIdCount];

                for (int runeIndex = 0; runeIndex < runeIdCount; runeIndex += 1)
                {
                    spellRequiredRuneIds[spellIndex][runeIndex] = ReadShort();
                }
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                int runeCount = ReadByte();
                spellRequiredRuneCount[spellIndex] = new int[runeCount];

                for (int runeIndex = 0; runeIndex < runeCount; runeIndex += 1)
                {
                    spellRequiredRuneCount[spellIndex][runeIndex] = ReadByte();
                }
            }

            prayerCount = ReadShort();
            prayerName = new string[prayerCount];
            prayerDescription = new string[prayerCount];
            prayerRequiredLevel = new int[prayerCount];
            prayerDrainRate = new int[prayerCount];

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex += 1)
            {
                prayerName[prayerIndex] = ReadString();
            }

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex += 1)
            {
                prayerDescription[prayerIndex] = ReadString();
            }

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex += 1)
            {
                prayerRequiredLevel[prayerIndex] = ReadByte();
            }

            for (int prayerIndex = 0; prayerIndex < prayerCount; prayerIndex += 1)
            {
                prayerDrainRate[prayerIndex] = ReadByte();
            }

            stringData = null;
            integerData = null;
        }
    }
}
