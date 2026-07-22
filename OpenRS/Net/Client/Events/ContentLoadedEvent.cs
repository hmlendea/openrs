using System;

namespace OpenRS.Net.Client.Events
{
    public sealed class ContentLoadedEventArgs(string statusText, decimal progress) : EventArgs
    {
        public string StatusText { get; set; } = statusText;

        public decimal Progress { get; set; } = progress;
    }
}
