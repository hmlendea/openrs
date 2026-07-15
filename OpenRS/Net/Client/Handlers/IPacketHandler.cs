namespace OpenRS.Net.Client.Handlers
{
    public interface IPacketHandler
    {
        void HandlePacket(int packetID, int packetLength, sbyte[] packetData);
    }
}
