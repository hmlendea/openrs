using System;

namespace OpenRS.Net.Client.Data
{
    internal sealed class GameData
    {
        private static int MaxModelCount => 5000;
        private static string NotAvailableModelName => "na";

        internal static string[] ModelNames { get; } = new string[MaxModelCount];
        internal static int ModelCount { get; private set; }

        internal static int GetModelNameIndex(string name)
        {
            if (string.Equals(name, NotAvailableModelName, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            for (int modelIndex = 0; modelIndex < ModelCount; modelIndex += 1)
            {
                if (string.Equals(ModelNames[modelIndex], name, StringComparison.OrdinalIgnoreCase))
                {
                    return modelIndex;
                }
            }

            ModelNames[ModelCount] = name;
            ModelCount += 1;

            return ModelCount - 1;
        }
    }
}

