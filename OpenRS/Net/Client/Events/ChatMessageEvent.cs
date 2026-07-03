namespace OpenRS.Net.Client.Events
{
    public delegate void ChatMessageEventHandler(object sender, ChatMessageEventArgs e);
    
    public class ChatMessageEventArgs
    {
        public string Message { get; set; }
        
        public ChatMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
