namespace OpenRS.Net.Client.Handlers
{
    internal sealed class PacketReadCursor
    {
        internal int Index { get; set; }

        internal PacketReadCursor(int index)
        {
            Index = index;
        }
    }
}
