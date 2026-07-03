namespace OpenRS.Net.Client.Events
{
    public delegate void ChatMessageEventHandler(object sender, ChatMessageEventArgs e);

    public class ChatMessageEventArgs(string message)
    {
        public string Message { get; set; } = message;
    }
}
