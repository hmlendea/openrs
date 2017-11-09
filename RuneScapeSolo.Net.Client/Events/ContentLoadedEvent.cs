using System;

namespace RuneScapeSolo.Net.Client.Events
{
    public class ContentLoadedEventArgs : EventArgs
    {
        public string StatusText { get; set; }
        public decimal Progress { get; set; }

        public ContentLoadedEventArgs(string statusText, decimal progress)
        {
            StatusText = statusText;
            Progress = progress;
        }
    }
}
