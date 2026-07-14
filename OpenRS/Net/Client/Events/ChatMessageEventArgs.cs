namespace OpenRS.Net.Client.Events
{
    public sealed class ChatMessageEventArgs(string message)
    {
        public string Message { get; set; } = message;
    }
}
