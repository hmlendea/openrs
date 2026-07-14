using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using OpenRS.Settings;

namespace OpenRS.Net.Client.Game
{
    public static class ChatFilter
    {
        private static Regex badWordPattern;
        private static Regex urlPattern;

        public static void Load()
        {
            IEnumerable<string> badWords = LoadWordSet("bad.txt");
            IEnumerable<string> badHosts = LoadWordSet("hosts.txt");
            IEnumerable<string> tlds = LoadWordSet("tlds.txt");

            // fragments.txt contains the original binary encoding table
            // (character substitution codes for leet-speak detection).
            // It is not used by this simplified plaintext filter.

            string badWordAlternation = string.Join("|", badWords.Select(Regex.Escape));
            badWordPattern = new Regex(
                $@"\b({badWordAlternation})\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string hostAlternation = string.Join("|", badHosts.Select(Regex.Escape));
            string tldAlternation = string.Join("|", tlds.Select(Regex.Escape));
            urlPattern = new Regex(
                $@"\b({hostAlternation})\.({tldAlternation})\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string FilterChat(string message)
        {
            if (badWordPattern is null)
            {
                return message;
            }

            string filteredMessage = badWordPattern.Replace(message, "****");
            filteredMessage = urlPattern.Replace(filteredMessage, "****");

            return filteredMessage;
        }

        private static IEnumerable<string> LoadWordSet(string fileName)
        {
            string filePath = Path.Combine(ApplicationPaths.ChatFilterDirectory, fileName);
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            return lines.Where(line => !string.IsNullOrWhiteSpace(line));
        }
    }
}
