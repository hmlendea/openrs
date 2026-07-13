using System;

namespace OpenRS.Net.Client.Data
{
    public sealed class GameData
    {
        public static string[] modelName = new string[5000];
        public static int modelCount;

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
    }
}

