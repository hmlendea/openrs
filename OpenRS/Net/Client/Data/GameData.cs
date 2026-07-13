using System;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Data
{
    public sealed class GameData
    {
        public static int spellProjectileCount;
        public static int overlayTextureCount;
        public static string[] modelName = new string[5000];
        public static int modelCount;
        public static string[] wallObjectModelNames = new string[5000];
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

            int itemCount = ReadShort();

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadString();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadShort();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadInt();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadShort();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadInt();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadByte();
            }

            for (int itemIndex = 0; itemIndex < itemCount; itemIndex += 1)
            {
                ReadByte();
            }

            int npcCount = ReadShort();

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadString();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadString();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                for (int animationPartIndex = 0; animationPartIndex < 12; animationPartIndex += 1)
                {
                    ReadByte();
                }
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadInt();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadShort();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadShort();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadByte();
            }

            for (int npcIndex = 0; npcIndex < npcCount; npcIndex += 1)
            {
                ReadString();
            }

            int textureCount = ReadShort();

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex += 1)
            {
                ReadString();
            }

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex += 1)
            {
                ReadString();
            }

            int animationCount = ReadShort();

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                ReadString();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                ReadInt();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                ReadByte();
            }

            for (int animationIndex = 0; animationIndex < animationCount; animationIndex += 1)
            {
                ReadByte();
            }

            int objectCount = ReadShort();

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadString();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadByte();
            }

            for (int objectIndex = 0; objectIndex < objectCount; objectIndex += 1)
            {
                ReadByte();
            }

            int wallObjectCount = ReadShort();

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadString();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadShort();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadInt();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadInt();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadByte();
            }

            for (int wallObjectIndex = 0; wallObjectIndex < wallObjectCount; wallObjectIndex += 1)
            {
                ReadByte();
            }

            int elevationCount = ReadShort();

            for (int elevationIndex = 0; elevationIndex < elevationCount; elevationIndex += 1)
            {
                ReadByte();
            }

            for (int elevationIndex = 0; elevationIndex < elevationCount; elevationIndex += 1)
            {
                ReadByte();
            }

            overlayTextureCount = ReadShort();

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex += 1)
            {
                ReadInt();
            }

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex += 1)
            {
                ReadByte();
            }

            for (int overlayIndex = 0; overlayIndex < overlayTextureCount; overlayIndex += 1)
            {
                ReadByte();
            }

            spellProjectileCount = ReadShort();
            int spellCount = ReadShort();

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                ReadString();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                ReadString();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                ReadByte();
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                int runeIdCount = ReadByte();

                for (int runeIndex = 0; runeIndex < runeIdCount; runeIndex += 1)
                {
                    ReadShort();
                }
            }

            for (int spellIndex = 0; spellIndex < spellCount; spellIndex += 1)
            {
                int runeCount = ReadByte();

                for (int runeIndex = 0; runeIndex < runeCount; runeIndex += 1)
                {
                    ReadByte();
                }
            }

            stringData = null;
            integerData = null;
        }
    }
}
