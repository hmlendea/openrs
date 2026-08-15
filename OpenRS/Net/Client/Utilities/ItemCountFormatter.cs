namespace OpenRS.Net.Client.Utilities
{
    internal static class ItemCountFormatter
    {
        private static int MillionDisplayLengthThreshold => 8;

        private static int ThousandsDisplayLengthThreshold => 4;

        internal static string Format(int itemCount)
        {
            string formattedCount = AddThousandsSeparators(itemCount.ToString());

            if (formattedCount.Length > MillionDisplayLengthThreshold)
            {
                return
                    "@gre@" +
                    formattedCount[..^MillionDisplayLengthThreshold] +
                    " million @whi@(" +
                    formattedCount +
                    ")";
            }

            if (formattedCount.Length > ThousandsDisplayLengthThreshold)
            {
                return
                    "@cya@" +
                    formattedCount[..^ThousandsDisplayLengthThreshold] +
                    "K @whi@(" +
                    formattedCount +
                    ")";
            }

            return formattedCount;
        }

        private static string AddThousandsSeparators(string formattedCount)
        {
            for (int separatorIndex = formattedCount.Length - 3;
                separatorIndex > 0;
                separatorIndex -= 3)
            {
                formattedCount =
                    formattedCount[..separatorIndex] +
                    "," +
                    formattedCount[separatorIndex..];
            }

            return formattedCount;
        }
    }
}