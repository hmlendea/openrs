using System;

namespace OpenRS.Net.Client.Events
{
    public class ContentLoadedEventArgs(string statusText, decimal progress) : EventArgs
    {
        public string StatusText { get; set; } = statusText;
        public decimal Progress { get; set; } = progress;
    }
}
